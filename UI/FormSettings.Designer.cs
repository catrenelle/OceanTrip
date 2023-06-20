using System;
using System.Drawing;

namespace Ocean_Trip
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.LlamaMarketButton = new System.Windows.Forms.Button();
            this.LisbethButton = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.buttonCurrentRoute = new System.Windows.Forms.Button();
            this.buttonSchedule = new System.Windows.Forms.Button();
            this.buttonBoatSettings = new System.Windows.Forms.Button();
            this.buttonIdleStuff = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.verboseLoggingToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.panelMenu.SuspendLayout();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(38)))), ((int)(((byte)(54)))));
            this.panelMenu.Controls.Add(this.label1);
            this.panelMenu.Controls.Add(this.verboseLoggingToggle);
            this.panelMenu.Controls.Add(this.pictureBox3);
            this.panelMenu.Controls.Add(this.pictureBox2);
            this.panelMenu.Controls.Add(this.LisbethButton);
            this.panelMenu.Controls.Add(this.LlamaMarketButton);
            this.panelMenu.Controls.Add(this.buttonCurrentRoute);
            this.panelMenu.Controls.Add(this.buttonSchedule);
            this.panelMenu.Controls.Add(this.buttonBoatSettings);
            this.panelMenu.Controls.Add(this.buttonIdleStuff);
            this.panelMenu.Controls.Add(this.panelLogo);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(175, 600);
            this.panelMenu.TabIndex = 0;
            this.panelMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(38)))), ((int)(((byte)(54)))));
            this.panelLogo.Controls.Add(this.pictureBox1);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(175, 50);
            this.panelLogo.TabIndex = 0;
            this.panelLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(175, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(730, 600);
            this.panelMain.TabIndex = 1;
            this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            // 
            // LlamaMarketButton
            // 
            this.LlamaMarketButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LlamaMarketButton.FlatAppearance.BorderSize = 0;
            this.LlamaMarketButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LlamaMarketButton.ForeColor = System.Drawing.Color.Gainsboro;
            this.LlamaMarketButton.Location = new System.Drawing.Point(0, 550);
            this.LlamaMarketButton.Name = "LlamaMarketButton";
            this.LlamaMarketButton.Size = new System.Drawing.Size(175, 50);
            this.LlamaMarketButton.TabIndex = 5;
            this.LlamaMarketButton.Text = "Llama Market";
            this.LlamaMarketButton.UseVisualStyleBackColor = true;
            this.LlamaMarketButton.Click += new System.EventHandler(this.LlamaMarketButton_Click);
            // 
            // LisbethButton
            // 
            this.LisbethButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.LisbethButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LisbethButton.FlatAppearance.BorderSize = 0;
            this.LisbethButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LisbethButton.ForeColor = System.Drawing.Color.Gainsboro;
            this.LisbethButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LisbethButton.Location = new System.Drawing.Point(0, 500);
            this.LisbethButton.Margin = new System.Windows.Forms.Padding(0);
            this.LisbethButton.Name = "LisbethButton";
            this.LisbethButton.Size = new System.Drawing.Size(175, 50);
            this.LisbethButton.TabIndex = 6;
            this.LisbethButton.Text = "Lisbeth";
            this.LisbethButton.UseVisualStyleBackColor = true;
            this.LisbethButton.Click += new System.EventHandler(this.LisbethButton_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Ocean_Trip.Properties.Resources.LlamaMarket_Logo;
            this.pictureBox3.Location = new System.Drawing.Point(-4, 550);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(50, 50);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Ocean_Trip.Properties.Resources.Lisbeth;
            this.pictureBox2.Location = new System.Drawing.Point(0, 500);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(46, 50);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // buttonCurrentRoute
            // 
            this.buttonCurrentRoute.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonCurrentRoute.FlatAppearance.BorderSize = 0;
            this.buttonCurrentRoute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCurrentRoute.ForeColor = System.Drawing.Color.Gainsboro;
            this.buttonCurrentRoute.Image = ((System.Drawing.Image)(resources.GetObject("buttonCurrentRoute.Image")));
            this.buttonCurrentRoute.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCurrentRoute.Location = new System.Drawing.Point(0, 200);
            this.buttonCurrentRoute.Name = "buttonCurrentRoute";
            this.buttonCurrentRoute.Size = new System.Drawing.Size(175, 50);
            this.buttonCurrentRoute.TabIndex = 4;
            this.buttonCurrentRoute.Text = "Current Route";
            this.buttonCurrentRoute.UseVisualStyleBackColor = true;
            this.buttonCurrentRoute.Click += new System.EventHandler(this.buttonCurrentRoute_Click);
            // 
            // buttonSchedule
            // 
            this.buttonSchedule.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSchedule.FlatAppearance.BorderSize = 0;
            this.buttonSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSchedule.ForeColor = System.Drawing.Color.Gainsboro;
            this.buttonSchedule.Image = ((System.Drawing.Image)(resources.GetObject("buttonSchedule.Image")));
            this.buttonSchedule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSchedule.Location = new System.Drawing.Point(0, 150);
            this.buttonSchedule.Name = "buttonSchedule";
            this.buttonSchedule.Size = new System.Drawing.Size(175, 50);
            this.buttonSchedule.TabIndex = 3;
            this.buttonSchedule.Text = "Schedule";
            this.buttonSchedule.UseVisualStyleBackColor = true;
            this.buttonSchedule.Click += new System.EventHandler(this.buttonSchedule_Click);
            // 
            // buttonBoatSettings
            // 
            this.buttonBoatSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonBoatSettings.FlatAppearance.BorderSize = 0;
            this.buttonBoatSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBoatSettings.ForeColor = System.Drawing.Color.Gainsboro;
            this.buttonBoatSettings.Image = ((System.Drawing.Image)(resources.GetObject("buttonBoatSettings.Image")));
            this.buttonBoatSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonBoatSettings.Location = new System.Drawing.Point(0, 100);
            this.buttonBoatSettings.Name = "buttonBoatSettings";
            this.buttonBoatSettings.Size = new System.Drawing.Size(175, 50);
            this.buttonBoatSettings.TabIndex = 2;
            this.buttonBoatSettings.Text = "Ocean Settings";
            this.buttonBoatSettings.UseVisualStyleBackColor = true;
            this.buttonBoatSettings.Click += new System.EventHandler(this.buttonBoatSettings_Click);
            // 
            // buttonIdleStuff
            // 
            this.buttonIdleStuff.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonIdleStuff.FlatAppearance.BorderSize = 0;
            this.buttonIdleStuff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonIdleStuff.ForeColor = System.Drawing.Color.Gainsboro;
            this.buttonIdleStuff.Image = ((System.Drawing.Image)(resources.GetObject("buttonIdleStuff.Image")));
            this.buttonIdleStuff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonIdleStuff.Location = new System.Drawing.Point(0, 50);
            this.buttonIdleStuff.Name = "buttonIdleStuff";
            this.buttonIdleStuff.Size = new System.Drawing.Size(175, 50);
            this.buttonIdleStuff.TabIndex = 1;
            this.buttonIdleStuff.Text = "Idle Activities";
            this.buttonIdleStuff.UseVisualStyleBackColor = true;
            this.buttonIdleStuff.Click += new System.EventHandler(this.buttonIdleStuff_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Ocean_Trip.Properties.Resources.OceanTripLogo;
            this.pictureBox1.Location = new System.Drawing.Point(8, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(157, 30);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(36, 467);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Verbose Logging";
            // 
            // verboseLoggingToggle
            // 
            this.verboseLoggingToggle.AutoSize = true;
            this.verboseLoggingToggle.Location = new System.Drawing.Point(8, 466);
            this.verboseLoggingToggle.MinimumSize = new System.Drawing.Size(22, 10);
            this.verboseLoggingToggle.Name = "verboseLoggingToggle";
            this.verboseLoggingToggle.OffBackColor = System.Drawing.Color.Gray;
            this.verboseLoggingToggle.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.verboseLoggingToggle.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.verboseLoggingToggle.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.verboseLoggingToggle.Size = new System.Drawing.Size(22, 14);
            this.verboseLoggingToggle.TabIndex = 9;
            this.verboseLoggingToggle.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(905, 600);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowIcon = false;
            this.Text = "BotBase Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormSettings_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            this.panelMenu.ResumeLayout(false);
            this.panelMenu.PerformLayout();
            this.panelLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Button buttonIdleStuff;
        private System.Windows.Forms.Button buttonBoatSettings;
        private System.Windows.Forms.Button buttonCurrentRoute;
        private System.Windows.Forms.Button buttonSchedule;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button LisbethButton;
        private System.Windows.Forms.Button LlamaMarketButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label1;
        private UI.Controls.ToggleButton verboseLoggingToggle;
    }
}