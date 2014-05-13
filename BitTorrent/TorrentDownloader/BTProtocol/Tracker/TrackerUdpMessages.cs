using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public static class TrackerUdpMessages
    {
        public const long TORRENT_PROTOCOL_CODE = 0x41727101980;
        public enum ACTIONS : int { Connect = 0, Announce = 1, Scrape = 2, Error = 3 }
        public enum EVENTS : int { None = 0, Completed = 1, Started = 2, Stopped = 3 }

        /// <summary>
        /// Message to establish a UDP 'connection' with server.
        /// </summary>
        /// <param name="transaction_id">random 20 B</param>
        /// <returns>raw message content</returns>
        public static byte[] Connection(Int32 transaction_id)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int64)TORRENT_PROTOCOL_CODE));
            msg.AddRange(BigEndian.GetBytes((Int32)ACTIONS.Connect));
            msg.AddRange(BigEndian.GetBytes((Int32)transaction_id));
            if (msg.Count != 16) throw new FormatException("Wrong udp request");
            return msg.ToArray();
        }

        /// <summary>
        /// Message to get and send data specific for user
        /// </summary>
        /// <param name="transaction_id">random 20 B</param>
        /// <param name="connection_id">20 B received in connection response</param>
        /// <param name="client_id">our peer id</param>
        /// <returns>raw message content</returns>
        public static byte[] Announce(Int32 transaction_id, Int64 connection_id, byte[] client_id, byte[] info_hash)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int64)connection_id)); //8
            msg.AddRange(BigEndian.GetBytes((Int32)ACTIONS.Announce)); //4
            msg.AddRange(BigEndian.GetBytes((Int32)transaction_id)); //4
            msg.AddRange(BigEndian.GetBytes(info_hash)); // 20
            msg.AddRange(BigEndian.GetBytes(client_id)); // 20
            msg.AddRange(BigEndian.GetBytes((Int64)0)); // downloaded
            msg.AddRange(BigEndian.GetBytes((Int64)0)); // left
            msg.AddRange(BigEndian.GetBytes((Int64)0)); // uploaded
            msg.AddRange(BigEndian.GetBytes((Int32)EVENTS.None)); // event
            msg.AddRange(BigEndian.GetBytes((Int32)0)); // IP (default)
            msg.AddRange(BigEndian.GetBytes((Int32)0)); // key ???
            msg.AddRange(BigEndian.GetBytes((Int32)(-1))); // num_want (default)
            msg.AddRange(BigEndian.GetBytes((Int16)0)); // port
            if (msg.Count != 98) throw new FormatException("Wrong udp request");
            return msg.ToArray();
        }

        /// <summary>
        /// Message to get general stats about torrent
        /// </summary>
        /// <param name="transaction_id">random 20 B</param>
        /// <param name="connection_id">20 B received in connection response</param>
        /// <returns>raw message content</returns>
        public static byte[] Scrape(Int32 transaction_id, Int64 connection_id, byte[] info_hash)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int64)connection_id)); //8
            msg.AddRange(BigEndian.GetBytes((Int32)ACTIONS.Scrape)); //4
            msg.AddRange(BigEndian.GetBytes((Int32)transaction_id)); //4
            msg.AddRange(BigEndian.GetBytes(info_hash)); // 20
            if (msg.Count != 36) throw new FormatException("Wrong udp request");
            return msg.ToArray();
        }

        /// <summary>
        /// Identifies response for request
        /// </summary>
        /// <returns>ID</returns>
        private static int GenerateTransactionID()
        {
            return (Int32)(new Random(3).Next());
        }
    }
}
