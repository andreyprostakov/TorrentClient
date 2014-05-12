using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitTorrentProtocol.Peer
{
    public static class PeerTcpMessages
    {
        public enum ACTIONS : byte { Choke = 0, Unchoke, Interested = 2, NotInterested, Have = 4, Bitfield, Request = 6, Piece, Cancel = 8, Port };
        private static String PROTOCOL_NAME = "BitTorrent protocol";

        /// <summary>
        /// Handshake is required to start a p2p dialog
        /// </summary>
        /// <param name="info_hash">torrent 'info' section hash</param>
        /// <param name="peer_id">our peer id</param>
        public static byte[] HandShake(byte[] info_hash, byte[] peer_id)
        {
            List<byte> msg = new List<byte>();
            msg.Add((byte)PROTOCOL_NAME.Length);
            msg.AddRange(Encoding.UTF8.GetBytes(PROTOCOL_NAME));
            msg.AddRange(new byte[8]);
            msg.AddRange(BigEndian.GetBytes(info_hash));
            msg.AddRange(BigEndian.GetBytes(peer_id));
            if (msg.Count != 68) throw new FormatException("Wrong handshake message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling our companion that we're still here.
        /// Required in timeouts more than 2 mins.
        /// </summary>
        public static byte[] KeepAlive()
        {
            return BigEndian.GetBytes((Int32)0);
        }

        /// <summary>
        /// Telling 'We won't share anything right now'
        /// </summary>
        public static byte[] Choke()
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)1));
            msg.Add((byte)ACTIONS.Choke);
            if (msg.Count != 1 + 4) throw new FormatException("Wrong choke message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'We decided to share now'
        /// </summary>
        public static byte[] Unchoke()
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)1));
            msg.Add((byte)ACTIONS.Unchoke);
            if (msg.Count != 1 + 4) throw new FormatException("Wrong unchoke message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'We want to download some data'
        /// </summary>
        public static byte[] Interested()
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)1));
            msg.Add((byte)ACTIONS.Interested);
            if (msg.Count != 1 + 4) throw new FormatException("Wrong interested message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'We don't need any data'
        /// </summary>
        /// <returns></returns>
        public static byte[] NotInterested()
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)1));
            msg.Add((byte)ACTIONS.NotInterested);
            if (msg.Count != 1 + 4) throw new FormatException("Wrong interested message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'I have a piece of the file'
        /// </summary>
        /// <param name="piece_index">piece index</param>
        public static byte[] Have(Int32 piece_index)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)5));
            msg.Add((byte)ACTIONS.Have);
            msg.AddRange(BigEndian.GetBytes(piece_index));
            if (msg.Count != 5 + 4) throw new FormatException("Wrong have message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'Here's map of all pieces I have'
        /// </summary>
        /// <param name="field">bitmap</param>
        public static byte[] Bitfield(byte[] field)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)(1 + field.Length)));
            msg.Add((byte)ACTIONS.Bitfield);
            msg.AddRange(BigEndian.GetBytes(field));
            if (msg.Count != 1 + field.Length + 4) throw new FormatException("Wrong bitfield message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Request for downloading specific data
        /// </summary>
        /// <param name="index">piece index</param>
        /// <param name="block_offset">offset in piece</param>
        /// <param name="block_length">length of block to download</param>
        public static byte[] Request(Int32 index, Int32 block_offset, Int32 block_length)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)13));
            msg.Add((byte)ACTIONS.Request);
            msg.AddRange(BigEndian.GetBytes(index));
            msg.AddRange(BigEndian.GetBytes(block_offset));
            msg.AddRange(BigEndian.GetBytes(block_length));
            if (msg.Count != 13 + 4) throw new FormatException("Wrong request message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Sending requested data
        /// </summary>
        /// <param name="index">piece index</param>
        /// <param name="block_offset">offset in piece</param>
        /// <param name="block">data</param>
        public static byte[] Piece(Int32 index, Int32 block_offset, byte[] block)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)9 + block.Length));
            msg.Add((byte)ACTIONS.Piece);
            msg.AddRange(BigEndian.GetBytes(index));
            msg.AddRange(BigEndian.GetBytes(block_offset));
            msg.AddRange(BigEndian.GetBytes(block));
            if (msg.Count != 9 + block.Length + 4) throw new FormatException("Wrong piece message format");
            return msg.ToArray();
        }

        /// <summary>
        /// Telling 'We don't need that data we requested before'
        /// </summary>
        /// <param name="index">piece index</param>
        /// <param name="block_offset">offset in piece</param>
        /// <param name="block_length">data length</param>
        public static byte[] Cancel(Int32 index, Int32 block_offset, Int32 block_length)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BigEndian.GetBytes((Int32)13));
            msg.Add((byte)ACTIONS.Cancel);
            msg.AddRange(BigEndian.GetBytes(index));
            msg.AddRange(BigEndian.GetBytes(block_offset));
            msg.AddRange(BigEndian.GetBytes(block_length));
            if (msg.Count != 13 + 4) throw new FormatException("Wrong cancel message format");
            return msg.ToArray();
        }
    }
}
