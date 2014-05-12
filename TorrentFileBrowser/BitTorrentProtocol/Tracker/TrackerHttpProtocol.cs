using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BDecoded;

namespace BitTorrentProtocol.Tracker
{
    public class TrackerHttpProtocol
    {
        private byte[] client_id;
        private int port;
        private TorrentTrackerInfo tracker_info;

        private const int TIMEOUT = 3000;


        public TrackerHttpProtocol(byte[] client_id, int port)
        {
            this.client_id = client_id;
            this.port = port;
            return;
        }

        /// <summary>
        /// Connect to specified tracker
        /// </summary>
        /// <returns>true if connection succeeded</returns>
        public bool Connect(TorrentTrackerInfo tracker_info)
        {
            this.tracker_info = tracker_info;
            String address = tracker_info.AnnounceUrl;
            if (address.Contains("http://")) return ConnectHttp(address);
            else tracker_info.Status = "Unknown address format";
            return false;
        }

        /// <summary>
        /// Send request, get response and parse it
        /// </summary>
        /// <returns>true if succeeded</returns>
        protected bool ConnectHttp(String address)
        {
            String request_url = BuildRequest(address);
            String response_content = GetResponse(request_url);
            if (response_content == null) return false;
            Stream response_data_stream = new MemoryStream(Encoding.ASCII.GetBytes(response_content));
            BDictionary response = (BDictionary)BEncodedDecoder.DecodeStream(response_data_stream);
            if (response["failure reason"] != null)
            {
                tracker_info.Status = response["failure reason"].ToString();
            }
            else
            {
                ParseResponse(response);
            }
            return true;
        }

        /// <summary>
        /// The one and only GET request message.
        /// Contains all interface data.
        /// </summary>
        /// <returns>request URL</returns>
        protected String BuildRequest(String announce_url)
        {
            UriGenerator uri_gen = new UriGenerator(announce_url);
            uri_gen.AddParameter("info_hash", tracker_info.Torrent.InfoHash);
            uri_gen.AddParameter("peer_id", client_id);
            uri_gen.AddParameter("left", tracker_info.Torrent.Size);
            uri_gen.AddParameter("port", port);
            uri_gen.AddParameter("uploaded", 0);
            uri_gen.AddParameter("downloaded", 0);
            uri_gen.AddParameter("no_peers_id", 0);
            uri_gen.AddParameter("compact", 0);
            return uri_gen.Uri;
        }

        /// <summary>
        /// Wait for response
        /// </summary>
        /// <returns>response content or null if failed</returns>
        public String GetResponse(String address)
        {        
            HttpWebRequest request = HttpWebRequest.Create(address) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = TIMEOUT;
            try
            {
                WebResponse response = request.GetResponse();
                Stream response_stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(response_stream);
                String response_content = reader.ReadToEnd();
                reader.Close();
                response_stream.Close();
                return response_content;
            }
            catch (WebException)
            {
                tracker_info.Status = "Wrong response from server";
                return null;
            }
        }

        /// <summary>
        /// Fill tracker info using data from server response
        /// </summary>
        /// <param name="peers_meta">BDecoded response</param>
        private void ParseResponse(BDictionary peers_meta)
        {
            tracker_info.Stats["Complete"] = Int32.Parse(peers_meta["complete"].ToString());
            tracker_info.Stats["Incomplete"] = Int32.Parse(peers_meta["incomplete"].ToString());
            tracker_info.PeersAddresses.Clear();
            for (int i = 0; i <= peers_meta.ToString().Length - 6; i += 6)
            {
                char[] address_encoded = peers_meta.ToString().Substring(i, 6).ToCharArray();
                String ip_encoded = new String(address_encoded.Take(4).Reverse().ToArray());
                byte[] ip = Encoding.UTF8.GetBytes(ip_encoded);
                String port_encoded = new String(address_encoded.Skip(4).Take(2).Reverse().ToArray());
                int port = BitConverter.ToInt16(Encoding.UTF8.GetBytes(port_encoded), 0);                
                tracker_info.PeersAddresses.Add(String.Format("{0}:{1}", String.Join(".", ip), port));
            }
            return;
        }
    }
}
