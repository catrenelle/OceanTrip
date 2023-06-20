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
            this.assistedFishingToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.assistedFishingLabel = new System.Windows.Forms.Label();
            this.fishingPriorityGroupBox = new System.Windows.Forms.GroupBox();
            this.automatic = new System.Windows.Forms.RadioButton();
            this.points = new System.Windows.Forms.RadioButton();
            this.fishingLog = new System.Windows.Forms.RadioButton();
            this.achievements = new System.Windows.Forms.RadioButton();
            this.ignoreBoat = new System.Windows.Forms.RadioButton();
            this.fishingRoute = new System.Windows.Forms.GroupBox();
            this.indigo = new System.Windows.Forms.RadioButton();
            this.ruby = new System.Windows.Forms.RadioButton();
            this.lateQueue = new System.Windows.Forms.GroupBox();
            this.lateQueueToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.fishFoodToggle = new Ocean_Trip.UI.Controls.ToggleButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fullGPAction = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.GPActionNothing = new System.Windows.Forms.RadioButton();
            this.GPActionDoubleHook = new System.Windows.Forms.RadioButton();
            this.GPActionChum = new System.Windows.Forms.RadioButton();
            this.patienceDefaultLogic = new System.Windows.Forms.RadioButton();
            this.patienceSpectralOnly = new System.Windows.Forms.RadioButton();
            this.patienceAlways = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.fishExchangeNone = new System.Windows.Forms.RadioButton();
            this.fishExchangeDesynthesize = new System.Windows.Forms.RadioButton();
            this.fishExchangeSell = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericRestockThreshold = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericRestockAmount = new System.Windows.Forms.NumericUpDown();
            this.groupBox3.SuspendLayout();
            this.fishingPriorityGroupBox.SuspendLayout();
            this.fishingRoute.SuspendLayout();
            this.lateQueue.SuspendLayout();
            this.fullGPAction.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockAmount)).BeginInit();
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
            // automatic
            // 
            this.automatic.AutoSize = true;
            this.automatic.Location = new System.Drawing.Point(6, 19);
            this.automatic.Name = "automatic";
            this.automatic.Size = new System.Drawing.Size(72, 17);
            this.automatic.TabIndex = 0;
            this.automatic.TabStop = true;
            this.automatic.Text = "Automatic";
            this.automatic.UseVisualStyleBackColor = true;
            this.automatic.CheckedChanged += new System.EventHandler(this.automatic_CheckedChanged);
            // 
            // points
            // 
            this.points.AutoSize = true;
            this.points.Location = new System.Drawing.Point(116, 19);
            this.points.Name = "points";
            this.points.Size = new System.Drawing.Size(54, 17);
            this.points.TabIndex = 1;
            this.points.TabStop = true;
            this.points.Text = "Points";
            this.points.UseVisualStyleBackColor = true;
            this.points.CheckedChanged += new System.EventHandler(this.points_CheckedChanged);
            // 
            // fishingLog
            // 
            this.fishingLog.AutoSize = true;
            this.fishingLog.Location = new System.Drawing.Point(226, 19);
            this.fishingLog.Name = "fishingLog";
            this.fishingLog.Size = new System.Drawing.Size(79, 17);
            this.fishingLog.TabIndex = 2;
            this.fishingLog.TabStop = true;
            this.fishingLog.Text = "Fishing Log";
            this.fishingLog.UseVisualStyleBackColor = true;
            this.fishingLog.CheckedChanged += new System.EventHandler(this.fishingLog_CheckedChanged);
            // 
            // achievements
            // 
            this.achievements.AutoSize = true;
            this.achievements.Enabled = false;
            this.achievements.Location = new System.Drawing.Point(336, 19);
            this.achievements.Name = "achievements";
            this.achievements.Size = new System.Drawing.Size(92, 17);
            this.achievements.TabIndex = 3;
            this.achievements.TabStop = true;
            this.achievements.Text = "Achievements";
            this.achievements.UseVisualStyleBackColor = true;
            this.achievements.CheckedChanged += new System.EventHandler(this.achievements_CheckedChanged);
            // 
            // ignoreBoat
            // 
            this.ignoreBoat.AutoSize = true;
            this.ignoreBoat.Location = new System.Drawing.Point(472, 19);
            this.ignoreBoat.Name = "ignoreBoat";
            this.ignoreBoat.Size = new System.Drawing.Size(80, 17);
            this.ignoreBoat.TabIndex = 4;
            this.ignoreBoat.TabStop = true;
            this.ignoreBoat.Text = "Ignore Boat";
            this.ignoreBoat.UseVisualStyleBackColor = true;
            this.ignoreBoat.CheckedChanged += new System.EventHandler(this.ignoreBoat_CheckedChanged);
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
            // indigo
            // 
            this.indigo.AutoSize = true;
            this.indigo.Location = new System.Drawing.Point(6, 19);
            this.indigo.Name = "indigo";
            this.indigo.Size = new System.Drawing.Size(54, 17);
            this.indigo.TabIndex = 0;
            this.indigo.TabStop = true;
            this.indigo.Text = "Indigo";
            this.indigo.UseVisualStyleBackColor = true;
            this.indigo.CheckedChanged += new System.EventHandler(this.indigo_CheckedChanged);
            // 
            // ruby
            // 
            this.ruby.AutoSize = true;
            this.ruby.Location = new System.Drawing.Point(116, 19);
            this.ruby.Name = "ruby";
            this.ruby.Size = new System.Drawing.Size(50, 17);
            this.ruby.TabIndex = 1;
            this.ruby.TabStop = true;
            this.ruby.Text = "Ruby";
            this.ruby.UseVisualStyleBackColor = true;
            this.ruby.CheckedChanged += new System.EventHandler(this.ruby_CheckedChanged);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Late Queue";
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
            // fullGPAction
            // 
            this.fullGPAction.Controls.Add(this.GPActionChum);
            this.fullGPAction.Controls.Add(this.GPActionDoubleHook);
            this.fullGPAction.Controls.Add(this.GPActionNothing);
            this.fullGPAction.ForeColor = System.Drawing.Color.Gainsboro;
            this.fullGPAction.Location = new System.Drawing.Point(235, 136);
            this.fullGPAction.Name = "fullGPAction";
            this.fullGPAction.Size = new System.Drawing.Size(340, 46);
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
            this.groupBox5.Location = new System.Drawing.Point(235, 190);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(340, 46);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Use Patience Skill";
            this.groupBox5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);

            // 
            // GPActionNothing
            // 
            this.GPActionNothing.AutoSize = true;
            this.GPActionNothing.Location = new System.Drawing.Point(8, 17);
            this.GPActionNothing.Name = "GPActionNothing";
            this.GPActionNothing.Size = new System.Drawing.Size(62, 17);
            this.GPActionNothing.TabIndex = 0;
            this.GPActionNothing.TabStop = true;
            this.GPActionNothing.Text = "Nothing";
            this.GPActionNothing.UseVisualStyleBackColor = true;
            this.GPActionNothing.CheckedChanged += new System.EventHandler(this.GPActionNothing_CheckedChanged);
            // 
            // GPActionDoubleHook
            // 
            this.GPActionDoubleHook.AutoSize = true;
            this.GPActionDoubleHook.Location = new System.Drawing.Point(118, 17);
            this.GPActionDoubleHook.Name = "GPActionDoubleHook";
            this.GPActionDoubleHook.Size = new System.Drawing.Size(88, 17);
            this.GPActionDoubleHook.TabIndex = 1;
            this.GPActionDoubleHook.TabStop = true;
            this.GPActionDoubleHook.Text = "Double Hook";
            this.GPActionDoubleHook.UseVisualStyleBackColor = true;
            this.GPActionDoubleHook.CheckedChanged += new System.EventHandler(this.GPActionDoubleHook_CheckedChanged);
            // 
            // GPActionChum
            // 
            this.GPActionChum.AutoSize = true;
            this.GPActionChum.Location = new System.Drawing.Point(254, 17);
            this.GPActionChum.Name = "GPActionChum";
            this.GPActionChum.Size = new System.Drawing.Size(52, 17);
            this.GPActionChum.TabIndex = 2;
            this.GPActionChum.TabStop = true;
            this.GPActionChum.Text = "Chum";
            this.GPActionChum.UseVisualStyleBackColor = true;
            this.GPActionChum.CheckedChanged += new System.EventHandler(this.GPActionChum_CheckedChanged);
            // 
            // patienceDefaultLogic
            // 
            this.patienceDefaultLogic.AutoSize = true;
            this.patienceDefaultLogic.Location = new System.Drawing.Point(8, 17);
            this.patienceDefaultLogic.Name = "patienceDefaultLogic";
            this.patienceDefaultLogic.Size = new System.Drawing.Size(88, 17);
            this.patienceDefaultLogic.TabIndex = 0;
            this.patienceDefaultLogic.TabStop = true;
            this.patienceDefaultLogic.Text = "Default Logic";
            this.patienceDefaultLogic.UseVisualStyleBackColor = true;
            this.patienceDefaultLogic.CheckedChanged += new System.EventHandler(this.patienceDefaultLogic_CheckedChanged);
            // 
            // patienceSpectralOnly
            // 
            this.patienceSpectralOnly.AutoSize = true;
            this.patienceSpectralOnly.Location = new System.Drawing.Point(118, 16);
            this.patienceSpectralOnly.Name = "patienceSpectralOnly";
            this.patienceSpectralOnly.Size = new System.Drawing.Size(88, 17);
            this.patienceSpectralOnly.TabIndex = 1;
            this.patienceSpectralOnly.TabStop = true;
            this.patienceSpectralOnly.Text = "Spectral Only";
            this.patienceSpectralOnly.UseVisualStyleBackColor = true;
            this.patienceSpectralOnly.CheckedChanged += new System.EventHandler(this.patienceSpectralOnly_CheckedChanged);
            // 
            // patienceAlways
            // 
            this.patienceAlways.AutoSize = true;
            this.patienceAlways.Location = new System.Drawing.Point(254, 16);
            this.patienceAlways.Name = "patienceAlways";
            this.patienceAlways.Size = new System.Drawing.Size(58, 17);
            this.patienceAlways.TabIndex = 2;
            this.patienceAlways.TabStop = true;
            this.patienceAlways.Text = "Always";
            this.patienceAlways.UseVisualStyleBackColor = true;
            this.patienceAlways.CheckedChanged += new System.EventHandler(this.patienceAlways_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.fishExchangeSell);
            this.groupBox4.Controls.Add(this.fishExchangeDesynthesize);
            this.groupBox4.Controls.Add(this.fishExchangeNone);
            this.groupBox4.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox4.Location = new System.Drawing.Point(235, 242);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(340, 46);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fish Exchange";
            this.groupBox4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormOceanSettings_MouseDown);

            // 
            // fishExchangeNone
            // 
            this.fishExchangeNone.AutoSize = true;
            this.fishExchangeNone.Location = new System.Drawing.Point(8, 19);
            this.fishExchangeNone.Name = "fishExchangeNone";
            this.fishExchangeNone.Size = new System.Drawing.Size(51, 17);
            this.fishExchangeNone.TabIndex = 0;
            this.fishExchangeNone.TabStop = true;
            this.fishExchangeNone.Text = "None";
            this.fishExchangeNone.UseVisualStyleBackColor = true;
            this.fishExchangeNone.CheckedChanged += new System.EventHandler(this.fishExchangeNone_CheckedChanged);
            // 
            // fishExchangeDesynthesize
            // 
            this.fishExchangeDesynthesize.AutoSize = true;
            this.fishExchangeDesynthesize.Location = new System.Drawing.Point(118, 19);
            this.fishExchangeDesynthesize.Name = "fishExchangeDesynthesize";
            this.fishExchangeDesynthesize.Size = new System.Drawing.Size(88, 17);
            this.fishExchangeDesynthesize.TabIndex = 1;
            this.fishExchangeDesynthesize.TabStop = true;
            this.fishExchangeDesynthesize.Text = "Desynthesize";
            this.fishExchangeDesynthesize.UseVisualStyleBackColor = true;
            this.fishExchangeDesynthesize.CheckedChanged += new System.EventHandler(this.fishExchangeDesynthesize_CheckedChanged);
            // 
            // fishExchangeSell
            // 
            this.fishExchangeSell.AutoSize = true;
            this.fishExchangeSell.Location = new System.Drawing.Point(254, 19);
            this.fishExchangeSell.Name = "fishExchangeSell";
            this.fishExchangeSell.Size = new System.Drawing.Size(42, 17);
            this.fishExchangeSell.TabIndex = 2;
            this.fishExchangeSell.TabStop = true;
            this.fishExchangeSell.Text = "Sell";
            this.fishExchangeSell.UseVisualStyleBackColor = true;
            this.fishExchangeSell.CheckedChanged += new System.EventHandler(this.fishExchangeSell_CheckedChanged);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Restock Threshold:";
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
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRestockAmount)).EndInit();
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
        private System.Windows.Forms.RadioButton automatic;
        private System.Windows.Forms.RadioButton ignoreBoat;
        private System.Windows.Forms.RadioButton achievements;
        private System.Windows.Forms.RadioButton fishingLog;
        private System.Windows.Forms.RadioButton points;
        private System.Windows.Forms.GroupBox fishingRoute;
        private System.Windows.Forms.RadioButton ruby;
        private System.Windows.Forms.RadioButton indigo;
        private System.Windows.Forms.GroupBox lateQueue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private UI.Controls.ToggleButton fishFoodToggle;
        private UI.Controls.ToggleButton lateQueueToggle;
        private System.Windows.Forms.GroupBox fullGPAction;
        private System.Windows.Forms.RadioButton GPActionNothing;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton GPActionDoubleHook;
        private System.Windows.Forms.RadioButton GPActionChum;
        private System.Windows.Forms.RadioButton patienceSpectralOnly;
        private System.Windows.Forms.RadioButton patienceDefaultLogic;
        private System.Windows.Forms.RadioButton patienceAlways;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton fishExchangeDesynthesize;
        private System.Windows.Forms.RadioButton fishExchangeNone;
        private System.Windows.Forms.RadioButton fishExchangeSell;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown numericRestockAmount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericRestockThreshold;
    }
}