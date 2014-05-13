namespace TorrentFileBrowser
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
            this.dialogOpenTorrentFile = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTorrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTorrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textRawContent = new System.Windows.Forms.RichTextBox();
            this.textParsedContent = new System.Windows.Forms.RichTextBox();
            this.buttonParse = new System.Windows.Forms.Button();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.labelFilename = new System.Windows.Forms.Label();
            this.textOutputContent = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dialogSaveTorrentFile = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogOpenTorrentFile
            // 
            this.dialogOpenTorrentFile.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.SkyBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1026, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTorrentFileToolStripMenuItem,
            this.saveAsTorrentFileToolStripMenuItem,
            this.compareFilesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fileToolStripMenuItem.Text = " File";
            // 
            // openTorrentFileToolStripMenuItem
            // 
            this.openTorrentFileToolStripMenuItem.Name = "openTorrentFileToolStripMenuItem";
            this.openTorrentFileToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.openTorrentFileToolStripMenuItem.Text = "Open torrent file..";
            this.openTorrentFileToolStripMenuItem.Click += new System.EventHandler(this.openTorrentFileToolStripMenuItem_Click);
            // 
            // saveAsTorrentFileToolStripMenuItem
            // 
            this.saveAsTorrentFileToolStripMenuItem.Name = "saveAsTorrentFileToolStripMenuItem";
            this.saveAsTorrentFileToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveAsTorrentFileToolStripMenuItem.Text = "Save as torrent file..";
            this.saveAsTorrentFileToolStripMenuItem.Click += new System.EventHandler(this.saveAsTorrentFileToolStripMenuItem_Click);
            // 
            // compareFilesToolStripMenuItem
            // 
            this.compareFilesToolStripMenuItem.Name = "compareFilesToolStripMenuItem";
            this.compareFilesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.compareFilesToolStripMenuItem.Text = "Compare files..";
            this.compareFilesToolStripMenuItem.Click += new System.EventHandler(this.compareFilesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Parsed content:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Input content:";
            // 
            // textRawContent
            // 
            this.textRawContent.Location = new System.Drawing.Point(12, 66);
            this.textRawContent.Name = "textRawContent";
            this.textRawContent.Size = new System.Drawing.Size(268, 350);
            this.textRawContent.TabIndex = 11;
            this.textRawContent.Text = "";
            // 
            // textParsedContent
            // 
            this.textParsedContent.Location = new System.Drawing.Point(298, 135);
            this.textParsedContent.Name = "textParsedContent";
            this.textParsedContent.Size = new System.Drawing.Size(421, 281);
            this.textParsedContent.TabIndex = 15;
            this.textParsedContent.Text = "";
            // 
            // buttonParse
            // 
            this.buttonParse.Location = new System.Drawing.Point(298, 64);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(149, 52);
            this.buttonParse.TabIndex = 16;
            this.buttonParse.Text = "Parse file";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.buttonParse_Click);
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.labelErrorMessage.Location = new System.Drawing.Point(453, 69);
            this.labelErrorMessage.MaximumSize = new System.Drawing.Size(220, 0);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(0, 13);
            this.labelErrorMessage.TabIndex = 17;
            this.labelErrorMessage.Visible = false;
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.ForeColor = System.Drawing.Color.Black;
            this.labelFilename.Location = new System.Drawing.Point(12, 33);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(0, 13);
            this.labelFilename.TabIndex = 18;
            // 
            // textOutputContent
            // 
            this.textOutputContent.Location = new System.Drawing.Point(746, 64);
            this.textOutputContent.Name = "textOutputContent";
            this.textOutputContent.Size = new System.Drawing.Size(268, 350);
            this.textOutputContent.TabIndex = 20;
            this.textOutputContent.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(746, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Output content:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1026, 428);
            this.Controls.Add(this.textOutputContent);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.buttonParse);
            this.Controls.Add(this.textParsedContent);
            this.Controls.Add(this.textRawContent);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Torrent Browser";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dialogOpenTorrentFile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTorrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox textRawContent;
        private System.Windows.Forms.RichTextBox textParsedContent;
        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.Label labelFilename;
        private System.Windows.Forms.RichTextBox textOutputContent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SaveFileDialog dialogSaveTorrentFile;
        private System.Windows.Forms.ToolStripMenuItem saveAsTorrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareFilesToolStripMenuItem;
    }
}

