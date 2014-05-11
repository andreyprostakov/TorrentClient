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
        public String LastError { get; private set; }
        protected int TransactionID;
        protected long ConnectionID;
        private Client Client;

        public enum ACTIONS: int {Connect = 0, Announce = 1, Scrape = 2, Error = 3}
        public enum EVENTS : int {None = 0, Completed = 1, Started = 2, Stopped = 3}
        protected String UDP_ADDRESS_PATTERN = @"udp://(.+):(\d{1,4})";
        private const int SEND_TIMEOUT = 1000;
        private const int RECEIVE_TIMEOUT = 3000;
        private const int CONNECTION_RETRIES = 3;


        public TrackerUdpProtocol(Client client)
        {
            this.Client = client;
            return;
        }

        public bool ConnectUDP(TorrentTrackerInfo tracker_info)
        {
            String address = tracker_info.AnnounceUrl;
            Torrent torrent = tracker_info.Torrent;
            if (!Regex.IsMatch(address, UDP_ADDRESS_PATTERN))
            {
                tracker_info.Status = "Unknown address format";
                return false;
            }
            try
            {
                UdpClient udp_client = CreateClient(address);

                byte[] msg = TrackerUdpMessages.Connection(GenerateTransactionID());
                byte[] connection_response = SendRequest(udp_client, msg);
                ProcessConnectionResponse(connection_response);

                msg = TrackerUdpMessages.Scrape(GenerateTransactionID(), ConnectionID, torrent.InfoHash);
                byte[] scrape_response = SendRequest(udp_client, msg, true);
                ProcessScrapeResponse(scrape_response, tracker_info);

                msg = TrackerUdpMessages.Announce(GenerateTransactionID(), ConnectionID, Client.Id, torrent.InfoHash);
                byte[] announce_response = SendRequest(udp_client, msg, true);
                ProcessAnnounceResponse(announce_response, tracker_info);

                udp_client.Close();
                tracker_info.Status = "Ok";
                return true;
            }
            catch (Exception ex)
            {
                if ((ex is SocketException) || (ex is FormatException) || (ex is WebException))
                {
                    tracker_info.Status = ex.Message;
                    return false;
                }
                throw ex;
            }
        }

        /// <summary>
        /// Send datagram using configured udp client
        /// </summary>
        /// <returns>received data or null</returns>
        protected byte[] SendRequest(UdpClient udp_client, byte[] request_data, bool reconnect_on_error = false)
        {
            for (int i = 0; ; i++)
            {
                byte[] response = null;
                try
                {
                    udp_client.Send(request_data, request_data.Length);
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                    response = udp_client.Receive(ref endpoint);
                }
                catch (SocketException ex)
                {
                    if (i < CONNECTION_RETRIES)
                    {
                        if (reconnect_on_error)
                        {
                            byte[] connection_response = SendRequest(udp_client, TrackerUdpMessages.Connection(GenerateTransactionID()));
                            ProcessConnectionResponse(connection_response);
                        }
                        continue;
                    }
                    else throw ex;
                }
                return response;
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
            udp_client = new UdpClient(0, AddressFamily.InterNetwork);
            udp_client.Connect(host_name, target_port);
            udp_client.Client.SendTimeout = SEND_TIMEOUT;
            udp_client.Client.ReceiveTimeout = RECEIVE_TIMEOUT;
            return udp_client;
        }

        /// <summary>
        /// Parse target address to define host name and port
        /// </summary>
        /// <returns>true if parsing was successful</returns>
        protected void ParseTrackerAddress(String address, out String host_name, out int port)
        {
            var parse_result = System.Text.RegularExpressions.Regex.Match(address, UDP_ADDRESS_PATTERN);
            host_name = parse_result.Groups[1].Value;
            bool correct_port = Int32.TryParse(parse_result.Groups[2].Value, out port);
            if (String.IsNullOrEmpty(host_name) || !correct_port) throw new FormatException("Wrong address format");
            return;
        }

        /// <summary>
        /// Parse connection response and define connection id
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected void ProcessConnectionResponse(byte[] response)
        {
            int action = BitConvertInt32(response, 0);
            int receiver_transaction_id = BitConvertInt32(response, 4);
            ConnectionID = BitConvertInt64(response, 8);
            if (action != (int)ACTIONS.Connect || receiver_transaction_id != TransactionID) throw new WebException("Wrong connection response");
            return;
        }

        /// <summary>
        /// Parse announce response and define peers
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected void ProcessAnnounceResponse(byte[] response, TorrentTrackerInfo tracker_info)
        {
            int action = BitConvertInt32(response, 0);
            if (action == (int)ACTIONS.Error) throw new WebException(Encoding.ASCII.GetString(response.Skip(8).ToArray()));
            int receiver_transaction_id = BitConvertInt32(response, 4);
            tracker_info.Interval = BitConvertInt32(response, 8);
            tracker_info.Stats["Leechers"] = BitConvertInt32(response, 12);
            tracker_info.Stats["Seeders"] = BitConvertInt32(response, 16);
            /*Reading announce addresses*/
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
            if (action != (int)ACTIONS.Announce || receiver_transaction_id != TransactionID) throw new WebException("Wrong connection response");
        }
        
        /// <summary>
        /// Parse scrape response and define peers count
        /// </summary>
        /// <returns>true if response is correct</returns>
        protected void ProcessScrapeResponse(byte[] response, TorrentTrackerInfo tracker_info)
        {
            int action = BitConvertInt32(response, 0);
            if (action == (int)ACTIONS.Error) throw new WebException(Encoding.ASCII.GetString(response.Skip(8).ToArray()));
            int receiver_transaction_id = BitConvertInt32(response, 4);
            tracker_info.Stats["Complete"] = BitConvertInt32(response, 8);
            tracker_info.Stats["Downloaded"] = BitConvertInt32(response, 12);
            tracker_info.Stats["Incomplete"] = BitConvertInt32(response, 16);
            if (action != (int)ACTIONS.Scrape || receiver_transaction_id != TransactionID) throw new WebException("Wrong connection response");
            return;
        }

        /// <summary>
        /// Identifies response for request
        /// </summary>
        /// <returns>ID</returns>
        private int GenerateTransactionID()
        {
            TransactionID = (Int32)(new Random(3).Next());
            return TransactionID;
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

    }
}
