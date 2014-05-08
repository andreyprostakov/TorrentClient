namespace Torrent_client
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonLoadTorrent = new System.Windows.Forms.Button();
            this.textTorrentFileContent = new System.Windows.Forms.RichTextBox();
            this.labelError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonLoadTorrent
            // 
            this.buttonLoadTorrent.Location = new System.Drawing.Point(192, 24);
            this.buttonLoadTorrent.Name = "buttonLoadTorrent";
            this.buttonLoadTorrent.Size = new System.Drawing.Size(106, 32);
            this.buttonLoadTorrent.TabIndex = 0;
            this.buttonLoadTorrent.Text = "Load torrent file...";
            this.buttonLoadTorrent.UseVisualStyleBackColor = true;
            this.buttonLoadTorrent.Click += new System.EventHandler(this.buttonLoadTorrent_Click);
            // 
            // textTorrentFileContent
            // 
            this.textTorrentFileContent.Location = new System.Drawing.Point(12, 62);
            this.textTorrentFileContent.Name = "textTorrentFileContent";
            this.textTorrentFileContent.Size = new System.Drawing.Size(584, 389);
            this.textTorrentFileContent.TabIndex = 2;
            this.textTorrentFileContent.Text = "";
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelError.Location = new System.Drawing.Point(317, 34);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(28, 13);
            this.labelError.TabIndex = 3;
            this.labelError.Text = "error";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 463);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.textTorrentFileContent);
            this.Controls.Add(this.buttonLoadTorrent);
            this.Name = "Form1";
            this.Text = "pTorrent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonLoadTorrent;
        private System.Windows.Forms.RichTextBox textTorrentFileContent;
        private System.Windows.Forms.Label labelError;
    }
}

