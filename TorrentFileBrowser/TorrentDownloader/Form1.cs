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
    public partial class Form1 : Form
    {
        Torrent torrent;
        Client client;

        public Form1()
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
            }
            return;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            HideErrorMessage();
            String uri = (String)listAnnounces.SelectedItem;
            if (uri == "") 
            {
                ShowErrorMessage("URI not determined");
                return; 
            }
            String result;
            BDecoded.IBElement response;
            if ((response = client.ConnectAnnouncer(torrent, uri, out result)) != null)
            {
                BDecoded.OutputAsYml printer = new BDecoded.OutputAsYml();
                textResponce.Text = printer.Output(response);            
            }
            else
            {
                ShowErrorMessage(result);
                textResponce.Text = "";
            }           
            return;
        }

        protected void HideErrorMessage()
        {
            labelErrorMessage.Hide();
            return;
        }

        protected void ShowErrorMessage(String message)
        {
            labelErrorMessage.Text = message;
            labelErrorMessage.Show();
            return;
        }

        private void ConnectToPeer(String IP, int port)
        {
            TcpClient tcp_client = new TcpClient();
            tcp_client.Connect("127.21.5.99", 45746);
            var stream = tcp_client.GetStream();
            String msg_handshake = String.Format("handshake: 19BitTorrent protocol        {0}{1}", HttpUtility.UrlEncode(client.Id), HttpUtility.UrlEncode(client.Id));
            stream.Write(Encoding.ASCII.GetBytes(msg_handshake), 0, msg_handshake.Length);
            byte[] buffer = new byte[128];
            int result = stream.Read(buffer, 0, 4);
            String received = Encoding.ASCII.GetString(buffer, 0, result);
            tcp_client.Close();
            return;
        }
    }
}
