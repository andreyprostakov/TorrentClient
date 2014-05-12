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
    public partial class FormMain : Form
    {
        private Torrent torrent;
        private Client client;

        static String[] TRACKERS_COLUMN_HEADERS = new String[] { "Announce", "Status", "Seeders", "Leechers", "Complete", "Incomplete", "Downloaded" };
        static int[] TRACKERS_COLUMN_WIDTHS = new int[] { 150, 100, 60, 60, 60, 70, 70 };

        public FormMain()
        {
            InitializeComponent();
            dialogOpenTorrentFile.Filter = "Torrent files|*.torrent";
            client = new Client();
            InitTrackersTable();
            PrepareTimer();
            return;
        }

        private void PrepareTimer()
        {
            timerDownloadProgress.Tick += new EventHandler(UpdateDownloadProgress);
            timerDownloadProgress.Interval = 1000;
            return;
        }

        private void UpdateDownloadProgress(Object sender, EventArgs e)
        {
            lock (torrent)
            {
                UpdateDownloadProgress();
                if (torrent.Completed == 1.0)
                {
                    OnStopDownloading();
                    torrent.OnDownloadFinished();
                }
            }
            return;
        }

        private void UpdateDownloadProgress()
        {
            int percents_progress = (int)(torrent.Completed * 100);
            progressDownload.Value = percents_progress;
            return;
        }

        private void openTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK)
            {
                OnNewTorrentFile();
            }
            return;
        }

        private void ShowTorrentFileInfo()
        {
            labelTorrentFileName.Text = Path.GetFileName(torrent.MetaFileName);
            textDestinationFolder.Text = torrent.DownloadDirectory;
            checkFiles.Items.Clear();
            checkFiles.Items.AddRange(torrent.Files.Select(f => FormattedFileInfo(f)).ToArray());
            return;
        }

        private String FormattedFileInfo(TargetFile file)
        {
            String name = Path.GetFileName(file.Path);
            String size_formatted = "0";
            int[] size_limits = new int[] {1000*1000*1000, 1000*1000, 1000, 0};
            String[] size_limita_abbr = new String[] {"GB", "MB", "KB", "B"};
            for (int i=0;i<size_limits.Length;i++)
            {
                if (file.Size > size_limits[i])
                {
                    double value = (size_limits[i] != 0 ? (double)file.Size / size_limits[i] : file.Size);
                    size_formatted = String.Format("{0} {1}", Math.Round(value, 2), size_limita_abbr[i]);
                    break;
                }
            }
            return String.Format("{0} ({1})", name, size_formatted);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitTrackersTable()
        {
            int columns_count = TRACKERS_COLUMN_HEADERS.Length; ;
            tableTrackers.Rows.Clear();
            tableTrackers.ColumnCount = columns_count;
            tableTrackers.RowHeadersVisible = false;
            for (int i = 0; i < columns_count; i++)
            {
                tableTrackers.Columns[i].HeaderText = TRACKERS_COLUMN_HEADERS[i];
                tableTrackers.Columns[i].Width = TRACKERS_COLUMN_WIDTHS[i];
            }
            return;
        }

        private void ShowTrackerInfo(TorrentTrackerInfo tracker_info)
        {
            int row_index = tableTrackers.Rows.Add();
            for (int i = 0; i < TRACKERS_COLUMN_HEADERS.Length; i++)
            {
                tableTrackers.Rows[row_index].Cells[i].Value = tracker_info[TRACKERS_COLUMN_HEADERS[i]];
            }
            return;
        }

        private void startDownloadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (torrent == null) return;
            if (client.StartDownloading(torrent))
            {
                OnStartDownloading();
            }
            return;
        }

        private void updateTrackingInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (torrent == null) return;
            OnUpdateTrackers();
            return;
        }

        private void aboutThisProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tracker downloader\n Written by Andrew Prostakov, 2014");
            return;
        }

        private void buttonChangeDestination_Click(object sender, EventArgs e)
        {
            if (dialogDestinationFolder.ShowDialog() == DialogResult.OK)
            {
                textDestinationFolder.Text = dialogDestinationFolder.SelectedPath;
            }
            return;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnStopDownloading();
            return;
        }

        private void OnNewTorrentFile()
        {
            torrent = new Torrent(dialogOpenTorrentFile.FileName);
            listPeers.Items.Clear();
            listPeers.Items.AddRange(torrent.PeersAddresses);
            tableTrackers.Rows.Clear();
            dialogDestinationFolder.SelectedPath = torrent.DownloadDirectory;
            buttonChangeDestination.Enabled = true;
            updateTrackingInfoToolStripMenuItem.Enabled = true;
            startDownloadingToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            ShowTorrentFileInfo();
            UpdateDownloadProgress();
            return;
        }

        private void OnUpdateTrackers()
        {
            client.CollectTorrentPeers(torrent);
            listPeers.Items.Clear();
            listPeers.Items.AddRange(torrent.PeersAddresses);
            tableTrackers.Rows.Clear();
            if (torrent.Announcers.Count > 0)
            {
                foreach (var tracker_info in torrent.Announcers.Values)
                {
                    ShowTrackerInfo(tracker_info);
                }
                startDownloadingToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
            }
            return;
        }

        private void OnStartDownloading()
        {
            startDownloadingToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            updateTrackingInfoToolStripMenuItem.Enabled = false;
            timerDownloadProgress.Start();
            openTorrentFileToolStripMenuItem.Enabled = false;
            return;
        }

        private void OnStopDownloading()
        {
            client.Pool.AbortAll();
            stopToolStripMenuItem.Enabled = false;
            startDownloadingToolStripMenuItem.Enabled = true;
            timerDownloadProgress.Stop();
            updateTrackingInfoToolStripMenuItem.Enabled = true;
            openTorrentFileToolStripMenuItem.Enabled = true;
            return;
        }
    }
}
