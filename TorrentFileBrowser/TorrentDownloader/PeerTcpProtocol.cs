using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class PeerTcpProtocol
    {
        public String LastError { get; private set; }
        private Client client;
        private TcpClient tcp_client;
        private byte[] partner_id;
        private Torrent torrent;

        public static String PROTOCOL_ID = "BitTorrent protocol";


        public PeerTcpProtocol(Client client)
        {
            this.client = client;
            return;
        }

        /// <summary>
        /// Download torrent file from specified peer
        /// </summary>
        /// <returns>true if succeeded</returns>
        public bool Connect(Torrent torrent, String peer_address)
        {
            this.torrent = torrent;
            try
            {
                CreateSocket(peer_address);
                Handshake();
                return true;
            }
            catch (Exception ex)
            {
                if ((ex is SocketException) || (ex is WebException) || (ex is FormatException))
                {
                    LastError = ex.Message;
                    return false;
                } else throw ex;
            }
            finally
            {
                if (tcp_client != null) tcp_client.Close();
            }
        }

        /// <summary>
        /// Perform handshake with specified peer
        /// </summary>
        /// <returns>true if succeeded</returns>
        public void Handshake()
        {
            var stream = tcp_client.GetStream();
            byte[] message = HandshakeMessage(torrent);
            stream.Write(message, 0, message.Length);
            byte[] buffer = new byte[128];
            int result = stream.Read(buffer, 0, buffer.Length);
            ParseHandshakeResponse(buffer, result);
            return;
        }


        /// <summary>
        /// Create tcp socket for communication with peer
        /// </summary>
        /// <returns>true if succeeded</returns>
        protected void CreateSocket(String address)
        {
            String[] address_parts = address.Split(':');
            int port = Int32.Parse(address_parts[1]);
            String ip = address_parts[0];
            if (String.IsNullOrEmpty(ip)) throw new FormatException("Wrong peer address format");
            tcp_client = new TcpClient(ip, port);
            return;
        }

        /// <summary>
        /// Generate message for handshake protocol
        /// </summary>
        /// <returns>raw message</returns>
        protected byte[] HandshakeMessage(Torrent torrent)
        {
            List<byte> message = new List<byte>();
            message.Add((byte)PROTOCOL_ID.Length);
            message.AddRange(Encoding.UTF8.GetBytes(PROTOCOL_ID));
            message.AddRange(new byte[8]);
            message.AddRange(torrent.InfoHash.Reverse().ToArray());
            message.AddRange(client.Id);
            if (message.Count != 68) throw new FormatException("Wrong handshake message format");
            return message.ToArray();
        }

        /// <summary>
        /// Parse peer response of handshake
        /// </summary>
        /// <returns>true if valid</returns>
        protected void ParseHandshakeResponse(byte[] response, int length)
        {
            if (length < 68) throw new FormatException("Wrong peer response format");            
            int pstrlen = response[0];
            String protocol = Encoding.UTF8.GetString(response, 1, pstrlen);
            byte[] info_hash = response.Skip(28).Take(20).Reverse().ToArray();
            partner_id = response.Skip(48).Take(20).ToArray();
            if (!(pstrlen == 19) && protocol.Equals(PROTOCOL_ID) && EqualBytes(info_hash, torrent.InfoHash))
                throw new FormatException("Wrong peer response format");
            return;
        }


        private bool EqualBytes(byte[] data1, byte[] data2)
        {
            if (data1.Length != data2.Length) return false;
            for (int i = 0; i < data1.Length;i++)
            {
                if (data1[i] != data2[i]) return false;
            }
            return true;
        }
    }
}
