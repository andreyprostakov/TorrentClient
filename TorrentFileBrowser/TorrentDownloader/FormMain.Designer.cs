namespace TorrentDownloader
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTorrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTrackingInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startDownloadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutThisProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogOpenTorrentFile = new System.Windows.Forms.OpenFileDialog();
            this.listPeers = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableTrackers = new System.Windows.Forms.DataGridView();
            this.dialogDestinationFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.labelTorrentFileName = new System.Windows.Forms.Label();
            this.checkFiles = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonChangeDestination = new System.Windows.Forms.Button();
            this.textDestinationFolder = new System.Windows.Forms.TextBox();
            this.progressDownload = new System.Windows.Forms.ProgressBar();
            this.timerDownloadProgress = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableTrackers)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 252);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Trackers:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Torrent file:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.updateTrackingInfoToolStripMenuItem,
            this.startDownloadingToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.aboutThisProgramToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(588, 24);
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
            // updateTrackingInfoToolStripMenuItem
            // 
            this.updateTrackingInfoToolStripMenuItem.Enabled = false;
            this.updateTrackingInfoToolStripMenuItem.Name = "updateTrackingInfoToolStripMenuItem";
            this.updateTrackingInfoToolStripMenuItem.Size = new System.Drawing.Size(127, 20);
            this.updateTrackingInfoToolStripMenuItem.Text = "Update tracking info";
            this.updateTrackingInfoToolStripMenuItem.Click += new System.EventHandler(this.updateTrackingInfoToolStripMenuItem_Click);
            // 
            // startDownloadingToolStripMenuItem
            // 
            this.startDownloadingToolStripMenuItem.Enabled = false;
            this.startDownloadingToolStripMenuItem.Image = global::TorrentDownloader.Properties.Resources.start_icon;
            this.startDownloadingToolStripMenuItem.Name = "startDownloadingToolStripMenuItem";
            this.startDownloadingToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
            this.startDownloadingToolStripMenuItem.ToolTipText = "Start downloading";
            this.startDownloadingToolStripMenuItem.Click += new System.EventHandler(this.startDownloadingToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Image = global::TorrentDownloader.Properties.Resources.stop_red_icon;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
            this.stopToolStripMenuItem.ToolTipText = "Stop downloading";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // aboutThisProgramToolStripMenuItem
            // 
            this.aboutThisProgramToolStripMenuItem.Name = "aboutThisProgramToolStripMenuItem";
            this.aboutThisProgramToolStripMenuItem.Size = new System.Drawing.Size(129, 20);
            this.aboutThisProgramToolStripMenuItem.Text = "About this program..";
            this.aboutThisProgramToolStripMenuItem.Click += new System.EventHandler(this.aboutThisProgramToolStripMenuItem_Click);
            // 
            // listPeers
            // 
            this.listPeers.FormattingEnabled = true;
            this.listPeers.Location = new System.Drawing.Point(398, 58);
            this.listPeers.Name = "listPeers";
            this.listPeers.Size = new System.Drawing.Size(174, 173);
            this.listPeers.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(395, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Peers:";
            // 
            // tableTrackers
            // 
            this.tableTrackers.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tableTrackers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableTrackers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tableTrackers.Location = new System.Drawing.Point(12, 274);
            this.tableTrackers.Name = "tableTrackers";
            this.tableTrackers.Size = new System.Drawing.Size(560, 137);
            this.tableTrackers.TabIndex = 27;
            // 
            // labelTorrentFileName
            // 
            this.labelTorrentFileName.AutoSize = true;
            this.labelTorrentFileName.Font = new System.Drawing.Font("MV Boli", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTorrentFileName.Location = new System.Drawing.Point(78, 40);
            this.labelTorrentFileName.Name = "labelTorrentFileName";
            this.labelTorrentFileName.Size = new System.Drawing.Size(0, 14);
            this.labelTorrentFileName.TabIndex = 28;
            // 
            // checkFiles
            // 
            this.checkFiles.FormattingEnabled = true;
            this.checkFiles.HorizontalScrollbar = true;
            this.checkFiles.Location = new System.Drawing.Point(12, 92);
            this.checkFiles.Name = "checkFiles";
            this.checkFiles.Size = new System.Drawing.Size(380, 139);
            this.checkFiles.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Download location:";
            // 
            // buttonChangeDestination
            // 
            this.buttonChangeDestination.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonChangeDestination.Enabled = false;
            this.buttonChangeDestination.Location = new System.Drawing.Point(364, 61);
            this.buttonChangeDestination.Name = "buttonChangeDestination";
            this.buttonChangeDestination.Size = new System.Drawing.Size(28, 20);
            this.buttonChangeDestination.TabIndex = 33;
            this.buttonChangeDestination.Text = "..";
            this.buttonChangeDestination.UseVisualStyleBackColor = false;
            this.buttonChangeDestination.Click += new System.EventHandler(this.buttonChangeDestination_Click);
            // 
            // textDestinationFolder
            // 
            this.textDestinationFolder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textDestinationFolder.Location = new System.Drawing.Point(108, 61);
            this.textDestinationFolder.Name = "textDestinationFolder";
            this.textDestinationFolder.ReadOnly = true;
            this.textDestinationFolder.Size = new System.Drawing.Size(250, 20);
            this.textDestinationFolder.TabIndex = 34;
            // 
            // progressDownload
            // 
            this.progressDownload.Location = new System.Drawing.Point(224, 237);
            this.progressDownload.Name = "progressDownload";
            this.progressDownload.Size = new System.Drawing.Size(100, 23);
            this.progressDownload.TabIndex = 35;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(588, 427);
            this.Controls.Add(this.progressDownload);
            this.Controls.Add(this.textDestinationFolder);
            this.Controls.Add(this.buttonChangeDestination);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkFiles);
            this.Controls.Add(this.labelTorrentFileName);
            this.Controls.Add(this.tableTrackers);
            this.Controls.Add(this.listPeers);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Torrent downloader";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableTrackers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTorrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog dialogOpenTorrentFile;
        private System.Windows.Forms.ListBox listPeers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView tableTrackers;
        private System.Windows.Forms.FolderBrowserDialog dialogDestinationFolder;
        private System.Windows.Forms.ToolStripMenuItem startDownloadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateTrackingInfoToolStripMenuItem;
        private System.Windows.Forms.Label labelTorrentFileName;
        private System.Windows.Forms.CheckedListBox checkFiles;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutThisProgramToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonChangeDestination;
        private System.Windows.Forms.TextBox textDestinationFolder;
        private System.Windows.Forms.ProgressBar progressDownload;
        private System.Windows.Forms.Timer timerDownloadProgress;
    }
}

