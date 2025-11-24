namespace Ocean_Trip
{
	partial class FormSchedule
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchedule));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.scheduleGrid = new System.Windows.Forms.DataGridView();
			this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Route = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TimeOfDay = new System.Windows.Forms.DataGridViewImageColumn();
			this.Objectives = new System.Windows.Forms.DataGridViewImageColumn();
			this.Objective2 = new System.Windows.Forms.DataGridViewImageColumn();
			this.label4 = new System.Windows.Forms.Label();
			this.routeNameValue = new System.Windows.Forms.ComboBox();
			this.exitIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.exitIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = "Schedules";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(476, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(216, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "This is a work in progress.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Gainsboro;
			this.label3.Location = new System.Drawing.Point(14, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(171, 13);
			this.label3.TabIndex = 19;
			this.label3.Text = "Where would you like to go today?";
			// 
			// scheduleGrid
			// 
			this.scheduleGrid.AllowUserToAddRows = false;
			this.scheduleGrid.AllowUserToDeleteRows = false;
			this.scheduleGrid.AllowUserToResizeColumns = false;
			this.scheduleGrid.AllowUserToResizeRows = false;
			this.scheduleGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.scheduleGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.scheduleGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
			this.scheduleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.scheduleGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
			this.Date,
			this.Time,
			this.Route,
			this.TimeOfDay,
			this.Objectives,
			this.Objective2});
			this.scheduleGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.scheduleGrid.GridColor = System.Drawing.Color.Gainsboro;
			this.scheduleGrid.Location = new System.Drawing.Point(17, 112);
			this.scheduleGrid.MultiSelect = false;
			this.scheduleGrid.Name = "scheduleGrid";
			this.scheduleGrid.ReadOnly = true;
			this.scheduleGrid.RowHeadersVisible = false;
			this.scheduleGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(38)))), ((int)(((byte)(54)))));
			this.scheduleGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Gainsboro;
			this.scheduleGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(164)))), ((int)(((byte)(237)))));
			this.scheduleGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
			this.scheduleGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.scheduleGrid.Size = new System.Drawing.Size(701, 476);
			this.scheduleGrid.TabIndex = 20;
			// 
			// Date
			// 
			this.Date.HeaderText = "Date";
			this.Date.Name = "Date";
			this.Date.ReadOnly = true;
			// 
			// Time
			// 
			this.Time.HeaderText = "Route Start Time";
			this.Time.Name = "Time";
			this.Time.ReadOnly = true;
			// 
			// Route
			// 
			this.Route.HeaderText = "Route";
			this.Route.Name = "Route";
			this.Route.ReadOnly = true;
			// 
			// TimeOfDay
			// 
			this.TimeOfDay.HeaderText = "Time of Day";
			this.TimeOfDay.Name = "TimeOfDay";
			this.TimeOfDay.ReadOnly = true;
			// 
			// Objectives
			// 
			this.Objectives.HeaderText = "First Objective";
			this.Objectives.Name = "Objectives";
			this.Objectives.ReadOnly = true;
			// 
			// Objective2
			// 
			this.Objective2.HeaderText = "Second Objective";
			this.Objective2.Name = "Objective2";
			this.Objective2.ReadOnly = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.Gainsboro;
			this.label4.Location = new System.Drawing.Point(14, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(39, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Route:";
			// 
			// routeNameValue
			// 
			this.routeNameValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
			this.routeNameValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.routeNameValue.ForeColor = System.Drawing.Color.Gainsboro;
			this.routeNameValue.FormattingEnabled = true;
			this.routeNameValue.Items.AddRange(new object[] {
			"Indigo",
			"Ruby"});
			this.routeNameValue.Location = new System.Drawing.Point(59, 85);
			this.routeNameValue.Name = "routeNameValue";
			this.routeNameValue.Size = new System.Drawing.Size(75, 21);
			this.routeNameValue.TabIndex = 22;
			this.routeNameValue.Text = "Indigo";
			this.routeNameValue.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// exitIcon
			// 
			this.exitIcon.Image = ((System.Drawing.Image)(resources.GetObject("exitIcon.Image")));
			this.exitIcon.Location = new System.Drawing.Point(698, 12);
			this.exitIcon.Name = "exitIcon";
			this.exitIcon.Size = new System.Drawing.Size(20, 20);
			this.exitIcon.TabIndex = 18;
			this.exitIcon.TabStop = false;
			// 
			// FormSchedule
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
			this.ClientSize = new System.Drawing.Size(730, 600);
			this.Controls.Add(this.routeNameValue);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.scheduleGrid);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.exitIcon);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormSchedule";
			this.Text = "FormIdleActivities";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormSchedule_MouseDown);
			((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.exitIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox exitIcon;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGridView scheduleGrid;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox routeNameValue;
		private System.Windows.Forms.DataGridViewTextBoxColumn Date;
		private System.Windows.Forms.DataGridViewTextBoxColumn Time;
		private System.Windows.Forms.DataGridViewTextBoxColumn Route;
		private System.Windows.Forms.DataGridViewImageColumn TimeOfDay;
		private System.Windows.Forms.DataGridViewImageColumn Objectives;
		private System.Windows.Forms.DataGridViewImageColumn Objective2;
	}
}