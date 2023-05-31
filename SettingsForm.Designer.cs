namespace OceanTripPlanner
{
    partial class SettingsForm
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Settings = new System.Windows.Forms.TabPage();
            this.routeInformationTab = new System.Windows.Forms.TabPage();
            this.refreshButton = new System.Windows.Forms.Button();
            this.toolsTab = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.refreshMissingFishButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.route1 = new System.Windows.Forms.TabPage();
            this.route2 = new System.Windows.Forms.TabPage();
            this.route3 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.routeNameValue = new System.Windows.Forms.Label();
            this.scheduleGrid = new System.Windows.Forms.DataGridView();
            this.Day = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Route = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RouteTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Objectives = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl.SuspendLayout();
            this.Settings.SuspendLayout();
            this.routeInformationTab.SuspendLayout();
            this.toolsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(372, 444);
            this.propertyGrid1.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.Settings);
            this.tabControl.Controls.Add(this.routeInformationTab);
            this.tabControl.Controls.Add(this.route1);
            this.tabControl.Controls.Add(this.route2);
            this.tabControl.Controls.Add(this.route3);
            this.tabControl.Controls.Add(this.toolsTab);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(386, 476);
            this.tabControl.TabIndex = 2;
            // 
            // Settings
            // 
            this.Settings.Controls.Add(this.propertyGrid1);
            this.Settings.Location = new System.Drawing.Point(4, 22);
            this.Settings.Name = "Settings";
            this.Settings.Padding = new System.Windows.Forms.Padding(3);
            this.Settings.Size = new System.Drawing.Size(378, 450);
            this.Settings.TabIndex = 0;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // routeInformationTab
            // 
            this.routeInformationTab.Controls.Add(this.scheduleGrid);
            this.routeInformationTab.Controls.Add(this.routeNameValue);
            this.routeInformationTab.Controls.Add(this.label3);
            this.routeInformationTab.Controls.Add(this.refreshButton);
            this.routeInformationTab.Location = new System.Drawing.Point(4, 22);
            this.routeInformationTab.Name = "routeInformationTab";
            this.routeInformationTab.Padding = new System.Windows.Forms.Padding(3);
            this.routeInformationTab.Size = new System.Drawing.Size(378, 450);
            this.routeInformationTab.TabIndex = 1;
            this.routeInformationTab.Text = "Schedule";
            this.routeInformationTab.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(295, 6);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 7;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshRouteInformation);
            // 
            // toolsTab
            // 
            this.toolsTab.Controls.Add(this.label2);
            this.toolsTab.Controls.Add(this.label1);
            this.toolsTab.Controls.Add(this.refreshMissingFishButton);
            this.toolsTab.Controls.Add(this.button1);
            this.toolsTab.Location = new System.Drawing.Point(4, 22);
            this.toolsTab.Name = "toolsTab";
            this.toolsTab.Size = new System.Drawing.Size(378, 450);
            this.toolsTab.TabIndex = 2;
            this.toolsTab.Text = "Tools";
            this.toolsTab.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(190, 65);
            this.label2.TabIndex = 5;
            this.label2.Text = "Refresh the Missing Fish cache. This \r\ncan be useful if you caught ocean fish \r\no" +
    "utside of using Ocean Trip. This will \r\nrequire you to restart the BotBase to\r\nt" +
    "ake effect.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Open Lisbeth BotBase Panel to manage \r\nrecipes";
            // 
            // refreshMissingFishButton
            // 
            this.refreshMissingFishButton.Location = new System.Drawing.Point(27, 115);
            this.refreshMissingFishButton.Name = "refreshMissingFishButton";
            this.refreshMissingFishButton.Size = new System.Drawing.Size(98, 25);
            this.refreshMissingFishButton.TabIndex = 3;
            this.refreshMissingFishButton.Text = "Refresh Cache";
            this.refreshMissingFishButton.UseVisualStyleBackColor = true;
            this.refreshMissingFishButton.Click += new System.EventHandler(this.refreshMissingFishCache_Button);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Lisbeth";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Lisbeth_Button);
            // 
            // route1
            // 
            this.route1.Location = new System.Drawing.Point(4, 22);
            this.route1.Name = "route1";
            this.route1.Size = new System.Drawing.Size(350, 450);
            this.route1.TabIndex = 3;
            this.route1.Text = "Route1";
            this.route1.UseVisualStyleBackColor = true;
            // 
            // route2
            // 
            this.route2.Location = new System.Drawing.Point(4, 22);
            this.route2.Name = "route2";
            this.route2.Size = new System.Drawing.Size(350, 450);
            this.route2.TabIndex = 4;
            this.route2.Text = "Route 2";
            this.route2.UseVisualStyleBackColor = true;
            // 
            // route3
            // 
            this.route3.Location = new System.Drawing.Point(4, 22);
            this.route3.Name = "route3";
            this.route3.Size = new System.Drawing.Size(350, 450);
            this.route3.TabIndex = 5;
            this.route3.Text = "Route 3";
            this.route3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Route:";
            // 
            // routeNameValue
            // 
            this.routeNameValue.AutoSize = true;
            this.routeNameValue.Location = new System.Drawing.Point(48, 11);
            this.routeNameValue.Name = "routeNameValue";
            this.routeNameValue.Size = new System.Drawing.Size(97, 13);
            this.routeNameValue.TabIndex = 16;
            this.routeNameValue.Text = "Route Name Value";
            // 
            // scheduleGrid
            // 
            this.scheduleGrid.AllowUserToAddRows = false;
            this.scheduleGrid.AllowUserToDeleteRows = false;
            this.scheduleGrid.AllowUserToResizeColumns = false;
            this.scheduleGrid.AllowUserToResizeRows = false;
            this.scheduleGrid.CausesValidation = false;
            this.scheduleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scheduleGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Day,
            this.Time,
            this.Route,
            this.RouteTime,
            this.Objectives});
            this.scheduleGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.scheduleGrid.Location = new System.Drawing.Point(0, 32);
            this.scheduleGrid.MultiSelect = false;
            this.scheduleGrid.Name = "scheduleGrid";
            this.scheduleGrid.ReadOnly = true;
            this.scheduleGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.scheduleGrid.RowHeadersWidth = 10;
            this.scheduleGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.scheduleGrid.ShowCellErrors = false;
            this.scheduleGrid.ShowEditingIcon = false;
            this.scheduleGrid.ShowRowErrors = false;
            this.scheduleGrid.Size = new System.Drawing.Size(380, 418);
            this.scheduleGrid.TabIndex = 17;
            // 
            // Day
            // 
            this.Day.HeaderText = "";
            this.Day.Name = "Day";
            this.Day.ReadOnly = true;
            this.Day.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Day.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Day.Width = 40;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Time.Width = 60;
            // 
            // Route
            // 
            this.Route.HeaderText = "Route";
            this.Route.Name = "Route";
            this.Route.ReadOnly = true;
            this.Route.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Route.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RouteTime
            // 
            this.RouteTime.HeaderText = "";
            this.RouteTime.Name = "RouteTime";
            this.RouteTime.ReadOnly = true;
            this.RouteTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RouteTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RouteTime.Width = 55;
            // 
            // Objectives
            // 
            this.Objectives.HeaderText = "Objectives";
            this.Objectives.Name = "Objectives";
            this.Objectives.ReadOnly = true;
            this.Objectives.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Objectives.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Objectives.Width = 111;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 476);
            this.Controls.Add(this.tabControl);
            this.Name = "SettingsForm";
            this.Text = "Ocean Trip Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.tabControl.ResumeLayout(false);
            this.Settings.ResumeLayout(false);
            this.routeInformationTab.ResumeLayout(false);
            this.routeInformationTab.PerformLayout();
            this.toolsTab.ResumeLayout(false);
            this.toolsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage Settings;
		private System.Windows.Forms.TabPage routeInformationTab;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.TabPage toolsTab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button refreshMissingFishButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage route1;
        private System.Windows.Forms.TabPage route2;
        private System.Windows.Forms.TabPage route3;
        private System.Windows.Forms.Label routeNameValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView scheduleGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Day;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Route;
        private System.Windows.Forms.DataGridViewTextBoxColumn RouteTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Objectives;
    }
}