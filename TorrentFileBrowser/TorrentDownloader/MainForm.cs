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
            client = new Client(this);
            //ConnectToPeer(null, 2);
            return;
        }

        private void openTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideErrorMessage();
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK)
            {
                torrent = new Torrent(dialogOpenTorrentFile.FileName);
                BDecoded.OutputAsYml printer = new BDecoded.OutputAsYml();
                textParsedTorrentFIle.Text = printer.Output(torrent.Meta);
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
            //labelErrorMessage.Hide();
            textAnnouncerInfo.Text = "";
            return;
        }

        protected void ShowErrorMessage(String message)
        {
            //labelErrorMessage.Text = message;
            //labelErrorMessage.Show();
            textAnnouncerInfo.Text = message;
            return;
        }

        private void ConnectToPeer(IPAddress IP, int port)
        {
            TcpClient tcp_client = new TcpClient();
            try
            {
                tcp_client.Connect("93.171.161.49", 47278);
                var stream = tcp_client.GetStream();
                String msg_handshake = String.Format("handshake: 19BitTorrent protocol        {0}{1}", HttpUtility.UrlEncode(client.Id), HttpUtility.UrlEncode(torrent.InfoHash));
                stream.Write(Encoding.ASCII.GetBytes(msg_handshake), 0, msg_handshake.Length);
                byte[] buffer = new byte[128];
                int result = stream.Read(buffer, 0, 4);
                String received = Encoding.ASCII.GetString(buffer, 0, result);
            }
            catch (SocketException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            tcp_client.Close();
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
                listPeers.Items.Clear();
                listPeers.Items.AddRange(tracker_info.PeersAddresses.ToArray());
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

        private void buttonConnect_Click_1(object sender, EventArgs e)
        {
            HideErrorMessage();
            String address = (String)listPeers.SelectedItem;
            if (address == "")
            {
                ShowErrorMessage("URI not determined");
                return;
            }
            String[] address_parts = address.Split(':');
            IPAddress ip = IPAddress.Parse(address_parts[0]);
            int port = Int32.Parse(address_parts[1]);
            ConnectToPeer(ip, port);
            return;
        }
    }
}
