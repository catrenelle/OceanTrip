using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ff14bot.Managers;
using LlamaLibrary.Properties;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;

namespace Ocean_Trip
{
    public partial class FormIdleActivities : Form
    {
        FormSettings parentForm;

        public FormIdleActivities(FormSettings parent)
        {
            InitializeComponent();

            parentForm = parent;

            pictureBox1.Image = ImageExtensions.ToGrayScale(Ocean_Trip.Properties.Resources.exit);


            // Bind Materials
            material1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material1", false, DataSourceUpdateMode.OnPropertyChanged);
            material2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material2", false, DataSourceUpdateMode.OnPropertyChanged);
            material3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material3", false, DataSourceUpdateMode.OnPropertyChanged);
            material4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material4", false, DataSourceUpdateMode.OnPropertyChanged);
            material5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material5", false, DataSourceUpdateMode.OnPropertyChanged);
            material6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material6", false, DataSourceUpdateMode.OnPropertyChanged);
            material7Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "material7", false, DataSourceUpdateMode.OnPropertyChanged);

            // Bind Aethersands
            aethersand1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand1", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand2", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand3", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand4", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand5", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand6", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand7Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand7", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand8Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand8", false, DataSourceUpdateMode.OnPropertyChanged);
            aethersand9Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "aethersand9", false, DataSourceUpdateMode.OnPropertyChanged);

            // Bind Food
            food1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "food1", false, DataSourceUpdateMode.OnPropertyChanged);
            food2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "food2", false, DataSourceUpdateMode.OnPropertyChanged);
            food3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "food3", false, DataSourceUpdateMode.OnPropertyChanged);
            food4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "food4", false, DataSourceUpdateMode.OnPropertyChanged);

            // Bind Potions
            potion1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "potion1", false, DataSourceUpdateMode.OnPropertyChanged);
            potion2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "potion2", false, DataSourceUpdateMode.OnPropertyChanged);
            potion3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "potion3", false, DataSourceUpdateMode.OnPropertyChanged);
            potion4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "potion4", false, DataSourceUpdateMode.OnPropertyChanged);

            // Bind Crystals
            fireToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "firecrystal", false, DataSourceUpdateMode.OnPropertyChanged);
            iceToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "icecrystal", false, DataSourceUpdateMode.OnPropertyChanged);
            windToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "windcrystal", false, DataSourceUpdateMode.OnPropertyChanged);
            earthToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "earthcrystal", false, DataSourceUpdateMode.OnPropertyChanged);
            lightningToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "lightningcrystal", false, DataSourceUpdateMode.OnPropertyChanged);
            waterToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "watercrystal", false, DataSourceUpdateMode.OnPropertyChanged);


            // Bottom Toggles!
            useCraftingFoodToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "useCraftingFood", false, DataSourceUpdateMode.OnPropertyChanged);
            refillScripsToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "refillScrips", false, DataSourceUpdateMode.OnPropertyChanged);
            purchaseHiCordialsToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "purchaseHiCordials", false, DataSourceUpdateMode.OnPropertyChanged);
            customBoatOrdersToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "customBoatOrders", false, DataSourceUpdateMode.OnPropertyChanged);
            resumeLisbethToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "resumeLisbeth", false, DataSourceUpdateMode.OnPropertyChanged);

            // Change Material Labels
            material1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[0]].CurrentLocaleName;
            material2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[1]].CurrentLocaleName;
            material3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[2]].CurrentLocaleName;
            material4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[3]].CurrentLocaleName;
            material5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[4]].CurrentLocaleName;
            material6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[5]].CurrentLocaleName;
            material7Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materials[6]].CurrentLocaleName;

            // Change Aethersand Labels
            aethersand1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[0]].CurrentLocaleName;
            aethersand2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[1]].CurrentLocaleName;
            aethersand3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[2]].CurrentLocaleName;
            aethersand4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[3]].CurrentLocaleName;
            aethersand5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[4]].CurrentLocaleName;
            aethersand6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[5]].CurrentLocaleName;
            aethersand7Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[6]].CurrentLocaleName;
            aethersand8Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[7]].CurrentLocaleName;
            aethersand9Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.aethersands[8]].CurrentLocaleName;

            // Change Food Labels
            food1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidfood[0]].CurrentLocaleName;
            food2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidfood[1]].CurrentLocaleName;
            food3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidfood[2]].CurrentLocaleName;
            food4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidfood[3]].CurrentLocaleName;

            // Change Potion Labels
            potion1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidpotions[0]].CurrentLocaleName;
            potion2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidpotions[1]].CurrentLocaleName;
            potion3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidpotions[2]].CurrentLocaleName;
            potion4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.raidpotions[3]].CurrentLocaleName;

            // No need to change Crystal labels



            // Update Materia List
            selectedMateria.SelectedIndex = OceanTripPlanner.OceanTripNewSettings.Instance.selectedMateriaIndex;
            updateMateria(selectedMateria.SelectedIndex);
        }

        private void FormIdleActivities_MouseDown(object sender, MouseEventArgs e)
        {
            parentForm.MoveWindow(sender, e); //UIElements.MoveWindow(Handle, sender, e);
        }

        private void selectedMateria_SelectedIndexChanged(object sender, EventArgs e)
        {
            OceanTripNewSettings.Instance.selectedMateriaIndex = selectedMateria.SelectedIndex;
            updateMateria(selectedMateria.SelectedIndex);
        }

        private void updateMateria(int selectedIndex)
        {
            // Clear bindings
            materia1Toggle.DataBindings.Clear();
            materia2Toggle.DataBindings.Clear();
            materia3Toggle.DataBindings.Clear();
            materia4Toggle.DataBindings.Clear();
            materia5Toggle.DataBindings.Clear();
            materia6Toggle.DataBindings.Clear();


            switch (selectedIndex)
            {
                case 0:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxii6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxii[5]].CurrentLocaleName;
                    break;

                case 1:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaxi6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaxi[5]].CurrentLocaleName;
                    break;

                case 2:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiax6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiax[5]].CurrentLocaleName;
                    break;

                case 3:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaix6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaix[5]].CurrentLocaleName;
                    break;

                case 4:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaviii6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaviii[5]].CurrentLocaleName;
                    break;

                case 5:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavii6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavii[5]].CurrentLocaleName;
                    break;

                case 6:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiavi6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiavi[5]].CurrentLocaleName;
                    break;

                case 7:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiav6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiav[5]].CurrentLocaleName;
                    break;

                case 8:
                    materia1Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv1", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia2Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv2", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia3Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv3", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia4Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv4", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia5Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv5", false, DataSourceUpdateMode.OnPropertyChanged);
                    materia6Toggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "materiaiv6", false, DataSourceUpdateMode.OnPropertyChanged);

                    materia1Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[0]].CurrentLocaleName;
                    materia2Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[1]].CurrentLocaleName;
                    materia3Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[2]].CurrentLocaleName;
                    materia4Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[3]].CurrentLocaleName;
                    materia5Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[4]].CurrentLocaleName;
                    materia6Label.Text = DataManager.ItemCache[(uint)OceanTripPlanner.Definitions.Defaults.materiaiv[5]].CurrentLocaleName;
                    break;
            }

            this.Refresh();
        }

        private void FormIdleActivities_KeyUp(object sender, KeyEventArgs e)
        {
            parentForm.FormSettings_KeyUp(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = ImageExtensions.ToGrayScale(Ocean_Trip.Properties.Resources.exit);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Image = Ocean_Trip.Properties.Resources.exit;
        }
    }
}
