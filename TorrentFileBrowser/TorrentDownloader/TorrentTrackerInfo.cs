using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class TorrentTrackerInfo
    {
        public String AnnounceUrl { get; private set; }
        public Torrent Torrent { get; private set; }
        public Dictionary<String, int> Stats { get; private set; }
        public int Interval { get; set; }
        public List<String> PeersAddresses { get; set; }
        public String Status { get; set; }

        public TorrentTrackerInfo(Torrent torrent, String announce_url)
        {
            Torrent = torrent;
            AnnounceUrl = announce_url;
            Stats = new Dictionary<string, int>();
            Status = "Ok";
            PeersAddresses = new List<string>();
            return;
        }

        public String this[String attribute]
        {
            get
            {
                switch (attribute)
                {
                    case "Announce": return AnnounceUrl;
                    case "Timeout": return Interval.ToString();
                    case "Status": return Status;
                    default: 
                        int value;
                        if (Stats.TryGetValue(attribute, out value)) return value.ToString();
                        else return "";
                }
            }
        }
    }
}
