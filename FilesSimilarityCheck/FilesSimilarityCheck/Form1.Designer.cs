namespace FilesSimilarityCheck
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
            this.textFile1 = new System.Windows.Forms.RichTextBox();
            this.buttonLoad1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonLoad2 = new System.Windows.Forms.Button();
            this.textFile2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textFile1
            // 
            this.textFile1.Location = new System.Drawing.Point(12, 71);
            this.textFile1.Name = "textFile1";
            this.textFile1.Size = new System.Drawing.Size(241, 309);
            this.textFile1.TabIndex = 0;
            this.textFile1.Text = "";
            // 
            // buttonLoad1
            // 
            this.buttonLoad1.Location = new System.Drawing.Point(12, 16);
            this.buttonLoad1.Name = "buttonLoad1";
            this.buttonLoad1.Size = new System.Drawing.Size(100, 23);
            this.buttonLoad1.TabIndex = 1;
            this.buttonLoad1.Text = "Load";
            this.buttonLoad1.UseVisualStyleBackColor = true;
            this.buttonLoad1.Click += new System.EventHandler(this.buttonLoad1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonLoad2
            // 
            this.buttonLoad2.Location = new System.Drawing.Point(274, 16);
            this.buttonLoad2.Name = "buttonLoad2";
            this.buttonLoad2.Size = new System.Drawing.Size(100, 23);
            this.buttonLoad2.TabIndex = 3;
            this.buttonLoad2.Text = "Load";
            this.buttonLoad2.UseVisualStyleBackColor = true;
            this.buttonLoad2.Click += new System.EventHandler(this.buttonLoad2_Click);
            // 
            // textFile2
            // 
            this.textFile2.Location = new System.Drawing.Point(274, 71);
            this.textFile2.Name = "textFile2";
            this.textFile2.Size = new System.Drawing.Size(241, 309);
            this.textFile2.TabIndex = 2;
            this.textFile2.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 403);
            this.Controls.Add(this.buttonLoad2);
            this.Controls.Add(this.textFile2);
            this.Controls.Add(this.buttonLoad1);
            this.Controls.Add(this.textFile1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textFile1;
        private System.Windows.Forms.Button buttonLoad1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonLoad2;
        private System.Windows.Forms.RichTextBox textFile2;
    }
}

