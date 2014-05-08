using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class TrackerUdpProtocol
    {
        public int Port { get; private set; }
        public BDecoded.BDictionary Parsed_response { get; set; }
        public String LastError { get; private set; }
        protected int TransactionID { get; set; }
        protected long ConnectionID { get; set; }
        private Client Client { get; set; }

        public const long TORRENT_PROTOCOL_CODE = 0x41727101980;
        public enum ACTIONS: int {Connect = 0, Announce = 1}
        public enum EVENTS : int {None = 0, Completed = 1, Started = 2, Stopped = 3}
        protected String UDP_ADDRESS_PATTERN = @"udp://(.+):(\d{1,4})";
        private const int TIMEOUT = 6000;

        public TrackerUdpProtocol(Client client)
        {
            this.Client = client;
            this.Port = client.port_listen;
            this.LastError = "";
            return;
        }

        public bool Connect(String address, Torrent torrent)
        {
            if (!Regex.IsMatch(address, UDP_ADDRESS_PATTERN))
            {
                LastError = "Unknown address format";
                return false;
            }
            UdpClient udp_client = CreateClient(address);
            byte[] connection_response = SendRequest(udp_client, ConnectionRequest());
            if (connection_response == null || !ProcessConnectionResponse(connection_response))
            {
                LastError = "Wrong connection response from tracker";
                return false;
            }
            byte[] announce_response = SendRequest(udp_client, AnnounceRequest(torrent));
            if (announce_response == null)
            {
                LastError = "Wrong announce response from tracker";
                return false;
            }
            udp_client.Close();
            return true;
        }

        /// <summary>
        /// Send data using configured udp client
        /// </summary>
        /// <returns>received data or null</returns>
        protected byte[] SendRequest(UdpClient udp_client, byte[] request_data)
        {
            try
            {
                udp_client.Send(request_data, request_data.Length);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] response = udp_client.Receive(ref endpoint);
                return response;
            }
            catch (SocketException ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Create udp client configured for specified address
        /// </summary>
        /// <param name="address">Domain udp address</param>
        /// <returns>client or null</returns>
        protected UdpClient CreateClient(String address)
        {
            int target_port;
            String host_name;
            ParseTrackerAddress(address, out host_name, out target_port);
            UdpClient udp_client;
            try
            {
                udp_client = new UdpClient(0, AddressFamily.InterNetwork);
                udp_client.Connect(host_name, target_port);
                udp_client.Client.ReceiveTimeout = TIMEOUT;
                return udp_client;
            }
            catch (SocketException ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Parse target address to define host name and port
        /// </summary>
        /// <returns>true if parsing was successful</returns>
        protected bool ParseTrackerAddress(String address, out String host_name, out int port)
        {
            var parse_result = System.Text.RegularExpressions.Regex.Match(address, UDP_ADDRESS_PATTERN);
            host_name = parse_result.Groups[1].Value;
            bool correct_port = Int32.TryParse(parse_result.Groups[2].Value, out port);
            return !String.IsNullOrEmpty(host_name) && correct_port;
        }

        /// <summary>
        /// Torrent protocol message for creating a connection
        /// </summary>
        /// <returns>Message content</returns>
        protected byte[] ConnectionRequest()
        {
            TransactionID = GenerateTransactionID();
            List<byte> message_content = new List<byte>();
            message_content.AddRange(BitConverter.GetBytes(TORRENT_PROTOCOL_CODE).Reverse().ToArray());
            message_content.AddRange(BitConverter.GetBytes((Int32)ACTIONS.Connect));
            message_content.AddRange(BitConverter.GetBytes(TransactionID));
            if (message_content.Count != 16) throw new FormatException("Wrong udp request");
            return message_content.ToArray();
        }

        protected byte[] AnnounceRequest(Torrent torrent)
        {
            TransactionID = GenerateTransactionID();
            List<byte> message_content = new List<byte>();
            message_content.AddRange(BitConverter.GetBytes((Int64)ConnectionID)); //8
            message_content.AddRange(BitConverter.GetBytes((Int32)ACTIONS.Announce)); //4
            message_content.AddRange(BitConverter.GetBytes((Int32)TransactionID)); //4
            message_content.AddRange(torrent.InfoHash); // 20
            message_content.AddRange(Client.Id); // 20
            message_content.AddRange(BitConverter.GetBytes((Int64)0)); // downloaded
            message_content.AddRange(BitConverter.GetBytes((Int64)0)); // left
            message_content.AddRange(BitConverter.GetBytes((Int64)0)); // uploaded
            message_content.AddRange(BitConverter.GetBytes((Int32)EVENTS.None)); // event
            message_content.AddRange(BitConverter.GetBytes((Int32)0)); // IP (default)
            message_content.AddRange(BitConverter.GetBytes((Int32)0)); // key ???
            message_content.AddRange(BitConverter.GetBytes((Int32)(-1))); // num_want (default)
            message_content.AddRange(BitConverter.GetBytes((Int16)0)); // port
            if (message_content.Count != 98) throw new FormatException("Wrong udp request");
            return message_content.ToArray();
        }
        

        /// <summary>
        /// Parse connection response and define connection id
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected bool ProcessConnectionResponse(byte[] response)
        {
            int action = BitConverter.ToInt32(response, 0);
            int receiver_transaction_id = BitConverter.ToInt32(response, 4);
            ConnectionID = BitConverter.ToInt64(response, 8);
            return action == 0 && receiver_transaction_id == TransactionID;
        }

        /// <summary>
        /// Identifies response for request
        /// </summary>
        /// <returns>ID</returns>
        protected int GenerateTransactionID()
        {
            return (Int32)(new Random(3).Next());
        }

    }
}
