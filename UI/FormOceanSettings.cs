using ff14bot.Managers;
using Ocean_Trip.Properties;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;
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

            pictureBox10.Image = ImageExtensions.ToGrayScale(Resources.exit);

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
            new ToolTip().SetToolTip(pictureBoxRagworm, $"{DataManager.ItemCache[FishBait.Ragworm].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxKrill, $"{DataManager.ItemCache[FishBait.Krill].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxPlumpWorm, $"{DataManager.ItemCache[FishBait.PlumpWorm].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxRatTail, $"{DataManager.ItemCache[FishBait.RatTail].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxGlowWorm, $"{DataManager.ItemCache[FishBait.GlowWorm].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxHeavySteelJig, $"{DataManager.ItemCache[FishBait.HeavySteelJig].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxShrimpCageFeeder, $"{DataManager.ItemCache[FishBait.ShrimpCageFeeder].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxPillBug, $"{DataManager.ItemCache[FishBait.PillBug].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxSquidStrip, $"{DataManager.ItemCache[FishBait.SquidStrip].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxMackerelStrip, $"{DataManager.ItemCache[FishBait.MackerelStrip].CurrentLocaleName}");
            new ToolTip().SetToolTip(pictureBoxStoneflyNymph, $"{DataManager.ItemCache[FishBait.StoneflyNymph].CurrentLocaleName}");


            // PictureBox Images
            pictureBoxRagworm.Image = UIElements.getIconImage(8, 23);
            pictureBoxKrill.Image = UIElements.getIconImage(9, 23);
            pictureBoxPlumpWorm.Image = UIElements.getIconImage(10, 23);
            pictureBoxRatTail.Image = UIElements.getIconImage(2, 23);
            pictureBoxGlowWorm.Image = UIElements.getIconImage(3, 23);
            pictureBoxHeavySteelJig.Image = UIElements.getIconImage(5, 23);
            pictureBoxShrimpCageFeeder.Image = UIElements.getIconImage(4, 23);
            pictureBoxPillBug.Image = UIElements.getIconImage(1, 23);
            pictureBoxSquidStrip.Image = UIElements.getIconImage(9, 23);
            pictureBoxMackerelStrip.Image = UIElements.getIconImage(2, 24);
            pictureBoxStoneflyNymph.Image = UIElements.getIconImage(6, 23);



            ragwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "ragwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
            krillLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "krillCount", false, DataSourceUpdateMode.OnPropertyChanged);
            plumpwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "plumpwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
            rattailLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "rattailCount", false, DataSourceUpdateMode.OnPropertyChanged);
            glowwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "glowwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
            heavysteeljigLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "heavysteeljigCount", false, DataSourceUpdateMode.OnPropertyChanged);
            shrimpcagefeederLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "shrimpcagefeederCount", false, DataSourceUpdateMode.OnPropertyChanged);
            pillbugLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "pillbugCount", false, DataSourceUpdateMode.OnPropertyChanged);
            squidstripLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "squidstripCount", false, DataSourceUpdateMode.OnPropertyChanged);
            mackerelstripLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "mackerelstripCount", false, DataSourceUpdateMode.OnPropertyChanged);
            stoneflynymphLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "stoneflynymphCount", false, DataSourceUpdateMode.OnPropertyChanged);
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

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            _parent.Close();
        }

        private void pictureBox10_MouseLeave(object sender, EventArgs e)
        {
            pictureBox10.Image = ImageExtensions.ToGrayScale(Resources.exit);
        }

        private void pictureBox10_MouseEnter(object sender, EventArgs e)
        {
            pictureBox10.Image = Resources.exit;
        }
    }
}
