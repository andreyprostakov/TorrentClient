using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BDecoded;

namespace TorrentDownloader
{
    public class TrackerHttpProtocol
    {
        public BDecoded.BDictionary Parsed_response { get; set; }
        private Client Client;
        private Torrent Torrent;
        private int Port;
        private String base_address;

        private static int TIMEOUT = 3000;


        public TrackerHttpProtocol(Client client)
        {
            Client = client;
            Port = client.port_listen;
            return;
        }

        public bool Connect(Torrent torrent, String address)
        {
            Torrent = torrent;
            base_address = address;
            if (address.Contains("http://")) return ConnectHttp(address);
            else torrent.Announcers[address].Status = "Unknown address format";
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

        protected bool ConnectHttp(String address)
        {
            String request_url = BuildRequest(address);
            String response_content = GetResponse(request_url);
            if (response_content == null) return false;
            BEncodedDecoder decoder = new BEncodedDecoder();
            Stream response_data_stream = new MemoryStream(Encoding.ASCII.GetBytes(response_content));
            Parsed_response = (BDictionary)BEncodedDecoder.DecodeStream(response_data_stream);
            return true;
        }

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
            catch (WebException ex)
            {
                Torrent.Announcers[base_address].Status = ex.Message;
                return null;
            }
        }
    }
}
