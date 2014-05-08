using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDecoded;

namespace TorrentFileBrowser
{
    public class Torrent
    {
        protected IBElement parsedContent;

        public Torrent(IBElement meta)
        {
            parsedContent = meta;
            return;
        }
    }
}
