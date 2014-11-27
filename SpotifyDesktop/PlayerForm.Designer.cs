namespace SpotifyDesktop
{
    partial class PlayerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Artist = new System.Windows.Forms.Label();
            this.CoverArt = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CoverArt)).BeginInit();
            this.SuspendLayout();
            // 
            // Artist
            // 
            this.Artist.AutoSize = true;
            this.Artist.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Artist.ForeColor = System.Drawing.Color.White;
            this.Artist.Location = new System.Drawing.Point(151, 105);
            this.Artist.Name = "Artist";
            this.Artist.Size = new System.Drawing.Size(38, 17);
            this.Artist.TabIndex = 1;
            this.Artist.Text = "Artist";
            this.Artist.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            this.Artist.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            // 
            // CoverArt
            // 
            this.CoverArt.Location = new System.Drawing.Point(16, 12);
            this.CoverArt.Name = "CoverArt";
            this.CoverArt.Size = new System.Drawing.Size(128, 128);
            this.CoverArt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CoverArt.TabIndex = 2;
            this.CoverArt.TabStop = false;
            this.CoverArt.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            this.CoverArt.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(1)))), ((int)(((byte)(3)))));
            this.ClientSize = new System.Drawing.Size(728, 152);
            this.Controls.Add(this.CoverArt);
            this.Controls.Add(this.Artist);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(1)))), ((int)(((byte)(3)))));
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MoveWithMouse);
            ((System.ComponentModel.ISupportInitialize)(this.CoverArt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Artist;
        private System.Windows.Forms.PictureBox CoverArt;
    }
}

