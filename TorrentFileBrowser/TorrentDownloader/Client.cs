using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        public int port_listen;
        public Threading.Pool Pool { get; private set; }

        public Client()
        {
            Id = GenerateId();
            port_listen = DefinePort();
            return;
        }
        
        //
        // Connecting to trackers
        //

        /// <summary>
        /// Update torrent peers info
        /// </summary>
        public void CollectTorrentPeers(Torrent torrent)
        {
            ConnectToTrackers(torrent.Announcers.Values.ToArray());
            List<String> peers_addresses = new List<String>();
            foreach (var announcer_info in torrent.Announcers.Values)
            {
                peers_addresses.AddRange(announcer_info.PeersAddresses);
            }
            peers_addresses.Sort();
            torrent.PeersAddresses = peers_addresses.Distinct().ToArray();
            return;
        }

        /// <summary>
        /// Question all trackers and wait for responses
        /// </summary>
        /// <param name="trackers"></param>
        protected void ConnectToTrackers(TorrentTrackerInfo[] trackers)
        {
            Pool = new Threading.Pool(Math.Min(trackers.Length, 6));
            foreach (var announcer_info in trackers)
            {
                Pool.AddRoutine(ConnectToTrackerRoutine, (Object)announcer_info);
            }
            Pool.WaitForEveryone();
            return;
        }

        /// <summary>
        /// Worker routine connecting to tracker by address
        /// </summary>
        /// <param name="parameter">Server host address</param>
        protected void ConnectToTrackerRoutine(Object parameter)
        {
            TorrentTrackerInfo tracker_info = (TorrentTrackerInfo)parameter;
            String address = tracker_info.AnnounceUrl;
            if (String.IsNullOrEmpty(tracker_info.AnnounceUrl)) throw new FormatException("Wrong address");
            if (address.Contains("http://") || address.Contains("https://"))
            {
                TrackerHttpProtocol tracker = new TrackerHttpProtocol(this.Id, this.port_listen);
                tracker.Connect(tracker_info);
            }
            else if (address.Contains("udp://"))
            {
                TrackerUdpProtocol tracker = new TrackerUdpProtocol(this.Id);
                tracker.ConnectUDP(tracker_info);
            }
            else
            {
                tracker_info.Status = "Wrong announce address given";
            }
            return;
        }

        //
        // Connecting to other peers
        //

        /// <summary>
        /// Initiate downloading routine and return
        /// </summary>
        /// <returns>true if downloading is acceptable</returns>
        public bool StartDownloading(Torrent torrent)
        {
            if (torrent.PrepareForDownload())
            {
                Pool = new Threading.Pool(Math.Min(torrent.Announcers.Count, 7));
                Pool.AddRoutine(DownloadingCycle, torrent);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Managing download connection tasks to keep the process non-stop
        /// </summary>
        protected void DownloadingCycle(Object parameter)
        {
            Torrent torrent = (Torrent)parameter;
            for (int i = 0; i < torrent.PeersAddresses.Length; i = (i + 1) % torrent.PeersAddresses.Length)
            {
                if (Pool.TasksInQueue() < torrent.PeersAddresses.Length / 2)
                {
                    Pool.AddRoutine(StartDownloadingRoutine, new Object[] { torrent, torrent.PeersAddresses[i] });
                }
            }
            return;
        }

        /// <summary>
        /// Worker routine connecting to peer
        /// </summary>
        /// <param name="parameter">array containing a torrent and peer address</param>
        protected void StartDownloadingRoutine(Object parameter)
        {
            Object[] parameters = (Object[])parameter;
            Torrent torrent = (Torrent)parameters[0];
            String peer_address = (String)parameters[1];
            PeerTcpProtocol peer = new PeerTcpProtocol(this.Id);
            peer.Connect(torrent, peer_address);
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
