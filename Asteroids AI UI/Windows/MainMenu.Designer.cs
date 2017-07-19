namespace AsteroidsAIUI.Windows
{
    partial class MainMenu
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
            this.addShipBtn = new System.Windows.Forms.Button();
            this.mainPbox = new System.Windows.Forms.PictureBox();
            this.frameTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mainPbox)).BeginInit();
            this.SuspendLayout();
            // 
            // addShipBtn
            // 
            this.addShipBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addShipBtn.Location = new System.Drawing.Point(665, 411);
            this.addShipBtn.Name = "addShipBtn";
            this.addShipBtn.Size = new System.Drawing.Size(146, 71);
            this.addShipBtn.TabIndex = 0;
            this.addShipBtn.Text = "Add Ship";
            this.addShipBtn.UseVisualStyleBackColor = true;
            this.addShipBtn.Visible = false;
            this.addShipBtn.Click += new System.EventHandler(this.addShipBtn_Click);
            // 
            // mainPbox
            // 
            this.mainPbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPbox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mainPbox.Location = new System.Drawing.Point(12, 12);
            this.mainPbox.Name = "mainPbox";
            this.mainPbox.Size = new System.Drawing.Size(797, 381);
            this.mainPbox.TabIndex = 1;
            this.mainPbox.TabStop = false;
            this.mainPbox.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPbox_Paint);
            // 
            // frameTimer
            // 
            this.frameTimer.Enabled = true;
            this.frameTimer.Interval = 1;
            this.frameTimer.Tick += new System.EventHandler(this.frameTimer_Tick);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 497);
            this.Controls.Add(this.mainPbox);
            this.Controls.Add(this.addShipBtn);
            this.MaximumSize = new System.Drawing.Size(837, 536);
            this.MinimumSize = new System.Drawing.Size(837, 536);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            ((System.ComponentModel.ISupportInitialize)(this.mainPbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addShipBtn;
        private System.Windows.Forms.PictureBox mainPbox;
        private System.Windows.Forms.Timer frameTimer;
    }
}