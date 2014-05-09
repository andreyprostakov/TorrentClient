using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BDecoded;

namespace TorrentDownloader
{
    public class Torrent
    {
        public BDecoded.BDictionary Meta { get; private set; }
        public List<File> Files;
        public File TargetFile;
        public String[] Announces { get; private set; }
        public byte[] InfoHash { get; private set; }
        public Dictionary<String, TorrentTrackerInfo> Trackers;

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public long Size
        {
            get
            {
                return SingleFile ? TargetFile.Size : Files.Sum(f => f.Size);
            }
        }

        public bool SingleFile
        {
            get
            {
                return Files == null && this.TargetFile != null;
            }
        }

        private String original_info;

        
        public Torrent(String file_name)
        {
            Stream reader = new FileStream(file_name, FileMode.Open);
            BDecoded.BEncodedDecoder decoder = new BDecoded.BEncodedDecoder();
            BDecoded.IBElement parsed_torrent_file = decoder.Decode(reader);
            if (!(parsed_torrent_file is BDecoded.BDictionary)) 
                throw new FormatException();

            BDictionary meta_info = (BDictionary)parsed_torrent_file;
            Meta = meta_info;
            CollectAnnounces();
            CollectFiles((BDictionary)meta_info["info"]);

            //byte[] target_hash = StringToByteArray("b3b3e49c01d855e1e101dd7ebb32fd31e15a5bee").Reverse().ToArray();

            Stream stream_reader = new FileStream(file_name, FileMode.Open);
            byte[] content = new byte[stream_reader.Length];
            stream_reader.Read(content, 0, content.Length);
            List<byte> dynamic_content = new List<byte>(content);
            int index = 0;
            for (int i = 6; i < content.Length; i++)
            {
                if (content[i] == (byte)'d' && content[i - 1] == (byte)'o' && content[i - 2] == (byte)'f' && content[i - 3] == (byte)'n')
                    index = i;
            }
            dynamic_content = content.Skip(index).ToList();
            dynamic_content.RemoveAt(dynamic_content.Count - 1);
            InfoHash = SHA1.Create().ComputeHash(dynamic_content.ToArray()).Reverse().ToArray();

            Trackers = new Dictionary<string, TorrentTrackerInfo>();
            return;            
        }

        private void CollectFiles(BDictionary info)
        {
            if (info["files"] != null)
            {
                Files = new List<TorrentDownloader.File>();
                BList files_list = (BList)info["files"];
                foreach (BDictionary file_meta in files_list.Values)
                    Files.Add(ReadFileInfo(file_meta));
            }
            else if (info is BDictionary)
            {
                this.TargetFile = ReadFileInfo((BDictionary)info);
            }
            else
            {
                throw new FormatException();
            }
            return;
        }

        private File ReadFileInfo(BDictionary file_meta)
        {
            long size = Int64.Parse(file_meta["length"].ToString());
            String path = (file_meta["path"] == null ? file_meta["name"] : file_meta["path"]).ToString();
            return new File(path, size);
        }

        private void CollectAnnounces()
        {
            String announce = Meta["announce"].ToString();
            List<String> announces = new List<string>();
            int announces_count = 1;
            if (Meta["announce-list"] != null)
            {
                BList announces_list = (BList)Meta["announce-list"];
                announces = announces_list.Values.Select(v => v.ToString()).ToList();
                announces_count = announces.Count;
            }
            if (announces.Count > 0)
                Announces = announces.ToArray();
            else
            {
                Announces = new String[] { announce };
            }
            return;
        }

    }
}
