using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;
using System.Windows.Forms;
using BDecoded;

namespace TorrentFileBrowser
{
    public partial class Form1 : Form
    {
        Torrent torrent_file;
        byte[] info_sha;
        IBElement decoded;
        String current_file_name;

        public Form1()
        {
            InitializeComponent();
            dialogOpenTorrentFile.Filter = "Torrent files|*.torrent";
            dialogSaveTorrentFile.Filter = "Torrent files|*.torrent";
            return;
        }

        protected void ShowFileContent(String filename)
        {
            HideErrorMessage();
            StreamReader reader = new StreamReader(filename);
            String raw_content = reader.ReadToEnd();
            textRawContent.Text = raw_content;
            reader.Close();
            Stream file_stream = new FileStream(filename, FileMode.Open);
            BDecoded.BEncodedDecoder decoder = new BDecoded.BEncodedDecoder();
            try
            {
                OutputAsYml output = new OutputAsYml();
                decoded = decoder.Decode(file_stream);
                textParsedContent.Text = output.Output(decoded);
                torrent_file = new Torrent(decoded);
                textOutputContent.Text = decoded.BEncode();
            }
            catch (FormatException ex)
            {
                ShowErrorMessage("Wrong file format");
            }
            catch (StackOverflowException ex)
            {
                ShowErrorMessage("Overflow!");
            }
            return;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogOpenTorrentFile.Multiselect = false;
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK)
            {
                current_file_name = dialogOpenTorrentFile.FileName; 
                labelFilename.Text = current_file_name;
                ShowFileContent(current_file_name);
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

        private void buttonParse_Click(object sender, EventArgs e)
        {
            HideErrorMessage();
            if (String.IsNullOrEmpty(current_file_name))
            {
                ShowErrorMessage("No file selected");
                return;
            }
            ShowFileContent(current_file_name);
            return;
        }

        private void saveAsTorrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogSaveTorrentFile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(dialogSaveTorrentFile.FileName);
                writer.Write(decoded.BEncode());
                writer.Close();
            }
            return;
        }

        private void compareFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogOpenTorrentFile.Multiselect = true;
            if (dialogOpenTorrentFile.ShowDialog() == DialogResult.OK && dialogOpenTorrentFile.FileNames.Count() > 1)
            {
                List<StreamReader> readers = new List<StreamReader>();
                bool files_equal = true;
                try
                {
                    foreach (String filename in dialogOpenTorrentFile.FileNames)
                        readers.Add(new StreamReader(filename, Encoding.ASCII));
                    while (readers.Find(r => r.EndOfStream) == null)
                    {
                        int check_byte = readers.First().Read();
                        if (readers.Find(r => r != readers.First() && r.Read() != check_byte) != null)
                            break;
                    }
                    files_equal = !readers.Exists(r => !r.EndOfStream);
                }
                finally
                {
                    foreach (var reader in readers)
                        reader.Close();
                }
            }
        }
    }
}
