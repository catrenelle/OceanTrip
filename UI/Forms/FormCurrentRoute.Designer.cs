namespace Ocean_Trip
{
	partial class FormCurrentRoute
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCurrentRoute));
			label1 = new System.Windows.Forms.Label();
			exitIcon = new System.Windows.Forms.PictureBox();
			r1title = new System.Windows.Forms.Label();
			groupBox5 = new System.Windows.Forms.GroupBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			currentFish = new System.Windows.Forms.DataGridView();
			Icon = new System.Windows.Forms.DataGridViewImageColumn();
			FishName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			Spectral = new System.Windows.Forms.DataGridViewTextBoxColumn();
			BiteTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
			AveragePoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DHBonus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			THBonus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			Available = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)exitIcon).BeginInit();
			groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)currentFish).BeginInit();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
			label1.ForeColor = System.Drawing.Color.White;
			label1.Location = new System.Drawing.Point(14, 10);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(159, 25);
			label1.TabIndex = 0;
			label1.Text = "Current Route";
			// 
			// exitIcon
			// 
			exitIcon.Image = (System.Drawing.Image)resources.GetObject("exitIcon.Image");
			exitIcon.Location = new System.Drawing.Point(814, 14);
			exitIcon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			exitIcon.Name = "exitIcon";
			exitIcon.Size = new System.Drawing.Size(23, 23);
			exitIcon.TabIndex = 19;
			exitIcon.TabStop = false;
			// 
			// r1title
			// 
			r1title.AutoSize = true;
			r1title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			r1title.ForeColor = System.Drawing.Color.Gainsboro;
			r1title.Location = new System.Drawing.Point(15, 67);
			r1title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			r1title.Name = "r1title";
			r1title.Size = new System.Drawing.Size(153, 20);
			r1title.TabIndex = 20;
			r1title.Text = "Route Name Here";
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(currentFish);
			groupBox5.ForeColor = System.Drawing.Color.Gainsboro;
			groupBox5.Location = new System.Drawing.Point(15, 105);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new System.Drawing.Size(822, 300);
			groupBox5.TabIndex = 33;
			groupBox5.TabStop = false;
			groupBox5.Text = "Fish List";
			// 
			// groupBox3
			// 
			groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
			groupBox3.Location = new System.Drawing.Point(15, 408);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(822, 240);
			groupBox3.TabIndex = 31;
			groupBox3.TabStop = false;
			groupBox3.Text = "Double Hook / Triple Hook Rules";
			// 
			// currentFish
			// 
			currentFish.AllowUserToAddRows = false;
			currentFish.AllowUserToDeleteRows = false;
			currentFish.AllowUserToResizeColumns = false;
			currentFish.AllowUserToResizeRows = false;
			currentFish.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			currentFish.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			currentFish.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 44);
			currentFish.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			currentFish.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Icon, FishName, Spectral, BiteTimer, AveragePoints, DHBonus, THBonus, Available });
			currentFish.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			currentFish.GridColor = System.Drawing.Color.Gainsboro;
			currentFish.Location = new System.Drawing.Point(6, 12);
			currentFish.MultiSelect = false;
			currentFish.Name = "currentFish";
			currentFish.ReadOnly = true;
			currentFish.RowHeadersVisible = false;
			currentFish.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(36, 38, 54);
			currentFish.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Gainsboro;
			currentFish.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 164, 237);
			currentFish.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
			currentFish.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			currentFish.Size = new System.Drawing.Size(810, 282);
			currentFish.TabIndex = 0;
			// 
			// Icon
			// 
			Icon.HeaderText = "Icon";
			Icon.Name = "Icon";
			Icon.ReadOnly = true;
			// 
			// FishName
			// 
			FishName.HeaderText = "Fish Name";
			FishName.Name = "FishName";
			FishName.ReadOnly = true;
			// 
			// Spectral
			// 
			Spectral.HeaderText = "Spectral";
			Spectral.Name = "Spectral";
			Spectral.ReadOnly = true;
			// 
			// BiteTimer
			// 
			BiteTimer.HeaderText = "Bite Timer";
			BiteTimer.Name = "BiteTimer";
			BiteTimer.ReadOnly = true;
			// 
			// AveragePoints
			// 
			AveragePoints.HeaderText = "Avg Points";
			AveragePoints.Name = "AveragePoints";
			AveragePoints.ReadOnly = true;
			// 
			// DHBonus
			// 
			DHBonus.HeaderText = "DH Bonus";
			DHBonus.Name = "DHBonus";
			DHBonus.ReadOnly = true;
			// 
			// THBonus
			// 
			THBonus.HeaderText = "TH Bonus";
			THBonus.Name = "THBonus";
			THBonus.ReadOnly = true;
			// 
			// Available
			// 
			Available.HeaderText = "Available?";
			Available.Name = "Available";
			Available.ReadOnly = true;
			// 
			// FormCurrentRoute
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.FromArgb(30, 30, 44);
			ClientSize = new System.Drawing.Size(852, 660);
			Controls.Add(groupBox5);
			Controls.Add(groupBox3);
			Controls.Add(r1title);
			Controls.Add(exitIcon);
			Controls.Add(label1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "FormCurrentRoute";
			Text = "7777";
			MouseDown += FormCurrentRoute_MouseDown;
			((System.ComponentModel.ISupportInitialize)exitIcon).EndInit();
			groupBox5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)currentFish).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox exitIcon;
		private System.Windows.Forms.Label r1title;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.DataGridView currentFish;
		private new System.Windows.Forms.DataGridViewImageColumn Icon;
		private System.Windows.Forms.DataGridViewTextBoxColumn FishName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Spectral;
		private System.Windows.Forms.DataGridViewTextBoxColumn BiteTimer;
		private System.Windows.Forms.DataGridViewTextBoxColumn AveragePoints;
		private System.Windows.Forms.DataGridViewTextBoxColumn DHBonus;
		private System.Windows.Forms.DataGridViewTextBoxColumn THBonus;
		private System.Windows.Forms.DataGridViewTextBoxColumn Available;
	}
}