using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class PeerConnector
    {
        public IPAddress IP { get; private set; }
        public int Port { get; private set; }
        public String LastError { get; private set; }
        private Torrent torrent;

        public PeerConnector(String address)
        {
            if (!address.Contains(":")) throw new FormatException();
            String[] address_parts = address.Split(':');
            IP = IPAddress.Parse(address_parts[0]);
            Port = Int32.Parse(address_parts[1]);
            return;
        }

        public bool Connect()
        {
            IPEndPoint endpoint = new IPEndPoint(IP, Port);
            TcpClient tcp_client = new TcpClient();
            try
            {
                tcp_client.Connect(endpoint);
                return true;
            }
            catch (SocketException ex)
            {
                LastError = ex.Message;
                return false;
            }
            finally
            {
                tcp_client.Close();
            }
        }
    }
}
