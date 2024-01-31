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

            exitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);

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
            pictureBoxSquidStrip.Image = UIElements.getIconImage(7, 23);
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


            // Achievements
            mantasPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 32));
            octopodsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(3, 32));
            sharksPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 32));
            jellyfishPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(5, 32));
            seadragonPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(6, 32));
            balloonsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(7, 32));
            crabsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(8, 32));
            indigo5kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
            indigo10kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
            indigo16kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
            indigo20kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));

            shrimpPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 34));
            shellfishPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(2, 34));
            squidPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(3, 34));
            ruby5kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));
            ruby10kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));
            ruby16kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));

            overall100kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
            overall500kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
            overall1mPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
            overall3mPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));

            // Achievements Databind
            mantasPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementMantas", false, DataSourceUpdateMode.OnPropertyChanged);
            octopodsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementOctopods", false, DataSourceUpdateMode.OnPropertyChanged);
            sharksPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSharks", false, DataSourceUpdateMode.OnPropertyChanged);
            jellyfishPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementJellyfish", false, DataSourceUpdateMode.OnPropertyChanged);
            seadragonPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSeadragons", false, DataSourceUpdateMode.OnPropertyChanged);
            balloonsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementBalloons", false, DataSourceUpdateMode.OnPropertyChanged);
            crabsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementCrabs", false, DataSourceUpdateMode.OnPropertyChanged);
            indigo5kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement5kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
            indigo10kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement10kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
            indigo16kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement16kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
            indigo20kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement20kindigo", false, DataSourceUpdateMode.OnPropertyChanged);

            shrimpPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementShrimp", false, DataSourceUpdateMode.OnPropertyChanged);
            shellfishPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementShellfish", false, DataSourceUpdateMode.OnPropertyChanged);
            squidPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSquid", false, DataSourceUpdateMode.OnPropertyChanged);
            ruby5kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement5kruby", false, DataSourceUpdateMode.OnPropertyChanged);
            ruby10kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement10kruby", false, DataSourceUpdateMode.OnPropertyChanged);
            ruby16kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement16kruby", false, DataSourceUpdateMode.OnPropertyChanged);

            overall100kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement100koverall", false, DataSourceUpdateMode.OnPropertyChanged);
            overall500kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement500koverall", false, DataSourceUpdateMode.OnPropertyChanged);
            overall1mPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement1moverall", false, DataSourceUpdateMode.OnPropertyChanged);
            overall3mPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement3moverall", false, DataSourceUpdateMode.OnPropertyChanged);
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
            exitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);
        }

        private void pictureBox10_MouseEnter(object sender, EventArgs e)
        {
            exitIcon.Image = Resources.exit;
        }

        private void mantasPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (mantasPicture.Checked)
                mantasPicture.Image = UIElements.getIconImage(9, 32);
        }

        private void octopodsPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (octopodsPicture.Checked)
                octopodsPicture.Image = UIElements.getIconImage(3, 32);
        }

        private void sharksPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (sharksPicture.Checked)
                sharksPicture.Image = UIElements.getIconImage(4, 32);
        }

        private void jellyfishPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (jellyfishPicture.Checked)
                jellyfishPicture.Image = UIElements.getIconImage(5, 32);
        }

        private void seadragonPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (seadragonPicture.Checked)
                seadragonPicture.Image = UIElements.getIconImage(6, 32);
        }

        private void balloonsPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (balloonsPicture.Checked)
                balloonsPicture.Image = UIElements.getIconImage(7, 32);
        }

        private void crabsPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (crabsPicture.Checked)
                crabsPicture.Image = UIElements.getIconImage(8, 32);
        }

        private void indigo5kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (indigo5kPicture.Checked)
                indigo5kPicture.Image = UIElements.getIconImage(4, 27);
        }

        private void indigo10kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (indigo10kPicture.Checked)
                indigo10kPicture.Image = UIElements.getIconImage(4, 27);
        }

        private void indigo16kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (indigo16kPicture.Checked)
                indigo16kPicture.Image = UIElements.getIconImage(4, 27);
        }

        private void indigo20kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (indigo20kPicture.Checked)
                indigo20kPicture.Image = UIElements.getIconImage(4, 27);
        }

        private void shrimpPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (shrimpPicture.Checked)
                shrimpPicture.Image = UIElements.getIconImage(4, 34);
        }

        private void shellfishPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (shellfishPicture.Checked)
                shellfishPicture.Image = UIElements.getIconImage(2, 34);
        }

        private void squidPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (squidPicture.Checked)
                squidPicture.Image = UIElements.getIconImage(3, 34);
        }

        private void ruby5kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (ruby5kPicture.Checked)
                ruby5kPicture.Image = UIElements.getIconImage(9, 27);
        }

        private void ruby10kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (ruby10kPicture.Checked)
                ruby10kPicture.Image = UIElements.getIconImage(9, 27);
        }

        private void ruby16kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (ruby16kPicture.Checked)
                ruby16kPicture.Image = UIElements.getIconImage(9, 27);
        }

        private void overall100kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (overall100kPicture.Checked)
                overall100kPicture.Image = UIElements.getIconImage(1, 27);
        }

        private void overall500kPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (overall500kPicture.Checked)
                overall500kPicture.Image = UIElements.getIconImage(1, 27);
        }

        private void overall1mPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (overall1mPicture.Checked)
                overall1mPicture.Image = UIElements.getIconImage(1, 27);
        }

        private void overall3mPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (overall3mPicture.Checked)
                overall3mPicture.Image = UIElements.getIconImage(1, 27);
        }
    }
}
