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
        public Dictionary<String, TorrentTrackerInfo> Announcers { get; private set; }
        public byte[] InfoHash { get; private set; }
        public Dictionary<String, TorrentTrackerInfo> Trackers { get; set; }
        public String[] PeersAddresses { get; set; }
        public long Size
        {
            get
            {
                return this.IsSingleFile ? TargetFile.Size : Files.Sum(f => f.Size);
            }
        }
        public bool IsSingleFile
        {
            get
            {
                return Files == null && this.TargetFile != null;
            }
        }
        public String Directory = @"D:\Учеба\Курсовой проект\assets";
        public byte[] Bitfield;
        public int PieceLength { 
            get {
                String text_value = ((BDictionary)Meta["info"])["piece length"].ToString();
                return Int32.Parse(text_value);
            } 
        }


        public Torrent(String file_name)
        {
            IBElement parsed_torrent_file = BEncodedDecoder.DecodeStream(new FileStream(file_name, FileMode.Open));
            if (!(parsed_torrent_file is BDictionary)) 
                throw new FormatException("Wrong .torrent file format");

            Meta = (BDictionary)parsed_torrent_file;
            CollectAnnounces(Meta["announce"], Meta["announce-list"]);
            CollectFiles((BDictionary)Meta["info"]);
            ComputeInfoHash(file_name);
            Trackers = new Dictionary<string, TorrentTrackerInfo>();
            Bitfield = new byte[Size / PieceLength + 1];
            PeersAddresses = new String[0];
            return;            
        }

        /// <summary>
        /// Proccess received piece
        /// </summary>
        /// <returns>true if valid</returns>
        public bool SavePiece(byte[] piece_data, int index)
        {
            byte[] checksum = SHA1.Create().ComputeHash(piece_data);
            byte[] true_checksum = PieceHash(index);
            for (int i = 0; i < checksum.Length; i++)
            {
                if (checksum[i] != true_checksum[i]) return false;
            }
            SetPieceBit(index);
            return true;
        }

        /// <summary>
        /// Convert torrent meta info into user-friendly YML
        /// </summary>
        /// <returns></returns>
        public String ToYml()
        {
            BDecoded.OutputAsYml printer = new BDecoded.OutputAsYml();
            return printer.Output(this.Meta);
        }


        /// <summary>
        /// info_hash is necessary for torrent protocol
        /// </summary>
        /// <param name="file_name">.torrent file name</param>
        private void ComputeInfoHash(String file_name)
        {
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
            return;            
        }

        /// <summary>
        /// Import torrent files info from bdecode structure
        /// </summary>
        /// <param name="info">DBecoded "info" dictionary</param>
        private void CollectFiles(BDictionary info)
        {
            if (info["files"] != null)
            {
                Files = new List<TorrentDownloader.File>();
                BList files_list = (BList)info["files"];
                foreach (BDictionary file_meta in files_list.Values)
                    Files.Add(ConvertFileInfo(file_meta));
            }
            else if (info is BDictionary)
            {
                this.TargetFile = ConvertFileInfo((BDictionary)info);
            }
            else
            {
                throw new FormatException();
            }
            return;
        }

        /// <summary>
        /// Translate bdecoded file data into specific File class (bridge)
        /// </summary>
        /// <returns>translation</returns>
        private File ConvertFileInfo(BDictionary file_meta)
        {
            long size = Int64.Parse(file_meta["length"].ToString());
            String path = (file_meta["path"] == null ? file_meta["name"] : file_meta["path"]).ToString();
            return new File(path, size);
        }

        /// <summary>
        /// Collect announces info into specified dictionary
        /// </summary>
        private void CollectAnnounces(IBElement meta_announce, IBElement meta_announce_list)
        {
            String announce = meta_announce.ToString();
            List<String> announces = new List<string>();
            if (meta_announce_list != null)
            {
                BList announces_list = (BList)meta_announce_list;
                announces = announces_list.Values.Select(v => v.ToString()).ToList();
            }
            this.Announcers = new Dictionary<string, TorrentTrackerInfo>();
            foreach (String announcer in announces)
                this.Announcers[announcer] = new TorrentTrackerInfo(this, announcer);
            return;
        }

        /// <summary>
        /// Checksum for specific peace
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private byte[] PieceHash(int index)
        {
            String total_checksum = ((BDictionary)Meta["info"])["pieces"].ToString();
            int start_checksum_index = index * PieceLength;
            if (start_checksum_index + PieceLength > total_checksum.Length) throw new FormatException("Checksum error");
            String piece_checksum = total_checksum.Substring(start_checksum_index, PieceLength);
            return Encoding.UTF8.GetBytes(piece_checksum);
        }

        /// <summary>
        /// Change bit in status bitfield
        /// </summary>
        /// <param name="index">index of a piece (bit)</param>
        private void SetPieceBit(int index)
        {
            int index_byte = index / 8;
            int index_bit = index % 8;
            byte map = (byte)(128 >> index_bit);
            if (index_byte >= Bitfield.Length) throw new ArgumentException();
            Bitfield[index_byte] |= map;
            return;
        }

    }
}
