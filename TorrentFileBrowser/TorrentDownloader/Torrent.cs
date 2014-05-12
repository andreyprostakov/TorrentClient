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
        public String MetaFileName { get; private set; }
        public BDecoded.BDictionary Meta { get; private set; }
        public List<TargetFile> Files;
        public Dictionary<String, TorrentTrackerInfo> Announcers { get; private set; }
        public byte[] InfoHash { get; private set; }
        public Dictionary<String, TorrentTrackerInfo> Trackers { get; set; }
        public String[] PeersAddresses { get; set; }
        public long Size
        {
            get
            {
                return Files.Sum(f => f.Size);
            }
        }
        public String DownloadDirectory {
            get
            {
                String dir_name = String.Format("download_{0}", MetaFileName.GetHashCode());
                return Path.Combine(Directory.GetCurrentDirectory(), dir_name);
            }
        }
        public BitField Bitfield;
        public int PiecesCount
        {
            get {
                return (int)Math.Ceiling((double)this.Size / this.PieceLength());
            }
        }
        public float Completed
        {
            get
            {
                return 1 - (float)Bitfield.MissingPieces().Length / PiecesCount;
            }
        }

        private String temp_download_file
        {
            get
            {
                String file_name = String.Format("~temp_buffer_{0}", MetaFileName.GetHashCode());
                return Path.Combine(this.DownloadDirectory, file_name);
            }
        }
        private String info_download_file
        {
            get
            {
                String file_name = String.Format("~dnld_info_{0}", MetaFileName.GetHashCode());
                return Path.Combine(this.DownloadDirectory, file_name);
            }
        }
        
        public static int FILE_BLOCK_SIZE = 16384;


        public Torrent(String file_name)
        {
            this.MetaFileName = file_name;
            IBElement parsed_torrent_file = BEncodedDecoder.DecodeStream(new FileStream(file_name, FileMode.Open));
            if (!(parsed_torrent_file is BDictionary)) 
                throw new FormatException("Wrong .torrent file format");

            this.Meta = (BDictionary)parsed_torrent_file;
            CollectAnnounces();
            CollectFiles();
            ComputeInfoHash(file_name);
            this.Bitfield = new BitField(PiecesCount);
            UpdateDownloadSatus();
            this.Trackers = new Dictionary<string, TorrentTrackerInfo>();
            this.PeersAddresses = new String[0];
            return;            
        }

        /// <summary>
        /// Proccess received piece
        /// </summary>
        /// <returns>true if valid</returns>
        public bool SavePiece(byte[] piece_data, int index)
        {
            byte[] checksum = SHA1.Create().ComputeHash(piece_data);
            //byte[] true_checksum = PieceHash(index);
            try
            {
                using (FileStream writer = new FileStream(temp_download_file, FileMode.Open))
                {
                    long result = writer.Seek(index * (long)PieceLength(), SeekOrigin.Begin);
                    writer.Write(piece_data, 0, piece_data.Length);
                }
                Bitfield.Set(index);
                Bitfield.SaveTo(info_download_file);
            }
            catch (IOException ex)
            {
                return false;
            }
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

        public bool PrepareForDownload()
        {
            if (!TouchDownloadDirectory()) return false;
            UpdateDownloadSatus();
            if (this.Completed == 1.0) return false;
            PrepareDiskSpace();
            return true;
        }

        public int PieceLength(int index = 0)
        {
            String text_value = ((BDictionary)Meta["info"])["piece length"].ToString();
            int length = Int32.Parse(text_value);
            if ((index + 1) * length > this.Size)
            {
                return (int)(Size - index * length);
            }
            return length;
        }
        
        public void OnDownloadFinished()
        {
            if (!File.Exists(temp_download_file)) throw new IOException();
            using (FileStream reader = new FileStream(temp_download_file, FileMode.Open))
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    String target_path = Path.Combine(DownloadDirectory, Files[i].Name);
                    FileStream writer = new FileStream(target_path, FileMode.Create);
                    byte[] content = new byte[Files[i].Size];
                    reader.Read(content, 0, (int)Files[i].Size);
                    writer.Write(content, 0, (int)Files[i].Size);
                    writer.Close();
                }
            }
            File.Delete(temp_download_file);
            return;
        }
        
        protected bool TouchDownloadDirectory()
        {
            try
            {
                if (!Directory.Exists(this.DownloadDirectory))
                    Directory.CreateDirectory(this.DownloadDirectory);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        /// <summary>
        /// Create temporal file to contain incoming pieces
        /// </summary>
        protected void PrepareDiskSpace()
        {
            if (!File.Exists(temp_download_file) || (new FileInfo(temp_download_file)).Length < Size)
            {
                using (FileStream writer = new FileStream(temp_download_file, FileMode.Create))
                {
                    writer.Seek(this.Size - 1, SeekOrigin.Begin);
                    writer.WriteByte(0);
                }
            }
            return;
        }

        protected void UpdateDownloadSatus()
        {
            if (File.Exists(info_download_file))
            {
                FileStream meta_file = null;
                try
                {
                    meta_file = File.Open(info_download_file, FileMode.Open);
                    byte[] data = new byte[Bitfield.Length];
                    int read_bytes = meta_file.Read(data, 0, Bitfield.Length);
                    Bitfield.Sum(data);
                }
                finally
                {
                    if (meta_file != null) meta_file.Close();
                }
            }
            else
            {
                this.Bitfield = new BitField(PiecesCount);
            }
            return;
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
        private void CollectFiles()
        {
            BDictionary info = (BDictionary)Meta["info"];
            if (info["files"] != null)
            {
                Files = new List<TorrentDownloader.TargetFile>();
                BList files_list = (BList)info["files"];
                foreach (BDictionary file_meta in files_list.Values)
                    Files.Add(ConvertFileInfo(file_meta));
            }
            else if (info is BDictionary)
            {
                Files = new List<TorrentDownloader.TargetFile>();
                Files.Add(ConvertFileInfo((BDictionary)info));
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
        private TargetFile ConvertFileInfo(BDictionary file_meta)
        {
            long size = Int64.Parse(file_meta["length"].ToString());
            String path;
            IBElement path_meta = file_meta["path"];
            if (path_meta != null)
            {
                if (path_meta is BList)
                    path = String.Join("/", ((BList)path_meta).Values.Select(v => v.ToString()));
                else
                    path = path_meta.ToString();
            }
            else if (file_meta["name"] != null)
            {
                path = file_meta["name"].ToString();
            }
            else throw new FormatException("Wrong torrent file format");
            return new TargetFile(path, size);
        }

        /// <summary>
        /// Collect announces info into specified dictionary
        /// </summary>
        private void CollectAnnounces()
        {            
            String announce = Meta["announce"].ToString();
            List<String> announces = new List<string>();
            if (Meta["announce-list"] != null)
            {
                BList announces_list = (BList)Meta["announce-list"];
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
            int start_checksum_index = index * PieceLength();
            if (start_checksum_index + PieceLength() > total_checksum.Length) throw new FormatException("Checksum error");
            String piece_checksum = total_checksum.Substring(start_checksum_index, PieceLength());
            return Encoding.UTF8.GetBytes(piece_checksum);
        }

    }
}
