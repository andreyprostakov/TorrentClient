using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class Client
    {
        public byte[] Id { get; private set; }
        public IPAddress ip_listen;
        public int port_listen;

        public static int FILE_CHUNK_SIZE = 16384;

        public Client()
        {
            Id = GenerateId();
            ip_listen = IPAddress.Any;
            port_listen = DefinePort();
            return;
        }

        public bool ConnectAnnouncer(TorrentTrackerInfo tracker_info)
        {
            String address = tracker_info.AnnounceUrl;
            if (String.IsNullOrEmpty(tracker_info.AnnounceUrl)) throw new FormatException("Wrong address");
            if (address.Contains("http://") || address.Contains("https://"))
            {
                TrackerHttpProtocol tracker = new TrackerHttpProtocol(this);
                return tracker.Connect(tracker_info.Torrent, address);
            }
            else if (address.Contains("udp://"))
            {
                TrackerUdpProtocol tracker = new TrackerUdpProtocol(this);
                return tracker.ConnectUDP(tracker_info);
            }
            else
            {
                tracker_info.Status = "Wrong announce address given";
                return false;
            }
        }

        /// <summary>
        /// Update torrent peers info
        /// </summary>
        public void CollectTorrentPeers(Torrent torrent)
        {
            List<String> peers_addresses = new List<String>();
            foreach (var announcer_info in torrent.Announcers.Values)
            {
                if (ConnectAnnouncer(announcer_info)) peers_addresses.AddRange(announcer_info.PeersAddresses);
            }
            peers_addresses.Sort();
            torrent.PeersAddresses = peers_addresses.Distinct().ToArray();
            return;
        }

        /// <summary>
        /// Generate self peer_id as random
        /// </summary>
        /// <returns>raw peer id</returns>
        private byte[] GenerateId()
        {
            long time = DateTime.UtcNow.ToBinary();
            Random rd = new Random((int)time);
            byte[] random_array = new byte[32];
            rd.NextBytes(random_array);
            SHA1 cryptographer = SHA1.Create();
            return cryptographer.ComputeHash(random_array);
        }
        
        /// <summary>
        /// Check if port is free
        /// </summary>
        private bool PortIsVacant(int port_number)
        {
            var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            return !connections.Any(c => c.Port == port_number);
        }

        /// <summary>
        /// Find a vacant port
        /// </summary>
        private int DefinePort()
        {
            int port = 6900;
            while (!PortIsVacant(port)) port++;
            return port;
        }
    }
}
