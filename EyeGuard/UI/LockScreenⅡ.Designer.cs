namespace EyeGuard.UI
{
    partial class LockScreenⅡ
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
            this.components = new System.ComponentModel.Container();
            this.bufferGif = new System.Windows.Forms.PictureBox();
            this.Unlock = new System.Windows.Forms.PictureBox();
            this.PromptText = new System.Windows.Forms.Label();
            this.pbx_image = new System.Windows.Forms.PictureBox();
            this.timer_countdown = new System.Windows.Forms.Timer(this.components);
            this.timer_top = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bufferGif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Unlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbx_image)).BeginInit();
            this.SuspendLayout();
            // 
            // bufferGif
            // 
            this.bufferGif.BackColor = System.Drawing.Color.Transparent;
            this.bufferGif.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bufferGif.Location = new System.Drawing.Point(205, 97);
            this.bufferGif.Name = "bufferGif";
            this.bufferGif.Size = new System.Drawing.Size(128, 128);
            this.bufferGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bufferGif.TabIndex = 10;
            this.bufferGif.TabStop = false;
            // 
            // Unlock
            // 
            this.Unlock.BackColor = System.Drawing.Color.Transparent;
            this.Unlock.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Unlock.Location = new System.Drawing.Point(620, 264);
            this.Unlock.Name = "Unlock";
            this.Unlock.Size = new System.Drawing.Size(32, 32);
            this.Unlock.TabIndex = 9;
            this.Unlock.TabStop = false;
            this.Unlock.Visible = false;
            this.Unlock.Click += new System.EventHandler(this.Unlock_Click);
            // 
            // PromptText
            // 
            this.PromptText.AutoSize = true;
            this.PromptText.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PromptText.ForeColor = System.Drawing.Color.Honeydew;
            this.PromptText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PromptText.Location = new System.Drawing.Point(25, 273);
            this.PromptText.Name = "PromptText";
            this.PromptText.Size = new System.Drawing.Size(396, 35);
            this.PromptText.TabIndex = 11;
            this.PromptText.Text = "距离解锁时间还有:00秒";
            // 
            // pbx_image
            // 
            this.pbx_image.BackColor = System.Drawing.Color.Black;
            this.pbx_image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbx_image.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbx_image.Location = new System.Drawing.Point(0, 0);
            this.pbx_image.Name = "pbx_image";
            this.pbx_image.Size = new System.Drawing.Size(800, 450);
            this.pbx_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbx_image.TabIndex = 8;
            this.pbx_image.TabStop = false;
            this.pbx_image.WaitOnLoad = true;
            // 
            // timer_countdown
            // 
            this.timer_countdown.Interval = 1000;
            this.timer_countdown.Tick += new System.EventHandler(this.timer_countdown_Tick);
            // 
            // timer_top
            // 
            this.timer_top.Enabled = true;
            this.timer_top.Interval = 1000;
            this.timer_top.Tick += new System.EventHandler(this.timer_top_Tick);
            // 
            // LockScreenⅡ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bufferGif);
            this.Controls.Add(this.Unlock);
            this.Controls.Add(this.PromptText);
            this.Controls.Add(this.pbx_image);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LockScreenⅡ";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LockScreenⅡ";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LockScreenⅡ_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bufferGif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Unlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbx_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox bufferGif;
        private System.Windows.Forms.PictureBox Unlock;
        private System.Windows.Forms.Label PromptText;
        private System.Windows.Forms.PictureBox pbx_image;
        private System.Windows.Forms.Timer timer_countdown;
        private System.Windows.Forms.Timer timer_top;
    }
}