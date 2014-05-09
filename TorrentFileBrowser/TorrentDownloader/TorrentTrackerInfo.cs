using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class TorrentTrackerInfo
    {
        public Torrent Torrent { get; private set; }
        public String AnnounceURL { get; private set; }
        public Dictionary<String, int> Stats { get; private set; }
        public int Interval { get; set; }
        public List<String> PeersAddresses { get; set; }

        public TorrentTrackerInfo(Torrent torrent, String announce_url)
        {
            Torrent = torrent;
            AnnounceURL = announce_url;
            Stats = new Dictionary<string, int>();
            return;
        }
    }
}
