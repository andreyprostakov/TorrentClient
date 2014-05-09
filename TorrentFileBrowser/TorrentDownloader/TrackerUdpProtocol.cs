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
        public String LastError { get; private set; }
        protected int TransactionID;
        protected long ConnectionID;
        private Client Client;

        public const long TORRENT_PROTOCOL_CODE = 0x41727101980;
        public enum ACTIONS: int {Connect = 0, Announce = 1, Scrape = 2, Error = 3}
        public enum EVENTS : int {None = 0, Completed = 1, Started = 2, Stopped = 3}
        protected String UDP_ADDRESS_PATTERN = @"udp://(.+):(\d{1,4})";
        private const int TIMEOUT = 1000;

        public TrackerUdpProtocol(Client client)
        {
            this.Client = client;
            this.Port = client.port_listen;
            this.LastError = "";
            return;
        }

        public TorrentTrackerInfo Connect(String address, Torrent torrent)
        {
            if (!Regex.IsMatch(address, UDP_ADDRESS_PATTERN))
            {
                LastError = "Unknown address format";
                return null;
            }
            UdpClient udp_client = CreateClient(address);
            if (udp_client == null) return null;

            byte[] connection_response = SendRequest(udp_client, ConnectionRequest());
            if (connection_response == null || !ProcessConnectionResponse(connection_response))
            {
                LastError = "Wrong connection response from tracker";
                return null;
            }

            TorrentTrackerInfo tracker_info = new TorrentTrackerInfo(torrent, address);
            byte[] scrape_response = SendRequest(udp_client, ScrapeRequest(torrent));
            if (scrape_response == null || !ProcessScrapeResponse(scrape_response, tracker_info)) return null;

            byte[] announce_response = SendRequest(udp_client, AnnounceRequest(torrent));
            if (announce_response == null || !ProcessAnnounceResponse(announce_response, tracker_info)) return null;

            torrent.Trackers[address] = tracker_info;
            udp_client.Close();
            return tracker_info;
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
            AppendMessage(message_content, (Int64)TORRENT_PROTOCOL_CODE);
            AppendMessage(message_content, (Int32)ACTIONS.Connect);
            AppendMessage(message_content, (Int32)TransactionID);
            if (message_content.Count != 16) throw new FormatException("Wrong udp request");
            return message_content.ToArray();
        }

        /// <summary>
        /// Parse connection response and define connection id
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected bool ProcessConnectionResponse(byte[] response)
        {
            int action = BitConvertInt32(response, 0);
            int receiver_transaction_id = BitConvertInt32(response, 4);
            ConnectionID = BitConvertInt64(response, 8);
            return action == (int)ACTIONS.Connect && receiver_transaction_id == TransactionID;
        }

        /// <summary>
        /// Torrent protocol message for getting peers list
        /// </summary>
        /// <returns>Message content</returns>
        protected byte[] AnnounceRequest(Torrent torrent)
        {
            TransactionID = GenerateTransactionID();
            List<byte> message_content = new List<byte>();
            AppendMessage(message_content, (Int64)ConnectionID); //8
            AppendMessage(message_content, (Int32)ACTIONS.Announce); //4
            AppendMessage(message_content, (Int32)TransactionID); //4
            AppendMessage(message_content, torrent.InfoHash); // 20
            AppendMessage(message_content, Client.Id); // 20
            AppendMessage(message_content, (Int64)0); // downloaded
            AppendMessage(message_content, (Int64)0); // left
            AppendMessage(message_content, (Int64)0); // uploaded
            AppendMessage(message_content, (Int32)EVENTS.None); // event
            AppendMessage(message_content, (Int32)0); // IP (default)
            AppendMessage(message_content, (Int32)0); // key ???
            AppendMessage(message_content, (Int32)(-1)); // num_want (default)
            AppendMessage(message_content, (Int16)0); // port
            if (message_content.Count != 98) throw new FormatException("Wrong udp request");
            return message_content.ToArray();
        }

        /// <summary>
        /// Parse announce response and define peers
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected bool ProcessAnnounceResponse(byte[] response, TorrentTrackerInfo tracker_info)
        {
            int action = BitConvertInt32(response, 0);
            if (action == (int)ACTIONS.Error)
            {
                String error_msg = Encoding.ASCII.GetString(response.Skip(8).ToArray());
                LastError = error_msg;
                return false;
            }
            int receiver_transaction_id = BitConvertInt32(response, 4);
            tracker_info.Interval = BitConvertInt32(response, 8);
            tracker_info.Stats["Leechers"] = BitConvertInt32(response, 12);
            tracker_info.Stats["Seeders"] = BitConvertInt32(response, 16);

            int addresses_count = (response.Length - 20) / 6;
            String[] peers_addresses = new String[addresses_count];
            for (int i = 0; i < addresses_count; i++)
            {
                int ip_encoded = BitConvertInt32(response, 20 + i*6);
                String ip = String.Join(".", BitConverter.GetBytes(ip_encoded).Reverse().ToArray());
                int port = (ushort)BitConvertInt16(response, 24 + i*6);
                peers_addresses[i] = String.Format("{0}:{1}", ip, port);
            }
            tracker_info.PeersAddresses = peers_addresses.ToList();
            return action == (int)ACTIONS.Announce && receiver_transaction_id == TransactionID;
        }
        
        /// <summary>
        /// Torrent protocol message for getting peers count
        /// </summary>
        /// <returns>Message content</returns>
        protected byte[] ScrapeRequest(Torrent torrent)
        {
            TransactionID = GenerateTransactionID();
            List<byte> message_content = new List<byte>();
            AppendMessage(message_content, (Int64)ConnectionID); //8
            AppendMessage(message_content, (Int32)ACTIONS.Scrape); //4
            AppendMessage(message_content, (Int32)TransactionID); //4
            AppendMessage(message_content, torrent.InfoHash); // 20
            if (message_content.Count != 36) throw new FormatException("Wrong udp request");
            return message_content.ToArray();
        }
        
        /// <summary>
        /// Parse scrape response and define peers count
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected bool ProcessScrapeResponse(byte[] response, TorrentTrackerInfo tracker_info)
        {
            int action = BitConvertInt32(response, 0);
            if (action == (int)ACTIONS.Error)
            {
                String error_msg = Encoding.ASCII.GetString(response.Skip(8).ToArray());
                LastError = error_msg;
                return false;
            }
            int receiver_transaction_id = BitConvertInt32(response, 4);
            tracker_info.Stats["Complete"] = BitConvertInt32(response, 8);
            tracker_info.Stats["Downloaded"] = BitConvertInt32(response, 12);
            tracker_info.Stats["Incomplete"] = BitConvertInt32(response, 16);
            return action == (int)ACTIONS.Scrape && receiver_transaction_id == TransactionID;
        }

        /// <summary>
        /// Identifies response for request
        /// </summary>
        /// <returns>ID</returns>
        protected int GenerateTransactionID()
        {
            return (Int32)(new Random(3).Next());
        }

        //
        // Genesis and parsing of udp message (big endian)
        //

        protected Int16 BitConvertInt16(byte[] data, int start_index)
        {
            int bytes = 16 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt16(ordered_data, 0);
        }
        protected Int32 BitConvertInt32(byte[] data, int start_index)
        {
            int bytes = 32 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt32(ordered_data, 0);
        }
        protected Int64 BitConvertInt64(byte[] data, int start_index)
        {
            int bytes = 64 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt64(ordered_data, 0);
        }

        protected void AppendMessage(List<byte> msg, Int16 value)
        {
            msg.AddRange(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        protected void AppendMessage(List<byte> msg, Int32 value)
        {
            msg.AddRange(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        protected void AppendMessage(List<byte> msg, Int64 value)
        {
            msg.AddRange(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        protected void AppendMessage(List<byte> msg, byte[] value)
        {
            msg.AddRange(value.Reverse().ToArray());
        }
        
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
