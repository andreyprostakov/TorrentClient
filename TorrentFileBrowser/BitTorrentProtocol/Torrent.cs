using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BDecoded;
using BitTorrentProtocol.Tracker;

namespace BitTorrentProtocol
{
    public class Torrent
    {
        public BDecoded.BDictionary Meta { get; private set; }
        public List<TargetFile> Files;
        public Dictionary<String, TorrentTrackerInfo> Announcers { get; private set; }
        public byte[] InfoHash { get; private set; }
        public String[] PeersAddresses { get; set; }
        public long Size
        {
            get
            {
                return Files.Sum(f => f.Size);
            }
        }
        public String DownloadDirectory { get; private set; }
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
        private byte[][] pieces_hashes;
        private String temp_download_file
        {
            get
            {
                String file_name = String.Format("~temp_buffer_{0}", Size.GetHashCode());
                return Path.Combine(this.DownloadDirectory, file_name);
            }
        }
        private String info_download_file
        {
            get
            {
                String file_name = String.Format("~dnld_info_{0}", Size.GetHashCode());
                return Path.Combine(this.DownloadDirectory, file_name);
            }
        }

        public static int FILE_BLOCK_SIZE = 16384;

        public Torrent(String file_name)
        {
            ReadMetaFile(file_name);
            InitDownloadDirectory();
            this.Bitfield = new BitField(PiecesCount);
            UpdateDownloadSatus();
            this.PeersAddresses = new String[0];
            return;            
        }


        //
        // Downloading
        //

        /// <summary>
        /// Prepare to start downloading this torrent
        /// </summary>
        /// <returns>true if success</returns>
        public bool PrepareForDownload()
        {
            if (!TouchDownloadDirectory()) return false;
            UpdateDownloadSatus();
            if (this.Completed == 1.0) return false;
            PrepareDiskSpace();
            return true;
        }

        /// <summary>
        /// Proccess received piece
        /// </summary>
        /// <returns>true if valid</returns>
        public bool SavePiece(byte[] piece_data, int index)
        {
            if (!CheckPieceHash(piece_data, index)) return false;
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
            catch (IOException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if piece hashsum is correct
        /// </summary>
        private bool CheckPieceHash(byte[] piece_content, int piece_index)
        {
            byte[] checksum = SHA1.Create().ComputeHash(piece_content);
            for (int i = 0; i < checksum.Length; i++)
            {
                if (checksum[i] != pieces_hashes[piece_index][i]) return false;
            }
            return true;
        }
                
        /// <summary>
        /// Actions after download is finished
        /// </summary>
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


        //
        // Receiving data from .torrent metafile
        //

        /// <summary>
        /// Fill itself with data from metafile
        /// </summary>
        /// <param name="file_name"></param>
        private void ReadMetaFile(String file_name)
        {
            IBElement parsed_torrent_file = BEncodedDecoder.DecodeStream(new FileStream(file_name, FileMode.Open));
            if (!(parsed_torrent_file is BDictionary))
                throw new FormatException("Wrong .torrent file format");
            this.Meta = (BDictionary)parsed_torrent_file;
            CollectAnnounces();
            CollectFiles();
            CollectHashSums(file_name);
            return;
        }

        /// <summary>
        /// Collect announces info from metafile into dictionary
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
        /// Extract files info from metafile
        /// </summary>
        /// <param name="info">DBecoded "info" dictionary</param>
        private void CollectFiles()
        {
            BDictionary info = (BDictionary)Meta["info"];
            if (info["files"] != null)
            {
                Files = new List<TargetFile>();
                BList files_list = (BList)info["files"];
                foreach (BDictionary file_meta in files_list.Values)
                    Files.Add(ConvertFileInfo(file_meta));
            }
            else if (info is BDictionary)
            {
                Files = new List<TargetFile>();
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
        /// Extract raw info data from metafile.
        /// Necessary to avoid encoding and decoding hashsums.
        /// </summary>
        /// <param name="file_name">.torrent file name</param>
        private void CollectHashSums(String file_name)
        {
            Stream stream_reader = new FileStream(file_name, FileMode.Open);
            byte[] content = new byte[stream_reader.Length];
            stream_reader.Read(content, 0, content.Length);
            stream_reader.Close();
            // Extracting info section content
            byte[] info_content = ExtractBDictionaryElement(content, "4:info");
            info_content = info_content.Take(info_content.Length - 1).ToArray();
            ExtractPiecesHashes(info_content);
            return;
        }
        private void ExtractPiecesHashes(byte[] info_content)
        {
            byte[] pieces_content = ExtractBDictionaryElement(info_content, "6:pieces");
            int pieces_content_length = ((BDictionary)Meta["info"])["pieces"].ToString().Length;
            for (int i = 0; i < pieces_content.Length; i++)
                if (pieces_content[i] == (byte)':')
                {
                    pieces_content = pieces_content.Skip(i + 1).Take(pieces_content_length).ToArray();
                    break;
                }
            if (pieces_content.Length != pieces_content_length || pieces_content.Length % 20 != 0) throw new FormatException("Wrong torrent file format");
            this.pieces_hashes = new byte[PiecesCount][];
            for (int i = 0; i < PiecesCount; i++)
            {
                pieces_hashes[i] = pieces_content.Skip(20 * i).Take(20).ToArray();
            }
            InfoHash = SHA1.Create().ComputeHash(info_content).Reverse().ToArray();
            return;
        }
        private byte[] ExtractBDictionaryElement(byte[] source, String name)
        {
            int start_index = FindSequenceIndex(source, Encoding.UTF8.GetBytes(name));
            if (start_index < 0) return null;
            return source.Skip(start_index + name.Length).ToArray();
        }
        private int FindSequenceIndex(byte[] source, byte[] key)
        {
            for (int i = 0; i < source.Length - key.Length + 1; i++)
            {
                if (source[i] != key[0]) continue;
                bool matches = true;
                for (int j = 1; j < key.Length; j++)
                {
                    if (source[i + j] != key[j])
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches) return i;
            }
            return -1;
        }


        /// <summary>
        /// Set default value as destination path
        /// </summary>
        private void InitDownloadDirectory()
        {
            String dir_name = String.Format("download_{0}", Size.GetHashCode());
            this.DownloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), dir_name);
            return;
        }

        /// <summary>
        /// Change directory and update info
        /// </summary>
        public void ChangeDownloadDirectory(String new_path)
        {
            DownloadDirectory = new_path;
            TouchDownloadDirectory();
            UpdateDownloadSatus();
            return;
        }

        /// <summary>
        /// Check if destination directory is ready
        /// </summary>
        /// <returns></returns>
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
        /// Create temporal file to collect incoming pieces
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

        /// <summary>
        /// Load status file if found one
        /// </summary>
        protected void UpdateDownloadSatus()
        {
            if (File.Exists(info_download_file))
            {
                Bitfield.LoadFrom(info_download_file);
            }
            else
            {
                this.Bitfield = new BitField(PiecesCount);
            }
            return;
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
    }
}
