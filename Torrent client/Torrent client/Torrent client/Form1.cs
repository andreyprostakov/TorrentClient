using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Torrent_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLoadTorrent_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ShowFileContent(openFileDialog1.FileName);
            }
        }

        void ShowFileContent(String filename)
        {
            TorrentFile torrent = new TorrentFile(filename);
            textTorrentFileContent.Text = torrent.ParsedContent();
            return;
        }
    }
}
