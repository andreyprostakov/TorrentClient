using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public class BEncodedDecoder
    {
        protected StreamReader reader;

        public IBElement Decode(Stream bencoded_reader)
        {
            reader = new StreamReader(bencoded_reader, Encoding.ASCII);
            BList list = null;
            try
            {
                list = ReadList();
            }
            finally
            {
                reader.Close();
                bencoded_reader.Close();
            }
            if (list.Count > 1)
                return list;
            else if (list.Count == 1)
                return list.First;
            else 
                throw new FormatException();
        }

        protected IBElement ReadElement()
        {
            char cur_char = (char)reader.Read();
            if (cur_char == 'e') return null;
            switch (cur_char)
            {
                case 'i': 
                    return ReadNumber();
                case 'l': 
                    return ReadList();
                case 'd': 
                    return ReadDictionary();
                default: 
                    if (cur_char < '0' || cur_char > '9') 
                        throw new FormatException();
                    long length = ReadTextLength(cur_char);
                    return ReadText(length);
            }
        }

        protected long ReadTextLength(char first_char)
        {
            String buffer = "" + first_char;
            char cur_char = (char)reader.Read();
            while (cur_char != ':')
            {
                buffer += cur_char;
                cur_char = (char)reader.Read();
            }
            long length;
            if (!Int64.TryParse(buffer, out length)) 
                throw new FormatException();
            return length;
        }

        protected BNumber ReadNumber()
        {
            String buffer = "";
            char cur_char = (char)reader.Read();
            while (cur_char != 'e')
            {
                buffer += cur_char;
                cur_char = (char)reader.Read();
            }
            long value;
            if (!Int64.TryParse(buffer, out value))
                throw new FormatException();
            return new BNumber(value);
        }

        protected BDictionary ReadDictionary()
        {
            BList list = ReadList();
            List<IBElement> values = new List<IBElement>();
            List<BText> keys = new List<BText>();
            for (int i = 0; i + 1 < list.Count; i+=2)
            {
                if (!(list[i] is BText)) 
                    throw new FormatException();
                keys.Add((BText)list[i]);
                values.Add(list[i+1]);
            }
            return new BDictionary(keys, values);
        }

        protected BList ReadList()
        {
            List<IBElement> list = new List<IBElement>();
            do
            {
                IBElement cur_element = ReadElement();
                if (cur_element != null)
                    list.Add(cur_element);
                else break;
            } while (!reader.EndOfStream);
            return new BList(list);
        }

        protected BText ReadText(long length)
        {
            String buffer = "";
            for (long i = 0; i < length; i++)
            {
                if (reader.EndOfStream) 
                    throw new FormatException();
                buffer += (char)reader.Read();
            }
            return new BText(buffer);
        }
    }
}
