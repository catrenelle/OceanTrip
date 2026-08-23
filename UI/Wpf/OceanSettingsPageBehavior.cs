using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ff14bot.Managers;
using Ocean_Trip.UI.Wpf.Converters;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for OceanSettingsPage.xaml: sets the settings DataContext and builds the
	/// bait-inventory and achievement-progress icon grids in code, since both need bindings to
	/// FFXIV_Databinds.Instance — a different object than the page's main DataContext — which is
	/// simplest done here rather than via clr-namespace x:Static references in loose XAML.
	/// </summary>
	public static class OceanSettingsPageBehavior
	{
		// Label is only a fallback for when the client's item cache hasn't loaded the localized
		// name yet (see BuildBaitTile) — ItemId is what actually drives the tooltip, so non-English
		// clients see the bait's name in their own language instead of this hardcoded English one.
		// IsLure marks items that work like Heavy Steel Jig: not consumed per-catch like regular
		// bait, so you only ever need 1 in inventory — those should only flag low-stock at 0, not
		// against the (bait-oriented) Restock Threshold.
		private static readonly (string Label, uint ItemId, int IconX, int IconY, string CountProperty, bool IsLure)[] BaitItems =
		{
			("Ragworm", FishBait.Ragworm, 8, 23, nameof(FFXIV_Databinds.ragwormCount), false),
			("Krill", FishBait.Krill, 9, 23, nameof(FFXIV_Databinds.krillCount), false),
			("Plump Worm", FishBait.PlumpWorm, 10, 23, nameof(FFXIV_Databinds.plumpwormCount), false),
			("Rat Tail", FishBait.RatTail, 2, 23, nameof(FFXIV_Databinds.rattailCount), false),
			("Glow Worm", FishBait.GlowWorm, 3, 23, nameof(FFXIV_Databinds.glowwormCount), false),
			("Heavy Steel Jig", FishBait.HeavySteelJig, 5, 23, nameof(FFXIV_Databinds.heavysteeljigCount), true),
			("Shrimp Cage Feeder", FishBait.ShrimpCageFeeder, 4, 23, nameof(FFXIV_Databinds.shrimpcagefeederCount), false),
			("Pill Bug", FishBait.PillBug, 1, 23, nameof(FFXIV_Databinds.pillbugCount), false),
			("Squid Strip", FishBait.SquidStrip, 7, 23, nameof(FFXIV_Databinds.squidstripCount), false),
			("Mackerel Strip", FishBait.MackerelStrip, 2, 24, nameof(FFXIV_Databinds.mackerelstripCount), false),
			("Stonefly Nymph", FishBait.StoneflyNymph, 6, 23, nameof(FFXIV_Databinds.stoneflynymphCount), false),
		};

		// Split by what the achievement is actually scoped to (route-specific creature/point-total
		// vs. overall-points-across-both-routes), rather than one flat list — mirrors the Indigo/Ruby
		// Achievement Focus cards above them, and keeps "3M" from reading as if it were a Ruby thing
		// just because it happened to render next to Ruby 16K.
		private static readonly (string Label, int IconX, int IconY, string AchievedProperty)[] IndigoAchievementItems =
		{
			("Mantas", 9, 32, nameof(FFXIV_Databinds.achievementMantas)),
			("Octopods", 3, 32, nameof(FFXIV_Databinds.achievementOctopods)),
			("Sharks", 4, 32, nameof(FFXIV_Databinds.achievementSharks)),
			("Jellyfish", 5, 32, nameof(FFXIV_Databinds.achievementJellyfish)),
			("Seadragons", 6, 32, nameof(FFXIV_Databinds.achievementSeadragons)),
			("Balloons", 7, 32, nameof(FFXIV_Databinds.achievementBalloons)),
			("Crabs", 8, 32, nameof(FFXIV_Databinds.achievementCrabs)),
			("Indigo 5K", 4, 27, nameof(FFXIV_Databinds.achievement5kindigo)),
			("Indigo 10K", 4, 27, nameof(FFXIV_Databinds.achievement10kindigo)),
			("Indigo 16K", 4, 27, nameof(FFXIV_Databinds.achievement16kindigo)),
			("Indigo 20K", 4, 27, nameof(FFXIV_Databinds.achievement20kindigo)),
		};

		private static readonly (string Label, int IconX, int IconY, string AchievedProperty)[] RubyAchievementItems =
		{
			("Shrimp", 4, 34, nameof(FFXIV_Databinds.achievementShrimp)),
			("Shellfish", 2, 34, nameof(FFXIV_Databinds.achievementShellfish)),
			("Squid", 3, 34, nameof(FFXIV_Databinds.achievementSquid)),
#if !RB_TC
			("Mantis", 5, 34, nameof(FFXIV_Databinds.achievementMantisShrimp)),
			("Prehistoric", 6, 34, nameof(FFXIV_Databinds.achievementPrehistoric)),
#endif
			("Ruby 5K", 9, 27, nameof(FFXIV_Databinds.achievement5kruby)),
			("Ruby 10K", 9, 27, nameof(FFXIV_Databinds.achievement10kruby)),
			("Ruby 16K", 9, 27, nameof(FFXIV_Databinds.achievement16kruby)),
		};

		private static readonly (string Label, int IconX, int IconY, string AchievedProperty)[] OverallAchievementItems =
		{
			("100K", 1, 27, nameof(FFXIV_Databinds.achievement100koverall)),
			("500K", 1, 27, nameof(FFXIV_Databinds.achievement500koverall)),
			("1M", 1, 27, nameof(FFXIV_Databinds.achievement1moverall)),
			("3M", 1, 27, nameof(FFXIV_Databinds.achievement3moverall)),
		};

		public static void Attach(UserControl page)
		{
			page.DataContext = OceanTripNewSettings.Instance;

#if RB_TC
			if (page.FindName("MantisFocusRadio") is UIElement mantisFocusRadio)
				mantisFocusRadio.Visibility = Visibility.Collapsed;
			if (page.FindName("PrehistoricFocusRadio") is UIElement prehistoricFocusRadio)
				prehistoricFocusRadio.Visibility = Visibility.Collapsed;
#endif

			var baitPanel = (WrapPanel)page.FindName("BaitIconPanel");
			foreach (var item in BaitItems)
				baitPanel.Children.Add(BuildBaitTile(item.Label, item.ItemId, item.IconX, item.IconY, item.CountProperty, item.IsLure));

			var indigoAchievementPanel = (WrapPanel)page.FindName("IndigoAchievementIconPanel");
			foreach (var item in IndigoAchievementItems)
				indigoAchievementPanel.Children.Add(BuildAchievementTile(item.Label, item.IconX, item.IconY, item.AchievedProperty));

			var rubyAchievementPanel = (WrapPanel)page.FindName("RubyAchievementIconPanel");
			foreach (var item in RubyAchievementItems)
				rubyAchievementPanel.Children.Add(BuildAchievementTile(item.Label, item.IconX, item.IconY, item.AchievedProperty));

			var overallAchievementPanel = (WrapPanel)page.FindName("OverallAchievementIconPanel");
			foreach (var item in OverallAchievementItems)
				overallAchievementPanel.Children.Add(BuildAchievementTile(item.Label, item.IconX, item.IconY, item.AchievedProperty));

			WireStepper(page, "LureMaxStacksUpButton", "LureMaxStacksDownButton",
				() => OceanTripNewSettings.Instance.LureMaxStacks, v => OceanTripNewSettings.Instance.LureMaxStacks = v);
			WireStepper(page, "RestockThresholdUpButton", "RestockThresholdDownButton",
				() => OceanTripNewSettings.Instance.BaitRestockThreshold, v => OceanTripNewSettings.Instance.BaitRestockThreshold = v);
			WireStepper(page, "RestockAmountUpButton", "RestockAmountDownButton",
				() => OceanTripNewSettings.Instance.BaitRestockAmount, v => OceanTripNewSettings.Instance.BaitRestockAmount = v);
		}

		/// <summary>
		/// Wires a pair of up/down RepeatButtons (click-and-hold repeats) to a numeric settings
		/// property, floored at 0. The TextBox next to them already binds to the same property, so
		/// mutating it here just flows back through the existing binding — no separate sync needed.
		/// </summary>
		private static void WireStepper(UserControl page, string upName, string downName, Func<int> get, Action<int> set)
		{
			var up = (RepeatButton)page.FindName(upName);
			var down = (RepeatButton)page.FindName(downName);

			up.Click += (s, e) => set(get() + 1);
			down.Click += (s, e) => set(Math.Max(0, get() - 1));
		}

		private static FrameworkElement BuildBaitTile(string label, uint itemId, int iconX, int iconY, string countProperty, bool isLure)
		{
			var stack = new StackPanel { Margin = new Thickness(0, 0, 8, 4), Width = 58 };

			string localeName = DataManager.ItemCache[itemId]?.CurrentLocaleName ?? label;
			var tile = new Border { Style = (Style)Application.Current.Resources["IconTile"] };
			var image = new Image { Source = IconAtlas.GetIcon(iconX, iconY), Width = 26, Height = 26, ToolTip = localeName };
			tile.Child = image;
			stack.Children.Add(tile);

			var count = new TextBlock
			{
				Style = (Style)Application.Current.Resources["SecondaryText"],
				TextAlignment = TextAlignment.Center
			};
			count.SetBinding(TextBlock.TextProperty, new Binding(countProperty) { Source = FFXIV_Databinds.Instance });

			// Flags a bait as low-stock (colored) once its count drops to/below Restock Threshold, so
			// this row doubles as an at-a-glance "what's about to restock" view. Lures (isLure) don't
			// get consumed per-catch like regular bait — you only ever need 1 — so they're only
			// flagged once actually at 0, regardless of the (bait-oriented) threshold.
			var stockWarning = new MultiBinding
			{
				Converter = (BaitStockToBrushConverter)Application.Current.Resources["BaitStockToBrushConverter"],
				ConverterParameter = isLure ? "ZeroOnly" : null
			};
			stockWarning.Bindings.Add(new Binding(countProperty) { Source = FFXIV_Databinds.Instance });
			stockWarning.Bindings.Add(new Binding(nameof(OceanTripNewSettings.BaitRestockThreshold)) { Source = OceanTripNewSettings.Instance });
			count.SetBinding(TextBlock.ForegroundProperty, stockWarning);

			stack.Children.Add(count);

			return stack;
		}

		private static FrameworkElement BuildAchievementTile(string label, int iconX, int iconY, string achievedProperty)
		{
			var stack = new StackPanel { Margin = new Thickness(0, 0, 8, 4), Width = 58 };

			var tile = new Border { Style = (Style)Application.Current.Resources["IconTile"] };
			var image = new Image { Width = 26, Height = 26, ToolTip = label };
			image.SetBinding(Image.SourceProperty, new Binding(achievedProperty)
			{
				Source = FFXIV_Databinds.Instance,
				Converter = (AchievementIconConverter)Application.Current.Resources["AchievementIconConverter"],
				ConverterParameter = $"{iconX},{iconY}"
			});
			tile.Child = image;
			stack.Children.Add(tile);

			var text = new TextBlock
			{
				Text = label,
				Style = (Style)Application.Current.Resources["SecondaryText"],
				TextAlignment = TextAlignment.Center,
				TextWrapping = TextWrapping.Wrap,
				FontSize = 11
			};
			stack.Children.Add(text);

			return stack;
		}
	}
}
