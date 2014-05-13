using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesSimilarityCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLoad1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream reader = new FileStream(openFileDialog1.FileName, FileMode.Open);
                byte[] content = new byte[reader.Length];
                reader.Read(content, 0, (int)reader.Length);
                textFile1.Text = BitConverter.ToString(content);
                reader.Close();
            }
            return;
        }

        private void buttonLoad2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream reader = new FileStream(openFileDialog1.FileName, FileMode.Open);
                byte[] content = new byte[reader.Length];
                reader.Read(content, 0, (int)reader.Length);
                textFile2.Text = BitConverter.ToString(content);
                reader.Close();
            }
            return;
        }
    }
}
