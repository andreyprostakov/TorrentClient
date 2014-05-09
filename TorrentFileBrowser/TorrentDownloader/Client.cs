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

        private TcpListener listener;
        private Thread listen_thread;
        private MainForm form;

        public Client(MainForm form)
        {
            this.form = form;
            Id = GenerateId();
            ip_listen = IPAddress.Any;
            port_listen = DefinePort();
            listener = new TcpListener(ip_listen, port_listen);
            listen_thread = new Thread(new ThreadStart(listener.Start));
            listen_thread.Start();
            return;
        }

        public TorrentTrackerInfo ConnectAnnouncer(Torrent torrent, String address, out String msg)
        {
            if (torrent == null || String.IsNullOrEmpty(address)) throw new ArgumentNullException();
            if (address.Contains("http://") || address.Contains("https://"))
            {
                TrackerHttpProtocol tracker = new TrackerHttpProtocol(this);
                tracker.Connect(torrent, address, out msg);
                return null;
            }
            else if (address.Contains("udp://"))
            {
                TrackerUdpProtocol tracker = new TrackerUdpProtocol(this);
                TorrentTrackerInfo tracker_info = tracker.Connect(address, torrent);
                if (tracker_info == null) msg = tracker.LastError;
                else msg = "";
                return tracker_info;
            }
            else
            {
                throw new FormatException("Wrong announce address given");
            }
        }

        private byte[] GenerateId()
        {
            long time = DateTime.UtcNow.ToBinary();
            Random rd = new Random((int)time);
            byte[] random_array = new byte[32];
            rd.NextBytes(random_array);
            SHA1 cryptographer = SHA1.Create();
            return cryptographer.ComputeHash(random_array);
        }

        private void ListenForPeers()
        {
            listener.Start();
            while (true)
            {
                TcpClient peer = listener.AcceptTcpClient();
                Thread handle_thread = new Thread(new ParameterizedThreadStart(HandleClient));
                handle_thread.Start();
            }
            return;
        }

        private void HandleClient(object _client)
        {
            form.Close();
            TcpClient client = (TcpClient)_client;
            NetworkStream reader = client.GetStream();
            byte[] msg = new byte[256];
            int bytes_read;
            while (true)
            {
                bytes_read = 0;
                try
                {
                    bytes_read = reader.Read(msg, 0, 256);
                }
                catch { break; }
            }
            ASCIIEncoding encoder = new ASCIIEncoding();
            System.Diagnostics.Debug.WriteLine(encoder.GetString(msg, 0, bytes_read));
            return;
        }

        private bool PortIsVacant(int port_number)
        {
            var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            return !connections.Any(c => c.Port == port_number);
        }

        private int DefinePort()
        {
            int port = 6900;
            while (!PortIsVacant(port)) port++;
            return port;
        }
    }
}
