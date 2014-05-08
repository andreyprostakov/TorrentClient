using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OSS.NBEncode.Transforms;
using OSS.NBEncode.Entities;

namespace Torrent_client
{
    public class TorrentFile
    {
        public String Filename { get; protected set; }

        public TorrentFile(String filename)
        {
            this.Filename = filename;
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }
            return;
        }

        public String Content()
        {
            StreamReader reader = new StreamReader(Filename);
            String content =  reader.ReadToEnd();
            reader.Close();
            return content;
        }

        public String ParsedContent()
        {
            var parser = new BObjectTransform();
            StreamReader reader = new StreamReader(Filename);
            IBObject bobj = parser.DecodeNext(reader.BaseStream);
            reader.Close();
            return WriteObject(0, bobj);
        }

        public String WriteObject(int indentLevel, IBObject obj)
        {
            switch (obj.BType)
            {
                case BObjectType.Integer:
                    return WriteInteger(indentLevel, (BInteger)obj);
                case BObjectType.ByteString:
                    return WriteByteString(indentLevel, (BByteString)obj);
                case BObjectType.List:
                    return WriteList(indentLevel, (BList)obj);
                case BObjectType.Dictionary:
                    return WriteDictionary(indentLevel, (BDictionary)obj);
                default:
                    return null;
            }
        }


        private String WriteInteger(int indentLevel, BInteger integer)
        {

            return String.Format("{0}{1}\n", GetIndentSpaces(indentLevel), integer.Value.ToString());
        }


        private String WriteByteString(int indentLevel, BByteString byteString)
        {
            return String.Format("{0}{1}\n", GetIndentSpaces(indentLevel), byteString.ConvertToText(Encoding.ASCII));
        }


        private String WriteList(int indentLevel, BList list)
        {
            String result = String.Format("{0}List:\n", GetIndentSpaces(indentLevel));
            foreach (IBObject obj in list.Value)
            {
                result += WriteObject(indentLevel + 1, obj);
            }
            return result;
        }


        private String WriteDictionary(int indentLevel, BDictionary dict)
        {
            String result = String.Format("{0}Dict:\n", GetIndentSpaces(indentLevel));
            foreach (var kvPair in dict.Value)
            {
                result += WriteByteString(indentLevel + 1, kvPair.Key);
                result += WriteObject(indentLevel + 2, kvPair.Value);
            }
            return result;
        }


        private string GetIndentSpaces(int indentLevel)
        {
            return new string(' ', indentLevel * 2);
        }

    }
}
