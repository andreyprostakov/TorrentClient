using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class BitField
    {
        public Int64 PiecesCount { get; private set; }
        public Int32 Length 
        {
            get
            {
                return bitfield.Length;
            }
        }
        private byte[] bitfield;

        public BitField(int pieces)
        {
            PiecesCount = pieces;
            bitfield = new byte[pieces / 8 + 1];
            return;
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= PiecesCount) throw new ArgumentException("Index out of bitfield");
                int index_byte = index / 8;
                int index_bit = index % 8;
                byte mask = (byte)(128 >> index_bit);
                return (bitfield[index_byte] & mask) != 0;
            }
        }

        public void Set(int index)
        {
            if (index < 0 || index >= PiecesCount) throw new ArgumentException("Index out of bitfield");
            int index_byte = index / 8;
            int index_bit = index % 8;
            byte mask = (byte)(128 >> index_bit);
            bitfield[index_byte] |= mask;
            return;
        }

        public void Sum(byte[] bitfield)
        {
            if (bitfield.Length != this.Length) throw new ArgumentException("Bitfields format mismatch");
            for (int i = 0; i < this.Length; i++)
                this.bitfield[i] |= bitfield[i];
            return;
        }

        public int[] MissingPieces()
        {
            List<int> missing = new List<int>();
            int piece_index = 0;
            for (int byte_index = 0; byte_index < this.Length; byte_index++)
            {
                for (int bit_index = 0; (bit_index < 8) && (piece_index < PiecesCount); bit_index++, piece_index++)
                {
                    byte mask = (byte)(128 >> bit_index);
                    if ((this.bitfield[byte_index] & mask) == 0)
                        missing.Add(piece_index);
                }
            }
            return missing.ToArray();
        }

        public int[] RequiredPieces(BitField available_bitfield)
        {
            List<int> required = new List<int>();
            int piece_index = 0;
            for (int byte_index = 0; byte_index < this.Length; byte_index++)
            {
                for (int bit_index = 0; (bit_index < 8) && (piece_index < PiecesCount); bit_index++, piece_index++)
                {
                    byte mask = (byte)(128 >> bit_index);
                    if ((this.bitfield[byte_index] & mask) == 0 && (available_bitfield.bitfield[byte_index] & mask) != 0)
                        required.Add(piece_index);
                }
            }
            return required.ToArray();
        }

        public bool SaveTo(String file_name)
        {
            using (FileStream writer = File.Open(file_name, FileMode.Create))
            {
                try
                {
                    writer.Write(this.bitfield, 0, this.Length);
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
