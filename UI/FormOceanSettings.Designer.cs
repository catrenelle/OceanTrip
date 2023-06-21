namespace Ocean_Trip
{
    partial class FormOceanSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.assistedFishingLabel = new System.Windows.Forms.Label();
            this.fishingPriorityGroupBox = new System.Windows.Forms.GroupBox();
            this.fishingRoute = new System.Windows.Forms.GroupBox();
            this.lateQueue = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fullGPAction = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericRestockAmount = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericRestockThreshold = new System.Windows.Forms.NumericUpDown();
            this.fishFoodToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.lateQueueToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.ignoreBoat = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.achievements = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.fishingLog = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.points = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.automatic = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.assistedFishingToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.indigo = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.ruby = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.GPActionNothing = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.GPActionDoubleHook = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.GPActionChum = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.patienceDefaultLogic = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.patienceSpectralOnly = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.patienceAlways = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.fishExchangeNone = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.fishExchangeDesynthesize = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.fishExchangeSell = new Ocean_Trip.UI.Controls.RadioButtonFlat();
            this.groupBox3.SuspendLayout();
            this.fishingPriorityGroupBox.SuspendLayout();
            this.fishingRoute.SuspendLayout();
            this.lateQueue.SuspendLayout();
            this.fullGPAction.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ocean Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 548);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(216, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "This is a work in progress.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gainsboro;
            this.label3.Location = new System.Drawing.Point(14, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(299, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "What settings would you like to apply to your ocean voyages?";
            // 
            // groupBox1
            // 
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(581, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(125, 211);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Indigo Acheivements";
            this.groupBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox2.Location = new System.Drawing.Point(581, 296);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 211);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ruby Acheivements";
            this.groupBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.assistedFishingLabel);
            this.groupBox3.Controls.Add(this.assistedFishingToggle);
            this.groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox3.Location = new System.Drawing.Point(581, 520);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 48);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Open World Fishing";
            this.groupBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // assistedFishingLabel
            // 
            this.assistedFishingLabel.AutoSize = true;
            this.assistedFishingLabel.Location = new System.Drawing.Point(34, 20);
            this.assistedFishingLabel.Name = "assistedFishingLabel";
            this.assistedFishingLabel.Size = new System.Drawing.Size(82, 13);
            this.assistedFishingLabel.TabIndex = 1;
            this.assistedFishingLabel.Text = "Assisted Fishing";
            // 
            // fishingPriorityGroupBox
            // 
            this.fishingPriorityGroupBox.Controls.Add(this.ignoreBoat);
            this.fishingPriorityGroupBox.Controls.Add(this.achievements);
            this.fishingPriorityGroupBox.Controls.Add(this.fishingLog);
            this.fishingPriorityGroupBox.Controls.Add(this.points);
            this.fishingPriorityGroupBox.Controls.Add(this.automatic);
            this.fishingPriorityGroupBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.fishingPriorityGroupBox.Location = new System.Drawing.Point(17, 79);
            this.fishingPriorityGroupBox.Name = "fishingPriorityGroupBox";
            this.fishingPriorityGroupBox.Size = new System.Drawing.Size(558, 49);
            this.fishingPriorityGroupBox.TabIndex = 9;
            this.fishingPriorityGroupBox.TabStop = false;
            this.fishingPriorityGroupBox.Text = "Fishing Priority";
            this.fishingPriorityGroupBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // fishingRoute
            // 
            this.fishingRoute.Controls.Add(this.ruby);
            this.fishingRoute.Controls.Add(this.indigo);
            this.fishingRoute.ForeColor = System.Drawing.Color.Gainsboro;
            this.fishingRoute.Location = new System.Drawing.Point(17, 134);
            this.fishingRoute.Name = "fishingRoute";
            this.fishingRoute.Size = new System.Drawing.Size(212, 48);
            this.fishingRoute.TabIndex = 10;
            this.fishingRoute.TabStop = false;
            this.fishingRoute.Text = "Fishing Route";
            this.fishingRoute.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // lateQueue
            // 
            this.lateQueue.Controls.Add(this.label5);
            this.lateQueue.Controls.Add(this.label4);
            this.lateQueue.Controls.Add(this.fishFoodToggle);
            this.lateQueue.Controls.Add(this.lateQueueToggle);
            this.lateQueue.ForeColor = System.Drawing.Color.Gainsboro;
            this.lateQueue.Location = new System.Drawing.Point(17, 188);
            this.lateQueue.Name = "lateQueue";
            this.lateQueue.Size = new System.Drawing.Size(212, 48);
            this.lateQueue.TabIndex = 11;
            this.lateQueue.TabStop = false;
            this.lateQueue.Text = "Trip Options";
            this.lateQueue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(144, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Crab Cakes";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Late Queue";
            // 
            // fullGPAction
            // 
            this.fullGPAction.Controls.Add(this.GPActionChum);
            this.fullGPAction.Controls.Add(this.GPActionDoubleHook);
            this.fullGPAction.Controls.Add(this.GPActionNothing);
            this.fullGPAction.ForeColor = System.Drawing.Color.Gainsboro;
            this.fullGPAction.Location = new System.Drawing.Point(235, 134);
            this.fullGPAction.Name = "fullGPAction";
            this.fullGPAction.Size = new System.Drawing.Size(340, 48);
            this.fullGPAction.TabIndex = 12;
            this.fullGPAction.TabStop = false;
            this.fullGPAction.Text = "Full GP Action";
            this.fullGPAction.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.patienceAlways);
            this.groupBox5.Controls.Add(this.patienceSpectralOnly);
            this.groupBox5.Controls.Add(this.patienceDefaultLogic);
            this.groupBox5.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox5.Location = new System.Drawing.Point(235, 188);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(340, 48);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Use Patience Skill";
            this.groupBox5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.fishExchangeSell);
            this.groupBox4.Controls.Add(this.fishExchangeDesynthesize);
            this.groupBox4.Controls.Add(this.fishExchangeNone);
            this.groupBox4.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox4.Location = new System.Drawing.Point(235, 242);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(340, 48);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fish Exchange";
            this.groupBox4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.numericRestockAmount);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.numericRestockThreshold);
            this.groupBox6.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox6.Location = new System.Drawing.Point(17, 296);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(558, 211);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tackle Box";
            this.groupBox6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            // 
            // numericRestockAmount
            // 
            this.numericRestockAmount.Location = new System.Drawing.Point(458, 185);
            this.numericRestockAmount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericRestockAmount.Name = "numericRestockAmount";
            this.numericRestockAmount.Size = new System.Drawing.Size(94, 20);
            this.numericRestockAmount.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(363, 187);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Restock Amount:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Restock Threshold:";
            // 
            // numericRestockThreshold
            // 
            this.numericRestockThreshold.Location = new System.Drawing.Point(112, 185);
            this.numericRestockThreshold.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericRestockThreshold.Name = "numericRestockThreshold";
            this.numericRestockThreshold.Size = new System.Drawing.Size(94, 20);
            this.numericRestockThreshold.TabIndex = 0;
            // 
            // fishFoodToggle
            // 
            this.fishFoodToggle.AutoSize = true;
            this.fishFoodToggle.Location = new System.Drawing.Point(116, 19);
            this.fishFoodToggle.MinimumSize = new System.Drawing.Size(22, 10);
            this.fishFoodToggle.Name = "fishFoodToggle";
            this.fishFoodToggle.OffBackColor = System.Drawing.Color.Gray;
            this.fishFoodToggle.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.fishFoodToggle.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.fishFoodToggle.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.fishFoodToggle.Size = new System.Drawing.Size(22, 14);
            this.fishFoodToggle.TabIndex = 1;
            this.fishFoodToggle.UseVisualStyleBackColor = true;
            // 
            // lateQueueToggle
            // 
            this.lateQueueToggle.AutoSize = true;
            this.lateQueueToggle.Location = new System.Drawing.Point(7, 20);
            this.lateQueueToggle.MinimumSize = new System.Drawing.Size(22, 10);
            this.lateQueueToggle.Name = "lateQueueToggle";
            this.lateQueueToggle.OffBackColor = System.Drawing.Color.Gray;
            this.lateQueueToggle.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.lateQueueToggle.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.lateQueueToggle.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.lateQueueToggle.Size = new System.Drawing.Size(22, 14);
            this.lateQueueToggle.TabIndex = 0;
            this.lateQueueToggle.UseVisualStyleBackColor = true;
            // 
            // ignoreBoat
            // 
            this.ignoreBoat.AutoSize = true;
            this.ignoreBoat.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.ignoreBoat.DisabledColor = System.Drawing.Color.SlateGray;
            this.ignoreBoat.Location = new System.Drawing.Point(464, 18);
            this.ignoreBoat.MinimumSize = new System.Drawing.Size(0, 15);
            this.ignoreBoat.Name = "ignoreBoat";
            this.ignoreBoat.Size = new System.Drawing.Size(92, 17);
            this.ignoreBoat.TabIndex = 6;
            this.ignoreBoat.TabStop = true;
            this.ignoreBoat.Text = "Ignore Boat";
            this.ignoreBoat.UnCheckedColor = System.Drawing.Color.Gray;
            this.ignoreBoat.UseVisualStyleBackColor = true;
            // 
            // achievements
            // 
            this.achievements.AutoSize = true;
            this.achievements.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.achievements.DisabledColor = System.Drawing.Color.SlateGray;
            this.achievements.Enabled = false;
            this.achievements.Location = new System.Drawing.Point(330, 18);
            this.achievements.MinimumSize = new System.Drawing.Size(0, 15);
            this.achievements.Name = "achievements";
            this.achievements.Size = new System.Drawing.Size(104, 17);
            this.achievements.TabIndex = 5;
            this.achievements.TabStop = true;
            this.achievements.Text = "Achievements";
            this.achievements.UnCheckedColor = System.Drawing.Color.Gray;
            this.achievements.UseVisualStyleBackColor = true;
            // 
            // fishingLog
            // 
            this.fishingLog.AutoSize = true;
            this.fishingLog.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.fishingLog.DisabledColor = System.Drawing.Color.SlateGray;
            this.fishingLog.Location = new System.Drawing.Point(225, 18);
            this.fishingLog.MinimumSize = new System.Drawing.Size(0, 15);
            this.fishingLog.Name = "fishingLog";
            this.fishingLog.Size = new System.Drawing.Size(91, 17);
            this.fishingLog.TabIndex = 4;
            this.fishingLog.TabStop = true;
            this.fishingLog.Text = "Fishing Log";
            this.fishingLog.UnCheckedColor = System.Drawing.Color.Gray;
            this.fishingLog.UseVisualStyleBackColor = true;
            // 
            // points
            // 
            this.points.AutoSize = true;
            this.points.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.points.DisabledColor = System.Drawing.Color.SlateGray;
            this.points.Location = new System.Drawing.Point(110, 18);
            this.points.MinimumSize = new System.Drawing.Size(0, 15);
            this.points.Name = "points";
            this.points.Size = new System.Drawing.Size(66, 17);
            this.points.TabIndex = 4;
            this.points.TabStop = true;
            this.points.Text = "Points";
            this.points.UnCheckedColor = System.Drawing.Color.Gray;
            this.points.UseVisualStyleBackColor = true;
            // 
            // automatic
            // 
            this.automatic.AutoSize = true;
            this.automatic.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.automatic.DisabledColor = System.Drawing.Color.SlateGray;
            this.automatic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.automatic.Location = new System.Drawing.Point(7, 18);
            this.automatic.MinimumSize = new System.Drawing.Size(0, 10);
            this.automatic.Name = "automatic";
            this.automatic.Size = new System.Drawing.Size(84, 17);
            this.automatic.TabIndex = 4;
            this.automatic.Text = "Automatic";
            this.automatic.UnCheckedColor = System.Drawing.Color.Gray;
            this.automatic.UseVisualStyleBackColor = true;
            // 
            // assistedFishingToggle
            // 
            this.assistedFishingToggle.AutoSize = true;
            this.assistedFishingToggle.Location = new System.Drawing.Point(6, 19);
            this.assistedFishingToggle.MinimumSize = new System.Drawing.Size(22, 10);
            this.assistedFishingToggle.Name = "assistedFishingToggle";
            this.assistedFishingToggle.OffBackColor = System.Drawing.Color.Gray;
            this.assistedFishingToggle.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.assistedFishingToggle.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.assistedFishingToggle.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.assistedFishingToggle.Size = new System.Drawing.Size(22, 14);
            this.assistedFishingToggle.TabIndex = 0;
            this.assistedFishingToggle.UseVisualStyleBackColor = true;
            // 
            // indigo
            // 
            this.indigo.AutoSize = true;
            this.indigo.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.indigo.DisabledColor = System.Drawing.Color.SlateGray;
            this.indigo.Location = new System.Drawing.Point(7, 17);
            this.indigo.MinimumSize = new System.Drawing.Size(0, 15);
            this.indigo.Name = "indigo";
            this.indigo.Size = new System.Drawing.Size(66, 17);
            this.indigo.TabIndex = 2;
            this.indigo.TabStop = true;
            this.indigo.Text = "Indigo";
            this.indigo.UnCheckedColor = System.Drawing.Color.Gray;
            this.indigo.UseVisualStyleBackColor = true;
            // 
            // ruby
            // 
            this.ruby.AutoSize = true;
            this.ruby.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.ruby.DisabledColor = System.Drawing.Color.SlateGray;
            this.ruby.Location = new System.Drawing.Point(110, 17);
            this.ruby.MinimumSize = new System.Drawing.Size(0, 15);
            this.ruby.Name = "ruby";
            this.ruby.Size = new System.Drawing.Size(62, 17);
            this.ruby.TabIndex = 3;
            this.ruby.TabStop = true;
            this.ruby.Text = "Ruby";
            this.ruby.UnCheckedColor = System.Drawing.Color.Gray;
            this.ruby.UseVisualStyleBackColor = true;
            // 
            // GPActionNothing
            // 
            this.GPActionNothing.AutoSize = true;
            this.GPActionNothing.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.GPActionNothing.DisabledColor = System.Drawing.Color.SlateGray;
            this.GPActionNothing.Location = new System.Drawing.Point(7, 17);
            this.GPActionNothing.MinimumSize = new System.Drawing.Size(0, 15);
            this.GPActionNothing.Name = "GPActionNothing";
            this.GPActionNothing.Size = new System.Drawing.Size(74, 17);
            this.GPActionNothing.TabIndex = 3;
            this.GPActionNothing.TabStop = true;
            this.GPActionNothing.Text = "Nothing";
            this.GPActionNothing.UnCheckedColor = System.Drawing.Color.Gray;
            this.GPActionNothing.UseVisualStyleBackColor = true;
            // 
            // GPActionDoubleHook
            // 
            this.GPActionDoubleHook.AutoSize = true;
            this.GPActionDoubleHook.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.GPActionDoubleHook.DisabledColor = System.Drawing.Color.SlateGray;
            this.GPActionDoubleHook.Location = new System.Drawing.Point(112, 17);
            this.GPActionDoubleHook.MinimumSize = new System.Drawing.Size(0, 15);
            this.GPActionDoubleHook.Name = "GPActionDoubleHook";
            this.GPActionDoubleHook.Size = new System.Drawing.Size(100, 17);
            this.GPActionDoubleHook.TabIndex = 4;
            this.GPActionDoubleHook.TabStop = true;
            this.GPActionDoubleHook.Text = "Double Hook";
            this.GPActionDoubleHook.UnCheckedColor = System.Drawing.Color.Gray;
            this.GPActionDoubleHook.UseVisualStyleBackColor = true;
            // 
            // GPActionChum
            // 
            this.GPActionChum.AutoSize = true;
            this.GPActionChum.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.GPActionChum.DisabledColor = System.Drawing.Color.SlateGray;
            this.GPActionChum.Location = new System.Drawing.Point(246, 17);
            this.GPActionChum.MinimumSize = new System.Drawing.Size(0, 15);
            this.GPActionChum.Name = "GPActionChum";
            this.GPActionChum.Size = new System.Drawing.Size(64, 17);
            this.GPActionChum.TabIndex = 5;
            this.GPActionChum.TabStop = true;
            this.GPActionChum.Text = "Chum";
            this.GPActionChum.UnCheckedColor = System.Drawing.Color.Gray;
            this.GPActionChum.UseVisualStyleBackColor = true;
            // 
            // patienceDefaultLogic
            // 
            this.patienceDefaultLogic.AutoSize = true;
            this.patienceDefaultLogic.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.patienceDefaultLogic.DisabledColor = System.Drawing.Color.SlateGray;
            this.patienceDefaultLogic.Location = new System.Drawing.Point(7, 16);
            this.patienceDefaultLogic.MinimumSize = new System.Drawing.Size(0, 15);
            this.patienceDefaultLogic.Name = "patienceDefaultLogic";
            this.patienceDefaultLogic.Size = new System.Drawing.Size(100, 17);
            this.patienceDefaultLogic.TabIndex = 3;
            this.patienceDefaultLogic.TabStop = true;
            this.patienceDefaultLogic.Text = "Default Logic";
            this.patienceDefaultLogic.UnCheckedColor = System.Drawing.Color.Gray;
            this.patienceDefaultLogic.UseVisualStyleBackColor = true;
            // 
            // patienceSpectralOnly
            // 
            this.patienceSpectralOnly.AutoSize = true;
            this.patienceSpectralOnly.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.patienceSpectralOnly.DisabledColor = System.Drawing.Color.SlateGray;
            this.patienceSpectralOnly.Location = new System.Drawing.Point(112, 16);
            this.patienceSpectralOnly.MinimumSize = new System.Drawing.Size(0, 15);
            this.patienceSpectralOnly.Name = "patienceSpectralOnly";
            this.patienceSpectralOnly.Size = new System.Drawing.Size(100, 17);
            this.patienceSpectralOnly.TabIndex = 4;
            this.patienceSpectralOnly.TabStop = true;
            this.patienceSpectralOnly.Text = "Spectral Only";
            this.patienceSpectralOnly.UnCheckedColor = System.Drawing.Color.Gray;
            this.patienceSpectralOnly.UseVisualStyleBackColor = true;
            // 
            // patienceAlways
            // 
            this.patienceAlways.AutoSize = true;
            this.patienceAlways.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.patienceAlways.DisabledColor = System.Drawing.Color.SlateGray;
            this.patienceAlways.Location = new System.Drawing.Point(246, 16);
            this.patienceAlways.MinimumSize = new System.Drawing.Size(0, 15);
            this.patienceAlways.Name = "patienceAlways";
            this.patienceAlways.Size = new System.Drawing.Size(70, 17);
            this.patienceAlways.TabIndex = 5;
            this.patienceAlways.TabStop = true;
            this.patienceAlways.Text = "Always";
            this.patienceAlways.UnCheckedColor = System.Drawing.Color.Gray;
            this.patienceAlways.UseVisualStyleBackColor = true;
            // 
            // fishExchangeNone
            // 
            this.fishExchangeNone.AutoSize = true;
            this.fishExchangeNone.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.fishExchangeNone.DisabledColor = System.Drawing.Color.SlateGray;
            this.fishExchangeNone.Location = new System.Drawing.Point(7, 19);
            this.fishExchangeNone.MinimumSize = new System.Drawing.Size(0, 15);
            this.fishExchangeNone.Name = "fishExchangeNone";
            this.fishExchangeNone.Size = new System.Drawing.Size(63, 17);
            this.fishExchangeNone.TabIndex = 3;
            this.fishExchangeNone.TabStop = true;
            this.fishExchangeNone.Text = "None";
            this.fishExchangeNone.UnCheckedColor = System.Drawing.Color.Gray;
            this.fishExchangeNone.UseVisualStyleBackColor = true;
            // 
            // fishExchangeDesynthesize
            // 
            this.fishExchangeDesynthesize.AutoSize = true;
            this.fishExchangeDesynthesize.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.fishExchangeDesynthesize.DisabledColor = System.Drawing.Color.SlateGray;
            this.fishExchangeDesynthesize.Location = new System.Drawing.Point(112, 19);
            this.fishExchangeDesynthesize.MinimumSize = new System.Drawing.Size(0, 15);
            this.fishExchangeDesynthesize.Name = "fishExchangeDesynthesize";
            this.fishExchangeDesynthesize.Size = new System.Drawing.Size(100, 17);
            this.fishExchangeDesynthesize.TabIndex = 4;
            this.fishExchangeDesynthesize.TabStop = true;
            this.fishExchangeDesynthesize.Text = "Desynthesize";
            this.fishExchangeDesynthesize.UnCheckedColor = System.Drawing.Color.Gray;
            this.fishExchangeDesynthesize.UseVisualStyleBackColor = true;
            // 
            // fishExchangeSell
            // 
            this.fishExchangeSell.AutoSize = true;
            this.fishExchangeSell.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(198)))));
            this.fishExchangeSell.DisabledColor = System.Drawing.Color.SlateGray;
            this.fishExchangeSell.Location = new System.Drawing.Point(246, 19);
            this.fishExchangeSell.MinimumSize = new System.Drawing.Size(0, 15);
            this.fishExchangeSell.Name = "fishExchangeSell";
            this.fishExchangeSell.Size = new System.Drawing.Size(54, 17);
            this.fishExchangeSell.TabIndex = 5;
            this.fishExchangeSell.TabStop = true;
            this.fishExchangeSell.Text = "Sell";
            this.fishExchangeSell.UnCheckedColor = System.Drawing.Color.Gray;
            this.fishExchangeSell.UseVisualStyleBackColor = true;
            // 
            // FormOceanSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(730, 600);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.fullGPAction);
            this.Controls.Add(this.lateQueue);
            this.Controls.Add(this.fishingRoute);
            this.Controls.Add(this.fishingPriorityGroupBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOceanSettings";
            this.Text = "FormIdleActivities";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.fishingPriorityGroupBox.ResumeLayout(false);
            this.fishingPriorityGroupBox.PerformLayout();
            this.fishingRoute.ResumeLayout(false);
            this.fishingRoute.PerformLayout();
            this.lateQueue.ResumeLayout(false);
            this.lateQueue.PerformLayout();
            this.fullGPAction.ResumeLayout(false);
            this.fullGPAction.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private UI.Controls.ToggleButton assistedFishingToggle;
        private System.Windows.Forms.Label assistedFishingLabel;
        private System.Windows.Forms.GroupBox fishingPriorityGroupBox;
        private System.Windows.Forms.GroupBox fishingRoute;
        private System.Windows.Forms.GroupBox lateQueue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private UI.Controls.ToggleButton fishFoodToggle;
        private UI.Controls.ToggleButton lateQueueToggle;
        private System.Windows.Forms.GroupBox fullGPAction;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown numericRestockAmount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericRestockThreshold;
        private UI.Controls.RadioButtonFlat automatic;
        private UI.Controls.RadioButtonFlat points;
        private UI.Controls.RadioButtonFlat fishingLog;
        private UI.Controls.RadioButtonFlat ignoreBoat;
        private UI.Controls.RadioButtonFlat achievements;
        private UI.Controls.RadioButtonFlat ruby;
        private UI.Controls.RadioButtonFlat indigo;
        private UI.Controls.RadioButtonFlat GPActionNothing;
        private UI.Controls.RadioButtonFlat GPActionDoubleHook;
        private UI.Controls.RadioButtonFlat GPActionChum;
        private UI.Controls.RadioButtonFlat patienceAlways;
        private UI.Controls.RadioButtonFlat patienceSpectralOnly;
        private UI.Controls.RadioButtonFlat patienceDefaultLogic;
        private UI.Controls.RadioButtonFlat fishExchangeSell;
        private UI.Controls.RadioButtonFlat fishExchangeDesynthesize;
        private UI.Controls.RadioButtonFlat fishExchangeNone;
    }
}