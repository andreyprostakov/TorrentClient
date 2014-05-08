namespace TorrentDownloader
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label3 = new System.Windows.Forms.Label();
            this.textResponce = new System.Windows.Forms.RichTextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textParsedTorrentFIle = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTorrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogOpenTorrentFile = new System.Windows.Forms.OpenFileDialog();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.listAnnounces = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(554, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Response:";
            // 
            // textResponce
            // 
            this.textResponce.Location = new System.Drawing.Point(557, 58);
            this.textResponce.Name = "textResponce";
            this.textResponce.Size = new System.Drawing.Size(248, 144);
            this.textResponce.TabIndex = 17;
            this.textResponce.Text = "";
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.CornflowerBlue;
            this.buttonConnect.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonConnect.Location = new System.Drawing.Point(301, 159);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(237, 50);
            this.buttonConnect.TabIndex = 16;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = false;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Announce URLs:";
            // 
            // textParsedTorrentFIle
            // 
            this.textParsedTorrentFIle.Location = new System.Drawing.Point(12, 58);
            this.textParsedTorrentFIle.Name = "textParsedTorrentFIle";
            this.textParsedTorrentFIle.Size = new System.Drawing.Size(276, 245);
            this.textParsedTorrentFIle.TabIndex = 21;
            this.textParsedTorrentFIle.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Torrent file:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.MediumPurple;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(847, 24);
            this.menuStrip1.TabIndex = 22;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTorrentFileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openTorrentFileToolStripMenuItem
            // 
            this.openTorrentFileToolStripMenuItem.Name = "openTorrentFileToolStripMenuItem";
            this.openTorrentFileToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openTorrentFileToolStripMenuItem.Text = "Open torrent file..";
            this.openTorrentFileToolStripMenuItem.Click += new System.EventHandler(this.openTorrentFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelErrorMessage.Location = new System.Drawing.Point(313, 222);
            this.labelErrorMessage.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(497, 39);
            this.labelErrorMessage.TabIndex = 23;
            this.labelErrorMessage.Text = resources.GetString("labelErrorMessage.Text");
            this.labelErrorMessage.Visible = false;
            // 
            // listAnnounces
            // 
            this.listAnnounces.FormattingEnabled = true;
            this.listAnnounces.Location = new System.Drawing.Point(304, 58);
            this.listAnnounces.Name = "listAnnounces";
            this.listAnnounces.Size = new System.Drawing.Size(234, 95);
            this.listAnnounces.TabIndex = 24;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(847, 309);
            this.Controls.Add(this.listAnnounces);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.textParsedTorrentFIle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textResponce);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Torrent downloader";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox textResponce;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox textParsedTorrentFIle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTorrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog dialogOpenTorrentFile;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.ListBox listAnnounces;
    }
}

