using OceanTripPlanner.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ocean_Trip
{
    public partial class FormSettings : Form
    {
        private Button currentButton;
        private Form currentForm;

        // Click to Drag Window
        public void MoveWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                UIElements.MoveWindow(Handle, sender, e);
        }

        public FormSettings()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(UIElements.CreateRoundRectRgn(0, 0, Width, Height, 30, 30));

            // Data Binding
            verboseLoggingToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "loggingMode", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ActivateButton(object buttonSender)
        {
            try
            {
                if (buttonSender != null)
                {
                    if (currentButton != (Button)buttonSender)
                    {
                        DeactivateButtons();
                        currentButton = (Button)buttonSender;
                        currentButton.BackColor = Colors.buttonActiveBackgroundColor;
                        currentButton.ForeColor = Colors.buttonActiveForegroundColor;
                        currentButton.Font = new Font("Microsoft Sans Serif", 9.25F, FontStyle.Regular, GraphicsUnit.Point);
                    }
                }
            }
            catch { }
        }

        private void DeactivateButtons()
        {
            foreach (Control button in panelMenu.Controls)
            {
                if (button.GetType() == typeof(Button))
                {
                    button.BackColor = Colors.buttonDefaultBackgroundColor;
                    button.ForeColor = Colors.buttonDefaultForegroundColor;
                    button.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
                }
            }
        }

        private void buttonIdleStuff_Click(object sender, EventArgs e)
        {
            loadForm(new FormIdleActivities(this), sender);
        }

        private void buttonBoatSettings_Click(object sender, EventArgs e)
        {
            loadForm(new FormOceanSettings(this), sender);
        }

        private void buttonSchedule_Click(object sender, EventArgs e)
        {
            loadForm(new FormSchedule(this), sender);
        }

        private void buttonCurrentRoute_Click(object sender, EventArgs e)
        {
            loadForm(new FormCurrentRoute(this), sender);
        }

        private void loadForm(Form childForm, object sender)
        {
            if (currentForm != null)
                currentForm.Close();

            ActivateButton(sender);
            currentForm = childForm;
            currentForm.TopLevel = false;
            currentForm.FormBorderStyle = FormBorderStyle.None;
            currentForm.Dock = DockStyle.Fill;
            panelMain.Controls.Add(currentForm);
            panelMain.Tag = currentForm; ; ;
            currentForm.BringToFront();
            currentForm.Show();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            loadForm(new FormIdleActivities(this), null);
            buttonIdleStuff.BackColor = Colors.buttonActiveBackgroundColor;
            buttonIdleStuff.ForeColor = Colors.buttonActiveForegroundColor;
            buttonIdleStuff.Font = new Font("Microsoft Sans Serif", 9.25F, FontStyle.Regular, GraphicsUnit.Point);
        }

        public void FormSettings_KeyUp(object sender, KeyEventArgs e)
        {
            ProcessDialogKey(e.KeyCode);
        }

        private void LisbethButton_Click(object sender, EventArgs e)
        {
            Lisbeth.OpenWindow();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LisbethButton_Click(sender, e);
        }

        private void LlamaMarketButton_Click(object sender, EventArgs e)
        {
            LlamaMarket.OpenMarketSettings();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            LlamaMarketButton_Click(sender, e);
        }
    }
}
