using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ff14bot.Managers;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for IdleActivitiesPage.xaml: builds the material/aethersand/food/potion
	/// toggle grids and the materia grade-by-type checkbox matrix in code, since their labels come
	/// from live game data (DataManager.ItemCache) and their bound property names are generated
	/// ("material1".."material16", etc.) rather than fixed — the same reason OceanSettingsPageBehavior
	/// builds its icon grids in code instead of loose XAML.
	/// </summary>
	public static class IdleActivitiesPageBehavior
	{
		private static readonly string[] MaterialProperties =
		{
			"material1", "material2", "material3", "material4", "material5", "material6", "material7",
			"material8", "material9", "material10", "material11", "material12", "material13",
			"material14", "material15", "material16"
		};

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

		/// <summary>
		/// Grades shown in the materia matrix by default; the rest sit behind the "older grades"
		/// toggle. Selections in collapsed rows still farm (the bot reads every enabled materia
		/// across all tiers), so the toggle line surfaces a count whenever any are enabled.
		/// </summary>
		private const int AlwaysVisibleTiers = 3;

		private const int MateriaTypesPerTier = 6;

		public static void Attach(UserControl page)
		{
			page.DataContext = OceanTripNewSettings.Instance;

			BuildItemPanel((Panel)page.FindName("MaterialsPanel"), Defaults.materials, MaterialProperties);
			BuildItemPanel((Panel)page.FindName("AethersandsPanel"), Defaults.aethersands, AethersandProperties,
				name => StripSuffix(name, " Aethersand"));
			BuildItemPanel((Panel)page.FindName("FoodPanel"), Defaults.raidfood, FoodProperties);
			BuildPotionsPanel(page);

			if (page.FindName("UseCraftingFoodCheck") is CheckBox useCraftingFoodCheck)
#if RB_DT
				useCraftingFoodCheck.Content = "Use \"Rroneek Steak\" when crafting";
#else
				useCraftingFoodCheck.Content = "Use \"Calamari Ripieni\" when crafting";
#endif

			AttachMateria(page);
		}

		private static void BuildItemPanel(Panel panel, int[] itemIds, string[] properties,
			Func<string, string> shortenLabel = null)
		{
			for (int i = 0; i < itemIds.Length && i < properties.Length; i++)
			{
				string fullName = DataManager.ItemCache[(uint)itemIds[i]].CurrentLocaleName;
				string label = shortenLabel?.Invoke(fullName) ?? fullName;
				var checkBox = new CheckBox
				{
					Content = label,
					// Shortened labels keep the real item name discoverable on hover.
					ToolTip = label != fullName ? fullName : null,
					Margin = new Thickness(0, 0, 12, 5),
					Style = (Style)Application.Current.Resources["AppCheckBox"]
				};
				checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(properties[i]));
				panel.Children.Add(checkBox);
			}
		}

		/// <summary>
		/// The raid potions all share one base name ("Grade 2 Gemdraught of Strength/Dexterity/..."),
		/// so the shared part moves into the card header ("Craft Grade 2 Gemdraughts") and the
		/// checkboxes keep just the stat. Derived from the live item names rather than hardcoded so
		/// the DT / CN / EW item sets all label themselves correctly.
		/// </summary>
		private static void BuildPotionsPanel(UserControl page)
		{
			var panel = (Panel)page.FindName("PotionsPanel");
			var names = Defaults.raidpotions
				.Select(id => DataManager.ItemCache[(uint)id].CurrentLocaleName)
				.ToArray();

			string prefix = SharedOfPrefix(names);
			if (prefix != null && page.FindName("PotionsGroup") is GroupBox potionsGroup)
				potionsGroup.Header = $"Craft {prefix.Substring(0, prefix.Length - " of ".Length)}s";

			BuildItemPanel(panel, Defaults.raidpotions, PotionProperties,
				prefix == null ? null : name => name.Substring(prefix.Length));
		}

		/// <summary>Common "... of " prefix shared by every name, or null if they don't all share one.</summary>
		private static string SharedOfPrefix(string[] names)
		{
			if (names.Length == 0)
				return null;

			int ofIndex = names[0].IndexOf(" of ", StringComparison.OrdinalIgnoreCase);
			if (ofIndex < 0)
				return null;

			string candidate = names[0].Substring(0, ofIndex + " of ".Length);
			return names.All(n => n.StartsWith(candidate, StringComparison.OrdinalIgnoreCase)) ? candidate : null;
		}

		private static string StripSuffix(string name, string suffix) =>
			name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
				? name.Substring(0, name.Length - suffix.Length)
				: name;

		/// <summary>
		/// Builds the materia matrix: one row per grade, one checkbox column per materia type
		/// (Craftsman's Competence/Cunning/Command, Gatherer's Guerdon/Guile/Grasp). This replaces
		/// the old grade ComboBox that paged one tier's checkboxes at a time — that hid enabled
		/// materia in every non-selected tier even though the bot farms all of them
		/// (OceanTripSettings.GetEnabledMateriaIds walks every tier), so state the user was acting
		/// on was invisible. The matrix shows all of it at once; grades below the top few collapse
		/// behind a toggle that always reports how many hidden materia are enabled.
		/// </summary>
		private static void AttachMateria(UserControl page)
		{
			var host = (StackPanel)page.FindName("MateriaHost");
			var checkBoxStyle = (Style)Application.Current.Resources["AppCheckBox"];
			var secondaryText = (Style)Application.Current.Resources["SecondaryText"];

			// Column 0 holds the grade labels; a fixed-width spacer column separates the
			// Craftsman's and Gatherer's groups.
			var grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			for (int t = 0; t < MateriaTypesPerTier; t++)
			{
				if (t == MateriaTypesPerTier / 2)
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(14) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			}
			for (int r = 0; r < MateriaTiers.Length + 2; r++)
				grid.RowDefinitions.Add(new RowDefinition());

			AddGroupHeader(grid, secondaryText, "Craftsman's", TypeColumn(0));
			AddGroupHeader(grid, secondaryText, "Gatherer's", TypeColumn(MateriaTypesPerTier / 2));

			for (int t = 0; t < MateriaTypesPerTier; t++)
			{
				var typeHeader = new TextBlock
				{
					Text = ShortMateriaName(DataManager.ItemCache[(uint)MateriaTiers[0].ItemIds[t]].CurrentLocaleName),
					Style = secondaryText,
					FontSize = 11,
					Margin = new Thickness(5, 0, 5, 4),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				Grid.SetRow(typeHeader, 1);
				Grid.SetColumn(typeHeader, TypeColumn(t));
				grid.Children.Add(typeHeader);
			}

			var hiddenRowElements = new List<FrameworkElement>();
			var hiddenCheckBoxes = new List<CheckBox>();
			bool expanded = false;

			var toggle = new TextBlock
			{
				Cursor = Cursors.Hand,
				Margin = new Thickness(0, 4, 0, 0),
				Foreground = (Brush)Application.Current.Resources["AccentLightBrush"],
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 12
			};
			var toggleLabel = new Run();
			var hiddenCount = new Run { Foreground = (Brush)Application.Current.Resources["WarningBrush"] };
			toggle.Inlines.Add(toggleLabel);
			toggle.Inlines.Add(hiddenCount);

			string collapsedRange = MateriaTiers.Length > AlwaysVisibleTiers
				? $"{MateriaTiers[AlwaysVisibleTiers].Label}–{MateriaTiers[MateriaTiers.Length - 1].Label.Replace("Grade ", "")}"
				: "";

			void UpdateToggle()
			{
				int enabled = hiddenCheckBoxes.Count(c => c.IsChecked == true);
				toggleLabel.Text = expanded ? "▾ Hide older grades" : $"▸ Show older grades ({collapsedRange})";
				hiddenCount.Text = enabled > 0 ? $"  —  {enabled} enabled" : "";
			}

			for (int row = 0; row < MateriaTiers.Length; row++)
			{
				var tier = MateriaTiers[row];
				bool hiddenRow = row >= AlwaysVisibleTiers;

				var gradeLabel = new TextBlock
				{
					Text = tier.Label,
					Style = secondaryText,
					Margin = new Thickness(0, 0, 10, 4),
					VerticalAlignment = VerticalAlignment.Center,
					Visibility = hiddenRow ? Visibility.Collapsed : Visibility.Visible
				};
				Grid.SetRow(gradeLabel, row + 2);
				Grid.SetColumn(gradeLabel, 0);
				grid.Children.Add(gradeLabel);
				if (hiddenRow)
					hiddenRowElements.Add(gradeLabel);

				for (int t = 0; t < MateriaTypesPerTier; t++)
				{
					var checkBox = new CheckBox
					{
						ToolTip = DataManager.ItemCache[(uint)tier.ItemIds[t]].CurrentLocaleName,
						Margin = new Thickness(0, 0, 0, 4),
						HorizontalAlignment = HorizontalAlignment.Center,
						Style = checkBoxStyle,
						Visibility = hiddenRow ? Visibility.Collapsed : Visibility.Visible
					};
					checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding($"{tier.Prefix}{t + 1}"));
					Grid.SetRow(checkBox, row + 2);
					Grid.SetColumn(checkBox, TypeColumn(t));
					grid.Children.Add(checkBox);

					if (hiddenRow)
					{
						hiddenRowElements.Add(checkBox);
						hiddenCheckBoxes.Add(checkBox);
						// Keep the collapsed-state count live: these fire both on user clicks and
						// when the two-way bindings resolve against saved settings on load.
						checkBox.Checked += (s, e) => UpdateToggle();
						checkBox.Unchecked += (s, e) => UpdateToggle();
					}
				}
			}

			host.Children.Add(grid);

			if (MateriaTiers.Length > AlwaysVisibleTiers)
			{
				toggle.MouseLeftButtonUp += (s, e) =>
				{
					expanded = !expanded;
					foreach (var element in hiddenRowElements)
						element.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
					UpdateToggle();
				};
				UpdateToggle();
				host.Children.Add(toggle);
			}
		}

		private static void AddGroupHeader(Grid grid, Style style, string text, int startColumn)
		{
			var header = new TextBlock
			{
				Text = text,
				Style = style,
				FontSize = 11,
				FontWeight = FontWeights.SemiBold,
				Margin = new Thickness(0, 0, 0, 2),
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Grid.SetRow(header, 0);
			Grid.SetColumn(header, startColumn);
			Grid.SetColumnSpan(header, MateriaTypesPerTier / 2);
			grid.Children.Add(header);
		}

		/// <summary>Grid column for materia type index 0..5, skipping the group-gap spacer column.</summary>
		private static int TypeColumn(int typeIndex) =>
			1 + typeIndex + (typeIndex >= MateriaTypesPerTier / 2 ? 1 : 0);

		/// <summary>
		/// "Craftsman's Competence Materia XII" -> "Competence" for the matrix column headers.
		/// Falls back to the full item name if it doesn't match the expected English shape.
		/// </summary>
		private static string ShortMateriaName(string itemName)
		{
			var match = Regex.Match(itemName, @"^(?:Craftsman's|Gatherer's)\s+(.+?)\s+Materia\b");
			return match.Success ? match.Groups[1].Value : itemName;
		}
	}
}
