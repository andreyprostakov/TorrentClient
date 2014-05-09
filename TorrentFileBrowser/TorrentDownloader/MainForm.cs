using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace TorrentDownloader
{
    public partial class MainForm : Form
    {
        Torrent torrent;
        Client client;

        public MainForm()
        {
            InitializeComponent();
            dialogOpenTorrentFile.Filter = "Torrent files|*.torrent";
            client = new Client();
            return;
        }

        private void openTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideErrorMessage();
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK)
            {
                torrent = new Torrent(dialogOpenTorrentFile.FileName);
                client.CollectTorrentPeers(torrent);
                listPeers.Items.Clear();
                listPeers.Items.AddRange(torrent.PeersAddresses);
                textParsedTorrentFIle.Text = torrent.ToYml();
                listAnnounces.Items.Clear();
                listAnnounces.Items.AddRange(torrent.Announces);
                var hash = torrent.InfoHash;
            }
            return;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        protected void HideErrorMessage()
        {
            textAnnouncerInfo.Text = "";
            return;
        }

        protected void ShowErrorMessage(String message)
        {
            textAnnouncerInfo.Text = message;
            return;
        }

        private void ConnectToPeer(String ip, int port)
        {
            try
            {
                TcpClient tcp_client = new TcpClient(ip, port);
                var stream = tcp_client.GetStream();
                List<byte> message = new List<byte>();
                message.Add((byte)19);
                message.AddRange(Encoding.UTF8.GetBytes("BitTorrent protocol"));
                message.AddRange(new byte[8]);
                message.AddRange(torrent.InfoHash.Reverse().ToArray());
                message.AddRange(client.Id);
                stream.Write(message.ToArray(), 0, message.Count);
                byte[] buffer = new byte[128];
                int result = stream.Read(buffer, 0, 128);
                if (result > 68)
                {
                    int pstrlen = buffer[0];
                    String protocol = Encoding.UTF8.GetString(buffer, 1, pstrlen);
                    byte[] info_hash = buffer.Skip(28).Take(20).ToArray();
                    byte[] peer_id = buffer.Skip(48).Take(20).ToArray();
                    result = 0;
                }
                tcp_client.Close();
            }
            catch (SocketException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            return;
        }

        private void ConnectAnnouncer(String announce_url)
        {
            String result;
            TorrentTrackerInfo tracker_info;
            if ((tracker_info = client.ConnectAnnouncer(torrent, announce_url, out result)) != null)
            {
                textAnnouncerInfo.Text = "";
                foreach (var key_value in tracker_info.Stats)
                {
                    textAnnouncerInfo.Text += String.Format("{0}: {1}\n", key_value.Key, key_value.Value);
                }
            }
            else
            {
                ShowErrorMessage(result);
            }           
            return;
        }

        private void listAnnounces_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideErrorMessage();
            String uri = (String)listAnnounces.SelectedItem;
            if (uri == "")
            {
                ShowErrorMessage("URI not determined");
                return;
            }
            ConnectAnnouncer(uri);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            HideErrorMessage();
            String address = (String)listPeers.SelectedItem;
            if (address == "")
            {
                ShowErrorMessage("URI not determined");
                return;
            }
            PeerTcpProtocol peer = new PeerTcpProtocol(client);
            peer.Connect(torrent, address);
            return;
        }
    }
}
