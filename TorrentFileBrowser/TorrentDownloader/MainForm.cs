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

        static String[] columns_headers = new String[] { "Announce", "Status", "Timeout", "Seeders", "Leechers", "Complete", "Incomplete", "Downloaded" };
        static int[] columns_widths = new int[] { 150, 100, 60, 60, 60, 60, 70, 70 };

        public MainForm()
        {
            InitializeComponent();
            dialogOpenTorrentFile.Filter = "Torrent files|*.torrent";
            client = new Client();
            InitTrackersTable();
            return;
        }

        private void openTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK)
            {
                torrent = new Torrent(dialogOpenTorrentFile.FileName);
                listPeers.Items.Clear();
                listPeers.Items.AddRange(torrent.PeersAddresses);
                textParsedTorrentFIle.Text = torrent.ToYml();
            }
            return;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitTrackersTable()
        {
            int columns_count = columns_headers.Length; ;
            tableTrackers.Rows.Clear();
            tableTrackers.ColumnCount = columns_count;
            tableTrackers.RowHeadersVisible = false;
            for (int i = 0; i < columns_count; i++)
            {
                tableTrackers.Columns[i].HeaderText = columns_headers[i];
                tableTrackers.Columns[i].Width = columns_widths[i];
            }
            return;
        }

        private void ShowTrackerInfo(TorrentTrackerInfo tracker_info)
        {
            int row_index = tableTrackers.Rows.Add();
            for (int i = 0; i < columns_headers.Length; i++)
            {
                tableTrackers.Rows[row_index].Cells[i].Value = tracker_info[columns_headers[i]];
            }
            return;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            String address = (String)listPeers.SelectedItem;
            if (address == "")
            {
                return;
            }
            PeerTcpProtocol peer = new PeerTcpProtocol(client);
            peer.Connect(torrent, address);
            return;
        }

        private void buttonUpdateTrackersInfo_Click(object sender, EventArgs e)
        {
            if (torrent == null) return;
            listPeers.Items.Clear();
            listPeers.Items.AddRange(torrent.PeersAddresses);
            client.CollectTorrentPeers(torrent);
            tableTrackers.Rows.Clear();
            foreach (var tracker_info in torrent.Announcers.Values)
            {
                ShowTrackerInfo(tracker_info);
            }
            return;
        }
    }
}
