using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentDownloader
{
    public class TrackerHttpProtocol
    {
        Client Client { get; set; }
        Torrent Torrent {get; set;}
        int Port { get; set; }
        public BDecoded.BDictionary Parsed_response { get; set; }


        public TrackerHttpProtocol(Client client)
        {
            Client = client;
            Port = client.port_listen;
            return;
        }

        public bool Connect(Torrent torrent, String address, out String result_msg)
        {
            Torrent = torrent;
            if (address.Contains("http://")) return ConnectHttp(address, out result_msg);
            else result_msg = "Unknown address format";
            return false;
        }

        protected String BuildRequest(String announce_url)
        {
            UriGenerator uri_gen = new UriGenerator(announce_url);
            uri_gen.AddParameter("info_hash", Torrent.InfoHash);
            uri_gen.AddParameter("peer_id", Client.Id);
            uri_gen.AddParameter("left", Torrent.Size);
            uri_gen.AddParameter("port", Port);
            uri_gen.AddParameter("uploaded", 0);
            uri_gen.AddParameter("downloaded", 0);
            uri_gen.AddParameter("no_peers_id", 0);
            uri_gen.AddParameter("compact", 0);
            return uri_gen.Uri;
        }

        protected bool ConnectHttp(String address, out String result_msg)
        {
            String request_url = BuildRequest(address);
            String response_content = GetResponse(request_url, out result_msg);
            if (response_content == null) return false;
            BDecoded.BEncodedDecoder decoder = new BDecoded.BEncodedDecoder();
            Parsed_response = (BDecoded.BDictionary)decoder.Decode(new MemoryStream(Encoding.ASCII.GetBytes(response_content)));
            result_msg = "OK";
            return true;
        }

        public String GetResponse(String address, out String result_msg)
        {
            result_msg = "HTTP";            
            HttpWebRequest request = HttpWebRequest.Create(address) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 5000;
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
            catch (WebException ex)
            {
                result_msg = ex.Message;
                return null;
            }
        }
    }
}
