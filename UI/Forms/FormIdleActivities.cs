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
	public partial class FormIdleActivities : BaseForm
	{
		public FormIdleActivities(FormSettings parent)
		{
			InitializeComponent();
			InitializeBaseForm(parent, pictureBox1);


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
			HandleMouseDown(sender, e);
		}

		private void selectedMateria_SelectedIndexChanged(object sender, EventArgs e)
		{
			OceanTripNewSettings.Instance.selectedMateriaIndex = selectedMateria.SelectedIndex;
			updateMateria(selectedMateria.SelectedIndex);
		}

		/// <summary>
		/// Mapping of dropdown indices to materia tier information
		/// </summary>
		private static readonly Dictionary<int, MateriaInfo> MateriaMapping = new Dictionary<int, MateriaInfo>
		{
#if RB_DT
			{ 0, new MateriaInfo("materiaxii", Defaults.materiaxii) },
			{ 1, new MateriaInfo("materiaxi", Defaults.materiaxi) },
			{ 2, new MateriaInfo("materiax", Defaults.materiax) },
			{ 3, new MateriaInfo("materiaix", Defaults.materiaix) },
			{ 4, new MateriaInfo("materiaviii", Defaults.materiaviii) },
			{ 5, new MateriaInfo("materiavii", Defaults.materiavii) },
			{ 6, new MateriaInfo("materiavi", Defaults.materiavi) },
			{ 7, new MateriaInfo("materiav", Defaults.materiav) },
			{ 8, new MateriaInfo("materiaiv", Defaults.materiaiv) }
#else
			{ 0, new MateriaInfo("materiax", Defaults.materiax) },
			{ 1, new MateriaInfo("materiaix", Defaults.materiaix) },
			{ 2, new MateriaInfo("materiaviii", Defaults.materiaviii) },
			{ 3, new MateriaInfo("materiavii", Defaults.materiavii) },
			{ 4, new MateriaInfo("materiavi", Defaults.materiavi) },
			{ 5, new MateriaInfo("materiav", Defaults.materiav) },
			{ 6, new MateriaInfo("materiaiv", Defaults.materiaiv) }
#endif
		};

		/// <summary>
		/// Helper class to store materia tier information
		/// </summary>
		private class MateriaInfo
		{
			public string SettingsPrefix { get; }
			public int[] ItemIds { get; }

			public MateriaInfo(string settingsPrefix, int[] itemIds)
			{
				SettingsPrefix = settingsPrefix;
				ItemIds = itemIds;
			}
		}

		private void updateMateria(int selectedIndex)
		{
			// Get materia info for selected tier
			if (!MateriaMapping.TryGetValue(selectedIndex, out var materiaInfo))
				return;

			// Get all toggle controls in order
			var toggles = new[] { materia1Toggle, materia2Toggle, materia3Toggle, materia4Toggle, materia5Toggle, materia6Toggle };
			var labels = new[] { materia1Label, materia2Label, materia3Label, materia4Label, materia5Label, materia6Label };

			// Clear all bindings
			foreach (var toggle in toggles)
				toggle.DataBindings.Clear();

			// Bind each toggle to its corresponding setting and set label text
			for (int i = 0; i < toggles.Length && i < materiaInfo.ItemIds.Length; i++)
			{
				// Build property name (e.g., "materiaxii1", "materiaxii2", etc.)
				string propertyName = $"{materiaInfo.SettingsPrefix}{i + 1}";

				// Bind toggle to settings property
				toggles[i].DataBindings.Add("Checked", OceanTripNewSettings.Instance, propertyName, false, DataSourceUpdateMode.OnPropertyChanged);

				// Set label to item name from game data
				labels[i].Text = DataManager.ItemCache[(uint)materiaInfo.ItemIds[i]].CurrentLocaleName;
			}

			this.Refresh();
		}

		private void FormIdleActivities_KeyUp(object sender, KeyEventArgs e)
		{
			HandleKeyUp(sender, e);
		}
	}
}
