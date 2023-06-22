using OceanTripPlanner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace Ocean_Trip
{
    public partial class FormOceanSettings : Form
    {
        FormSettings _parent;

        public FormOceanSettings(FormSettings parent)
        {
            InitializeComponent();
            _parent = parent;

            // Data Binding
            lateQueueToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "lateBoatQueue", false, DataSourceUpdateMode.OnPropertyChanged);
            fishFoodToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "OceanFood", false, DataSourceUpdateMode.OnPropertyChanged);
            numericRestockAmount.DataBindings.Add("Value", OceanTripPlanner.OceanTripNewSettings.Instance, "baitRestockAmount", false, DataSourceUpdateMode.OnPropertyChanged);
            numericRestockThreshold.DataBindings.Add("Value", OceanTripPlanner.OceanTripNewSettings.Instance, "baitRestockThreshold", false, DataSourceUpdateMode.OnPropertyChanged);
            assistedFishingToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "openWorldFishing", false, DataSourceUpdateMode.OnPropertyChanged);

            // Parent
            //toggleButton1.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "openWorldFishing", false, DataSourceUpdateMode.OnPropertyChanged);


            //// Radio Buttons
            // Fishing Priority
            switch (OceanTripNewSettings.Instance.FishPriority)
            {
                case OceanTripPlanner.FishPriority.Auto:
                    automatic.Select();
                    break;
                case OceanTripPlanner.FishPriority.Points:
                    points.Select();
                    break;
                case OceanTripPlanner.FishPriority.FishLog:
                    fishingLog.Select();
                    break;
                case OceanTripPlanner.FishPriority.Achievements:
                    achievements.Select();
                    break;
                case OceanTripPlanner.FishPriority.IgnoreBoat:
                    ignoreBoat.Select();
                    break;
            }

            // Fishing Route
            switch (OceanTripNewSettings.Instance.FishingRoute)
            {
                case OceanTripPlanner.FishingRoute.Indigo:
                    indigo.Select();
                    break;
                case OceanTripPlanner.FishingRoute.Ruby:
                    ruby.Select();
                    break;
            }

            // Full GP Action
            switch(OceanTripNewSettings.Instance.FullGPAction)
            {
                case OceanTripPlanner.FullGPAction.None:
                    GPActionNothing.Select();
                    break;
                case OceanTripPlanner.FullGPAction.DoubleHook:
                    GPActionDoubleHook.Select();
                    break;
                case OceanTripPlanner.FullGPAction.Chum:
                    GPActionChum.Select();
                    break;
            }

            // Use Patience
            switch (OceanTripNewSettings.Instance.Patience)
            {
                case ShouldUsePatience.OnlyForSpecificFish:
                    patienceDefaultLogic.Select();
                    break;
                case ShouldUsePatience.SpectralOnly:
                    patienceSpectralOnly.Select();
                    break;
                case ShouldUsePatience.AlwaysUsePatience:
                    patienceAlways.Select();
                    break;
            }

            // Exchange Fish
            switch (OceanTripNewSettings.Instance.ExchangeFish)
            {
                case ExchangeFish.None:
                    fishExchangeNone.Select();
                    break;
                case ExchangeFish.Desynth:
                    fishExchangeDesynthesize.Select();
                    break;
                case ExchangeFish.Sell:
                    fishExchangeSell.Select();
                    break;
            }


            // PictureBox Tooltips
            new ToolTip().SetToolTip(pictureBoxRagworm, "Ragworm");
            new ToolTip().SetToolTip(pictureBoxKrill, "Krill");


            // PictureBox Images
            pictureBoxRagworm.Image = UIElements.getIconImage(8, 23);
            pictureBoxKrill.Image = UIElements.getIconImage(9, 23);
        }


        // Fishing Priority Radio Button Changes
        private void automatic_CheckedChanged(object sender, EventArgs e)
        {
            if (automatic.Checked)
                OceanTripNewSettings.Instance.FishPriority = FishPriority.Auto;
        }

        private void points_CheckedChanged(object sender, EventArgs e)
        {
            if (points.Checked)
                OceanTripNewSettings.Instance.FishPriority = FishPriority.Points;
        }

        private void fishingLog_CheckedChanged(object sender, EventArgs e)
        {
            if (fishingLog.Checked)
                OceanTripNewSettings.Instance.FishPriority = FishPriority.FishLog;
        }

        private void achievements_CheckedChanged(object sender, EventArgs e)
        {
            if (achievements.Checked)
                OceanTripNewSettings.Instance.FishPriority = FishPriority.Achievements;
        }

        private void ignoreBoat_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreBoat.Checked)
                OceanTripNewSettings.Instance.FishPriority = FishPriority.IgnoreBoat;
        }

        
        // Fishing Route
        private void indigo_CheckedChanged(object sender, EventArgs e)
        {
            if (indigo.Checked)
                OceanTripNewSettings.Instance.FishingRoute = FishingRoute.Indigo;
        }

        private void ruby_CheckedChanged(object sender, EventArgs e)
        {
            if (ruby.Checked)
                OceanTripNewSettings.Instance.FishingRoute = FishingRoute.Ruby;
        }


        // Full GP Action
        private void GPActionNothing_CheckedChanged(object sender, EventArgs e)
        {
            if (GPActionNothing.Checked)
                OceanTripNewSettings.Instance.FullGPAction = FullGPAction.None;
        }

        private void GPActionDoubleHook_CheckedChanged(object sender, EventArgs e)
        {
            if (GPActionDoubleHook.Checked)
                OceanTripNewSettings.Instance.FullGPAction = FullGPAction.DoubleHook;
        }

        private void GPActionChum_CheckedChanged(object sender, EventArgs e)
        {
            if (GPActionChum.Checked)
                OceanTripNewSettings.Instance.FullGPAction = FullGPAction.Chum;
        }


        // Patience
        private void patienceDefaultLogic_CheckedChanged(object sender, EventArgs e)
        {
            if (patienceDefaultLogic.Checked)
                OceanTripNewSettings.Instance.Patience = ShouldUsePatience.OnlyForSpecificFish;
        }

        private void patienceSpectralOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (patienceSpectralOnly.Checked)
                OceanTripNewSettings.Instance.Patience = ShouldUsePatience.SpectralOnly;
        }

        private void patienceAlways_CheckedChanged(object sender, EventArgs e)
        {
            if (patienceAlways.Checked)
                OceanTripNewSettings.Instance.Patience = ShouldUsePatience.AlwaysUsePatience;
        }


        // Fish Exchange
        private void fishExchangeNone_CheckedChanged(object sender, EventArgs e)
        {
            if (fishExchangeNone.Checked)
                OceanTripNewSettings.Instance.ExchangeFish = ExchangeFish.None;
        }

        private void fishExchangeDesynthesize_CheckedChanged(object sender, EventArgs e)
        {
            if (fishExchangeDesynthesize.Checked)
                OceanTripNewSettings.Instance.ExchangeFish = ExchangeFish.Desynth;
        }

        private void fishExchangeSell_CheckedChanged(object sender, EventArgs e)
        {
            if (fishExchangeSell.Checked)
                OceanTripNewSettings.Instance.ExchangeFish = ExchangeFish.Sell;
        }


        private void FormOceanSettings_MouseDown(object sender, MouseEventArgs e)
        {
            _parent.MoveWindow(sender, e); //UIElements.MoveWindow(Handle, sender, e);
        }

    }
}
