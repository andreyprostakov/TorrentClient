using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitTorrentProtocol
{
    /// <summary>
    /// Class generally duplicates BitConverter functions using BigEndian instead of LittleEndian
    /// </summary>
    public static class BigEndian
    {
        public static Int16 GetInt16(byte[] data, int start_index)
        {
            int bytes = 16 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt16(ordered_data, 0);
        }
        public static Int32 GetInt32(byte[] data, int start_index)
        {
            int bytes = 32 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt32(ordered_data, 0);
        }
        public static Int64 GetInt64(byte[] data, int start_index)
        {
            int bytes = 64 / 8;
            byte[] ordered_data = data.Skip(start_index).Take(bytes).Reverse().ToArray();
            return BitConverter.ToInt64(ordered_data, 0);
        }

        public static byte[] GetBytes(Int16 value)
        {         
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        public static byte[] GetBytes(Int32 value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        public static byte[] GetBytes(Int64 value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        public static byte[] GetBytes(byte[] value)
        {
            return value.Reverse().ToArray();
        }
    }
}
