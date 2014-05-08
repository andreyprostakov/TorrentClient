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

        public byte[] InfoHash
        {
            get
            {
                String info_content = Meta["info"].BEncode();
                return SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(info_content));
            }
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
            BDecoded.Decoder decoder = new BDecoded.Decoder();
            BDecoded.IBElement parsed_torrent_file = decoder.Decode(reader);
            if (!(parsed_torrent_file is BDecoded.BDictionary)) 
                throw new FormatException();

            BDictionary meta_info = (BDictionary)parsed_torrent_file;
            Meta = meta_info;
            CollectAnnounces();
            CollectFiles((BDictionary)meta_info["info"]);

            StreamReader stream_reader = new StreamReader(file_name, Encoding.ASCII);
            String all_content = stream_reader.ReadToEnd();
            original_info = new string(all_content.Skip(all_content.IndexOf("4:infod") + "4:infod".Length).ToArray());
            stream_reader.Close();
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
