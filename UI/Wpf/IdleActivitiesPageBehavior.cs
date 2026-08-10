using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ff14bot.Managers;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for IdleActivitiesPage.xaml: builds the material/aethersand/food/potion/materia
	/// toggle grids in code, since their labels come from live game data (DataManager.ItemCache) and
	/// their bound property names are generated ("material1".."material7", etc.) rather than fixed —
	/// the same reason OceanSettingsPageBehavior builds its icon grids in code instead of loose XAML.
	/// </summary>
	public static class IdleActivitiesPageBehavior
	{
		private static readonly string[] MaterialProperties =
			{ "material1", "material2", "material3", "material4", "material5", "material6", "material7" };

		private static readonly string[] AethersandProperties =
		{
			"aethersand1", "aethersand2", "aethersand3", "aethersand4", "aethersand5",
			"aethersand6", "aethersand7", "aethersand8", "aethersand9"
		};

		private static readonly string[] FoodProperties = { "food1", "food2", "food3", "food4" };
		private static readonly string[] PotionProperties = { "potion1", "potion2", "potion3", "potion4" };

		private static readonly (string Prefix, string Label, int[] ItemIds)[] MateriaTiers =
		{
#if RB_DT
			("materiaxii", "Grade XII", Defaults.materiaxii),
			("materiaxi", "Grade XI", Defaults.materiaxi),
			("materiax", "Grade X", Defaults.materiax),
			("materiaix", "Grade IX", Defaults.materiaix),
			("materiaviii", "Grade VIII", Defaults.materiaviii),
			("materiavii", "Grade VII", Defaults.materiavii),
			("materiavi", "Grade VI", Defaults.materiavi),
			("materiav", "Grade V", Defaults.materiav),
			("materiaiv", "Grade IV", Defaults.materiaiv),
#else
			("materiax", "Grade X", Defaults.materiax),
			("materiaix", "Grade IX", Defaults.materiaix),
			("materiaviii", "Grade VIII", Defaults.materiaviii),
			("materiavii", "Grade VII", Defaults.materiavii),
			("materiavi", "Grade VI", Defaults.materiavi),
			("materiav", "Grade V", Defaults.materiav),
			("materiaiv", "Grade IV", Defaults.materiaiv),
#endif
		};

		public static void Attach(UserControl page)
		{
			page.DataContext = OceanTripNewSettings.Instance;

			BuildItemPanel((WrapPanel)page.FindName("MaterialsPanel"), Defaults.materials, MaterialProperties);
			BuildItemPanel((WrapPanel)page.FindName("AethersandsPanel"), Defaults.aethersands, AethersandProperties);
			BuildItemPanel((WrapPanel)page.FindName("FoodPanel"), Defaults.raidfood, FoodProperties);
			BuildItemPanel((WrapPanel)page.FindName("PotionsPanel"), Defaults.raidpotions, PotionProperties);

			if (page.FindName("UseCraftingFoodCheck") is CheckBox useCraftingFoodCheck)
#if RB_DT
				useCraftingFoodCheck.Content = "Use \"Rroneek Steak\" when crafting";
#else
				useCraftingFoodCheck.Content = "Use \"Calamari Ripieni\" when crafting";
#endif

			AttachMateria(page);
		}

		private static void BuildItemPanel(WrapPanel panel, int[] itemIds, string[] properties)
		{
			for (int i = 0; i < itemIds.Length && i < properties.Length; i++)
			{
				var checkBox = new CheckBox
				{
					Content = DataManager.ItemCache[(uint)itemIds[i]].CurrentLocaleName,
					Margin = new Thickness(0, 0, 32, 10),
					Style = (Style)Application.Current.Resources["AppCheckBox"]
				};
				checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(properties[i]));
				panel.Children.Add(checkBox);
			}
		}

		private static void AttachMateria(UserControl page)
		{
			var combo = (ComboBox)page.FindName("MateriaGradeCombo");
			var panel = (WrapPanel)page.FindName("MateriaPanel");

			foreach (var tier in MateriaTiers)
				combo.Items.Add(tier.Label);

			combo.SelectionChanged += (s, e) =>
			{
				OceanTripNewSettings.Instance.selectedMateriaIndex = combo.SelectedIndex;
				RebuildMateriaPanel(panel, combo.SelectedIndex);
			};

			combo.SelectedIndex = OceanTripNewSettings.Instance.selectedMateriaIndex;
		}

		private static void RebuildMateriaPanel(WrapPanel panel, int selectedIndex)
		{
			panel.Children.Clear();

			if (selectedIndex < 0 || selectedIndex >= MateriaTiers.Length)
				return;

			var tier = MateriaTiers[selectedIndex];
			for (int i = 0; i < tier.ItemIds.Length; i++)
			{
				var checkBox = new CheckBox
				{
					Content = DataManager.ItemCache[(uint)tier.ItemIds[i]].CurrentLocaleName,
					Margin = new Thickness(0, 0, 32, 10),
					Style = (Style)Application.Current.Resources["AppCheckBox"]
				};
				checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding($"{tier.Prefix}{i + 1}"));
				panel.Children.Add(checkBox);
			}
		}
	}
}
