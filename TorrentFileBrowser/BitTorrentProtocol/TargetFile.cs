using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitTorrentProtocol
{
    public class TargetFile
    {
        public long Size {get;set;}
        public String Path {get;set;}
        public String Name
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }

        public TargetFile(String path, long size)
        {
            Path = path;
            Size = size;
            return;
        }

        public override string ToString()
        {
            return String.Format("path: {0}, length: {1}", Path, Size);
        }
    }
}
