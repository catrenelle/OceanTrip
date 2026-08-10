using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using OceanTripPlanner;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for CurrentRoutePage.xaml. Unlike the other pages, this one polls live game
	/// state on a timer (route/leg, weather, catch log) rather than binding to a settings singleton,
	/// so the fish grid and bait recommendations stay current while the player is out on the boat.
	/// Bait recommendations use the curated Route.NormalBait/SpectralBait values from
	/// fishingRoutes.json (the same baseline the live bot itself falls back to) rather than
	/// reimplementing NormalBaitSelector/SpectralBaitSelector's full Intuition/mooch-chain logic,
	/// which is stateful and has side effects (bait changes, chain tracking) unsafe to call from UI code.
	/// </summary>
	public static class CurrentRoutePageBehavior
	{
		// Same Resources/icons.png atlas coordinates as OceanSettingsPageBehavior.BaitItems.
		private static readonly Dictionary<uint, (int X, int Y)> BaitIcons = new Dictionary<uint, (int X, int Y)>
		{
			[OceanTripPlanner.Definitions.FishBait.PillBug] = (1, 23),
			[OceanTripPlanner.Definitions.FishBait.RatTail] = (2, 23),
			[OceanTripPlanner.Definitions.FishBait.GlowWorm] = (3, 23),
			[OceanTripPlanner.Definitions.FishBait.ShrimpCageFeeder] = (4, 23),
			[OceanTripPlanner.Definitions.FishBait.HeavySteelJig] = (5, 23),
			[OceanTripPlanner.Definitions.FishBait.StoneflyNymph] = (6, 23),
			[OceanTripPlanner.Definitions.FishBait.SquidStrip] = (7, 23),
			[OceanTripPlanner.Definitions.FishBait.Ragworm] = (8, 23),
			[OceanTripPlanner.Definitions.FishBait.Krill] = (9, 23),
			[OceanTripPlanner.Definitions.FishBait.PlumpWorm] = (10, 23),
			[OceanTripPlanner.Definitions.FishBait.MackerelStrip] = (2, 24),
		};

		private static readonly Dictionary<string, Color> RarityColors = new Dictionary<string, Color>
		{
			["Rare"] = Color.FromRgb(0xE0, 0xB8, 0x4A),
			["Uncommon"] = Color.FromRgb(0x9C, 0xB4, 0xD6),
		};

		private static readonly Color DefaultAccentColor = Color.FromRgb(0x33, 0x36, 0x48);
		private static readonly Color UnavailableOverlayColor = Color.FromArgb(0x70, 0xE0, 0x3B, 0x3B);
		private static readonly Color HighlightBackgroundColor = Color.FromArgb(0x33, 0x00, 0x89, 0xC6);
		private static readonly Color SubtypeBadgeBackground = Color.FromArgb(0xCC, 0x12, 0x14, 0x1C);

		// Fish.Achievement stores the singular category tag ("Crab", "Shrimp", ...) that
		// AchievementFishDataCache already maps to an AchievementType for the settings-page focus
		// picker; Schedule's ObjectiveIcons atlas is keyed by the plural label the game itself shows
		// ("Crabs", "Shrimp"). This bridges the two so both pages draw from the same icon set instead
		// of a second copy of the atlas coordinates.
		private static readonly Dictionary<Ocean_Trip.Definitions.AchievementType, string> AchievementIconLabels =
			new Dictionary<Ocean_Trip.Definitions.AchievementType, string>
			{
				[Ocean_Trip.Definitions.AchievementType.Mantas] = "Mantas",
				[Ocean_Trip.Definitions.AchievementType.Octopods] = "Octopods",
				[Ocean_Trip.Definitions.AchievementType.Sharks] = "Sharks",
				[Ocean_Trip.Definitions.AchievementType.Jellyfish] = "Jellyfish",
				[Ocean_Trip.Definitions.AchievementType.Seadragons] = "Seadragons",
				[Ocean_Trip.Definitions.AchievementType.Balloons] = "Balloons",
				[Ocean_Trip.Definitions.AchievementType.Crabs] = "Crabs",
				[Ocean_Trip.Definitions.AchievementType.Shrimp] = "Shrimp",
				[Ocean_Trip.Definitions.AchievementType.Shellfish] = "Shellfish",
				[Ocean_Trip.Definitions.AchievementType.Squid] = "Squid",
				[Ocean_Trip.Definitions.AchievementType.MantisShrimp] = "Mantis",
				[Ocean_Trip.Definitions.AchievementType.Prehistoric] = "Prehistoric",
			};

		/// <summary>
		/// Resolves the subtype badge icon for a fish tile — the corner dot's replacement. Rare
		/// signature fish (Sothis, Coral Manta, ...) store their own display name directly in
		/// Achievement and match a Schedule objective-icon entry by that exact name; everything else
		/// goes through the singular-tag -> plural-label bridge above. Returns null (no badge) rather
		/// than a placeholder when neither resolves, since most fish have no special subtype at all.
		/// </summary>
		private static ImageSource SubtypeIcon(Ocean_Trip.Definitions.Fish fish)
		{
			if (string.IsNullOrEmpty(fish.Achievement))
				return null;

			var direct = SchedulePageBehavior.GetSubtypeIcon(fish.Achievement);
			if (direct != null)
				return direct;

			var achievementType = Ocean_Trip.Definitions.AchievementFishDataCache.MapAchievementString(fish.Achievement);
			return achievementType != Ocean_Trip.Definitions.AchievementType.None
				&& AchievementIconLabels.TryGetValue(achievementType, out var label)
				? SchedulePageBehavior.GetSubtypeIcon(label)
				: null;
		}

		/// <summary>
		/// An active mission with 2+ catches still needed — the threshold below which Double/Triple
		/// Hook isn't worth the GP, since a single normal hook would finish it just as fast.
		/// </summary>
		private class MissionDHOpportunity
		{
			public TugType? TugType;
			public string CategoryTag;
			public string MissionText;
			public int Remaining;
		}

		public static void Attach(UserControl page)
		{
			if (page.FindName("RouteBannerBrush") is ImageBrush bannerBrush)
				bannerBrush.ImageSource = LoadBannerImage();

			Refresh(page);

			var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
			timer.Tick += (s, e) => Refresh(page);
			timer.Start();

			// Each Attach() call gets its own timer instance captured by this closure, so pages
			// navigated away from stop only their own timer — no shared/static state to leak.
			page.Unloaded += (s, e) => timer.Stop();
		}

		private static BitmapImage _cachedBannerImage;

		/// <summary>
		/// Static and decorative (unlike the fish/mission data), so it's loaded once per process and
		/// reused across every navigation to this page rather than re-reading the file each time.
		/// </summary>
		private static BitmapImage LoadBannerImage()
		{
			if (_cachedBannerImage != null)
				return _cachedBannerImage;

			var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };
			foreach (var dir in possibleDirectories)
			{
				var path = System.IO.Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "OceanFishingBanner.png");
				if (System.IO.File.Exists(path))
				{
					_cachedBannerImage = new BitmapImage(new Uri(path, UriKind.Absolute));
					return _cachedBannerImage;
				}
			}

			return null;
		}

		private static void Refresh(UserControl page)
		{
			try
			{
				RefreshCore(page);
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Current Route page refresh failed: {ex}");
				ShowEmptyState(page);
			}
		}

		private static void RefreshCore(UserControl page)
		{
			var endeavor = new Endeavor();
			endeavor.CheckDirector();

			bool simulating = RouteSimulation.Enabled;
			string detectedRoute;
			uint leg;

			if (simulating)
			{
				detectedRoute = RouteSimulation.Route;
				leg = (uint)Math.Max(0, Math.Min(2, RouteSimulation.Leg));
			}
			else
			{
				if (endeavor.CurrentZone > 2)
				{
					ShowEmptyState(page);
					return;
				}

				detectedRoute = WorldManager.RawZoneId == OceanTripPlanner.Definitions.Zones.TheEndeavor ? "Indigo" : "Ruby";
				leg = endeavor.CurrentZone;
			}

			var schedule = Ocean_Trip.Definitions.Routes.GetSchedule(route: detectedRoute);
			if (schedule == null || leg >= schedule.Length)
			{
				ShowEmptyState(page);
				return;
			}

			string location = schedule[leg].Item1;
			string timeOfDay = schedule[leg].Item2;
			if (string.IsNullOrEmpty(timeOfDay))
				timeOfDay = "Day";
			string weather = WorldManager.CurrentWeather;

			var routeInfo = Ocean_Trip.Definitions.RouteDataCache.GetRoutesWithFish()
				.FirstOrDefault(x => x.Route.RouteShortName == location);

			ShowContent(page);

			((FrameworkElement)page.FindName("SimulationBanner")).Visibility = simulating ? Visibility.Visible : Visibility.Collapsed;

			var headerText = (TextBlock)page.FindName("RouteHeaderText");
			var subText = (TextBlock)page.FindName("RouteSubText");
			headerText.Text = routeInfo?.Route.RouteName ?? Schedule.areaName(location);
			subText.Text = $"{timeOfDay} · Stop {leg + 1} of 3 · {weather}";

			var missingFish = global::OceanTrip.FishingLog.MissingFish() ?? new HashSet<uint>();

			bool IsAvailable(Ocean_Trip.Definitions.Fish fish) =>
				fish.TimeOfDayExclusion1 != timeOfDay && fish.TimeOfDayExclusion2 != timeOfDay &&
				fish.WeatherExclusion1 != weather && fish.WeatherExclusion2 != weather;

			var normalFish = routeInfo?.NormalFish?.Where(f => f != null).ToList() ?? new List<Ocean_Trip.Definitions.Fish>();
			var spectralFish = routeInfo?.SpectralFish?.Where(f => f != null).ToList() ?? new List<Ocean_Trip.Definitions.Fish>();

			// While simulating, the real Endeavor mission slots are all zero (not actually on a
			// voyage) — use the random simulated set instead so the Missions card and DH/TH mission
			// badges have something to exercise. Both consumers just need a flat (type, progress)
			// list, so build it once here regardless of source.
			(uint Type, ushort Progress)[] missionSlots = simulating
				? RouteSimulation.Missions.Select(m => (m.MissionId, m.Progress)).ToArray()
				: new[]
					{
						(endeavor.Mission1Type, endeavor.Mission1Progress),
						(endeavor.Mission2Type, endeavor.Mission2Progress),
						(endeavor.Mission3Type, endeavor.Mission3Progress),
					};

			BuildMissionsCard(page, missionSlots);
			var missionOpportunities = GetMissionDHOpportunities(missionSlots);

			BuildFishPanel((WrapPanel)page.FindName("NormalFishPanel"), normalFish, missingFish, IsAvailable, missionOpportunities, timeOfDay, weather);
			BuildFishPanel((WrapPanel)page.FindName("SpectralFishPanel"), spectralFish, missingFish, IsAvailable, missionOpportunities, timeOfDay, weather);

			BuildBaitCards(page, routeInfo, normalFish, spectralFish, missingFish, IsAvailable);

			BuildFishLogTarget(page, normalFish, spectralFish, missingFish, IsAvailable, missionOpportunities, timeOfDay, weather);
		}

		/// <summary>
		/// Active missions where Double/Triple Hook would meaningfully speed things up: a single
		/// successful DH/TH catches several fish sharing one bite at once, so it advances a
		/// bite-strength or category mission by 2-3 in one hookset instead of one cast at a time.
		/// Pure reads (Endeavor's memory reads + MissionDataCache's cached JSON), same safety as
		/// the tug-only version this replaces — no side effects, safe from the UI refresh loop.
		/// </summary>
		private static List<MissionDHOpportunity> GetMissionDHOpportunities((uint Type, ushort Progress)[] slots)
		{
			var opportunities = new List<MissionDHOpportunity>();

			foreach (var (type, progress) in slots)
			{
				if (type == 0)
					continue;

				var condition = Ocean_Trip.Definitions.MissionDataCache.GetById(type);
				if (condition == null)
					continue;

				int remaining = condition.Count - progress;
				if (remaining < 2)
					continue;

				var tugType = Ocean_Trip.Definitions.MissionDataCache.GetRequiredTugType(condition.Text);
				if (tugType.HasValue)
				{
					opportunities.Add(new MissionDHOpportunity { TugType = tugType, MissionText = condition.Text, Remaining = remaining });
					continue;
				}

				var categoryTags = Ocean_Trip.Definitions.MissionDataCache.GetRequiredAchievementTags(condition.Text);
				if (categoryTags != null)
				{
					foreach (var tag in categoryTags)
						opportunities.Add(new MissionDHOpportunity { CategoryTag = tag, MissionText = condition.Text, Remaining = remaining });
				}
			}

			return opportunities;
		}

		/// <summary>
		/// Lives in the right column of the banner now (not its own card), so instead of a
		/// Visibility toggle, an empty mission list collapses the column's width to 0 — the left
		/// column's Width="*" then fills the freed space automatically, matching the "don't reserve
		/// empty space" goal without needing to re-flow anything by hand.
		/// </summary>
		private static void BuildMissionsCard(UserControl page, (uint Type, ushort Progress)[] slots)
		{
			var columnDef = (ColumnDefinition)page.FindName("MissionsColumnDef");
			var panel = (StackPanel)page.FindName("MissionsPanel");
			panel.Children.Clear();

			bool any = false;
			foreach (var (type, progress) in slots)
			{
				if (type == 0)
					continue;

				var condition = Ocean_Trip.Definitions.MissionDataCache.GetById(type);
				if (condition == null)
					continue;

				any = true;
				panel.Children.Add(BuildMissionRow(condition, progress));
			}

			columnDef.Width = any ? new GridLength(210) : new GridLength(0);
			((FrameworkElement)page.FindName("MissionsColumn")).Visibility = any ? Visibility.Visible : Visibility.Collapsed;
		}

		private static FrameworkElement BuildMissionRow(Ocean_Trip.Definitions.IkdMissionCondition condition, ushort progress)
		{
			bool complete = progress >= condition.Count;

			var row = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

			row.Children.Add(new TextBlock
			{
				Text = condition.Text,
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 11,
				Foreground = Brushes.White,
				TextWrapping = TextWrapping.Wrap,
			});

			// trackWidth 150 (not the column's full ~210) leaves room for the progress text to sit
			// beside the bar instead of wrapping underneath it.
			const double trackWidth = 150;
			double fraction = condition.Count > 0 ? Math.Min(1.0, (double)progress / condition.Count) : 0;

			var progressRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };

			// Explicit HorizontalAlignment.Left, not the FrameworkElement default of Stretch — Stretch
			// with an explicit Width centers the element within its allotted space instead of pinning
			// it to the left edge, which is what made the bar look indented relative to the mission
			// text above it.
			var trackGrid = new Grid { Width = trackWidth, Height = 5, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
			trackGrid.Children.Add(new Border
			{
				Width = trackWidth,
				Height = 5,
				CornerRadius = new CornerRadius(2.5),
				Background = new SolidColorBrush(Color.FromArgb(0x50, 0xFF, 0xFF, 0xFF)),
			});
			trackGrid.Children.Add(new Border
			{
				Width = trackWidth * fraction,
				Height = 5,
				CornerRadius = new CornerRadius(2.5),
				Background = (Brush)Application.Current.Resources[complete ? "AccentLightBrush" : "AccentBrush"],
				HorizontalAlignment = HorizontalAlignment.Left,
			});
			progressRow.Children.Add(trackGrid);

			progressRow.Children.Add(new TextBlock
			{
				Text = $"{progress}/{condition.Count}",
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 10,
				Foreground = new SolidColorBrush(Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF)),
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(8, 0, 0, 0),
			});

			row.Children.Add(progressRow);

			return row;
		}

		private static void ShowEmptyState(UserControl page)
		{
			((FrameworkElement)page.FindName("EmptyStatePanel")).Visibility = Visibility.Visible;
			((FrameworkElement)page.FindName("ContentScroll")).Visibility = Visibility.Collapsed;
		}

		private static void ShowContent(UserControl page)
		{
			((FrameworkElement)page.FindName("EmptyStatePanel")).Visibility = Visibility.Collapsed;
			((FrameworkElement)page.FindName("ContentScroll")).Visibility = Visibility.Visible;
		}

		private static void BuildBaitCards(UserControl page, Ocean_Trip.Definitions.RouteWithFish routeInfo,
			List<Ocean_Trip.Definitions.Fish> normalFish, List<Ocean_Trip.Definitions.Fish> spectralFish,
			HashSet<uint> missingFish, Func<Ocean_Trip.Definitions.Fish, bool> isAvailable)
		{
			var normalPanel = (Panel)page.FindName("NormalBaitPanel");
			var spectralPanel = (Panel)page.FindName("SpectralBaitPanel");
			normalPanel.Children.Clear();
			spectralPanel.Children.Clear();

			if (routeInfo == null)
			{
				normalPanel.Children.Add(new TextBlock
				{
					Text = "No route data available for this stop.",
					FontFamily = new FontFamily("Segoe UI"),
					FontSize = 11,
					Foreground = new SolidColorBrush(Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF)),
					TextWrapping = TextWrapping.Wrap,
				});
				return;
			}

			// Trigger Spectral sits under Normal (same column) rather than alongside During
			// Spectral/For Sothis — it's still the "normal fishing" phase of the stop, just aimed at
			// procuring Spectral Current, whereas the second column is entirely about what to do once
			// Spectral Current has already started.
			normalPanel.Children.Add(BuildBaitRow("Normal", routeInfo.Route.NormalBait));

			var spectralTriggerBait = normalFish
				.Where(f => f.CausesSpectral && isAvailable(f))
				.GroupBy(f => f.FavoriteBait)
				.OrderByDescending(g => g.Count())
				.Select(g => (uint?)g.Key)
				.FirstOrDefault() ?? routeInfo.Route.NormalBait;

			normalPanel.Children.Add(BuildBaitRow("Trigger Spectral", spectralTriggerBait));

			// Route.SpectralBait (fishingRoutes.json) is inconsistently curated — sometimes it's the
			// route's signature Rare fish's own FavoriteBait, sometimes (when that fish has no bait
			// preference) it's something else entirely. Compute the actual best-overall bait instead,
			// using the same missing-fish-first/points-fallback logic BaitRanker already applies for
			// normal fishing, run against the full available spectral-fish pool for this stop.
			var availableSpectralFish = spectralFish.Where(isAvailable).ToList();
			uint bestSpectralBait = OceanTripPlanner.Strategies.BaitRanker.SelectBaitForMissingFish(availableSpectralFish, missingFish);
			if (bestSpectralBait == 0)
				bestSpectralBait = OceanTripPlanner.Strategies.BaitRanker.SelectBaitForPoints(availableSpectralFish);
			if (bestSpectralBait == 0)
				bestSpectralBait = routeInfo.Route.SpectralBait;

			spectralPanel.Children.Add(BuildBaitRow("During Spectral", bestSpectralBait));

			// The route's signature Rare fish (e.g. Sothis at Galadion Bay) is usually the whole
			// point of chasing Spectral Current — call out its specific favorite bait separately
			// from the "best overall" recommendation above, since they're often different baits.
			// Some signature fish (Glass Dragon, Seafaring Toad) have no bait preference at all,
			// in which case there's nothing meaningful to show here.
			var specialFish = spectralFish.FirstOrDefault(f => f.Rarity == "Rare");
			if (specialFish != null && specialFish.FavoriteBait != 0)
				spectralPanel.Children.Add(BuildBaitRow($"For {specialFish.FishName}", specialFish.FavoriteBait));
		}

		private static void BuildFishLogTarget(UserControl page, List<Ocean_Trip.Definitions.Fish> normalFish,
			List<Ocean_Trip.Definitions.Fish> spectralFish, HashSet<uint> missingFish, Func<Ocean_Trip.Definitions.Fish, bool> isAvailable,
			List<MissionDHOpportunity> missionOpportunities, string timeOfDay, string weather)
		{
			var group = (GroupBox)page.FindName("FishLogTargetGroup");
			var panel = (WrapPanel)page.FindName("FishLogTargetPanel");

			bool focusFishLog = OceanTripNewSettings.Instance.FishPriority == FishPriority.FishLog
				|| OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto;

			if (!focusFishLog)
			{
				group.Visibility = Visibility.Collapsed;
				return;
			}

			group.Visibility = Visibility.Visible;
			panel.Children.Clear();

			var target = normalFish.Concat(spectralFish)
				.Where(f => !f.RequiresIntuition && isAvailable(f) && missingFish.Contains((uint)f.FishID))
				.OrderBy(f => RarityOrder(f.Rarity))
				.ThenByDescending(f => f.Points)
				.FirstOrDefault();

			panel.Children.Add(target != null
				? BuildFishTile(target, caught: false, available: true, highlight: true, missionOpportunities, timeOfDay, weather)
				: new TextBlock
				{
					Text = "No priority fish remaining at this stop — nice work!",
					Style = (Style)Application.Current.Resources["SecondaryText"],
					Margin = new Thickness(4)
				});
		}

		private static void BuildFishPanel(WrapPanel panel, List<Ocean_Trip.Definitions.Fish> fishList,
			HashSet<uint> missingFish, Func<Ocean_Trip.Definitions.Fish, bool> isAvailable, List<MissionDHOpportunity> missionOpportunities,
			string timeOfDay, string weather)
		{
			panel.Children.Clear();

			if (fishList.Count == 0)
			{
				panel.Children.Add(new TextBlock
				{
					Text = "No fish data available for this stop.",
					Style = (Style)Application.Current.Resources["SecondaryText"],
					Margin = new Thickness(4)
				});
				return;
			}

			foreach (var fish in fishList)
			{
				bool caught = !missingFish.Contains((uint)fish.FishID);
				bool available = isAvailable(fish);
				panel.Children.Add(BuildFishTile(fish, caught, available, highlight: false, missionOpportunities, timeOfDay, weather));
			}
		}

		private static FrameworkElement BuildFishTile(Ocean_Trip.Definitions.Fish fish, bool caught, bool available, bool highlight,
			List<MissionDHOpportunity> missionOpportunities, string timeOfDay, string weather)
		{
			var accentColor = !string.IsNullOrEmpty(fish.Rarity) && RarityColors.TryGetValue(fish.Rarity, out var rarityColor)
				? rarityColor
				: DefaultAccentColor;

			var border = new Border
			{
				Width = 76,
				Margin = new Thickness(0, 0, 8, 8),
				CornerRadius = new CornerRadius(6),
				Background = highlight
					? new SolidColorBrush(HighlightBackgroundColor)
					: (Brush)Application.Current.Resources["SurfaceBrush"],
				BorderBrush = new SolidColorBrush(accentColor),
				BorderThickness = new Thickness(highlight ? 2 : 1),
				Padding = new Thickness(4),
				Opacity = available ? 1.0 : 0.55,
			};

			var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
			var iconGrid = new Grid { Width = 40, Height = 40, HorizontalAlignment = HorizontalAlignment.Center };

			ImageSource iconSource = caught
				? (ImageSource)IconAtlas.GetIcon(fish.IconX, fish.IconY)
				: IconAtlas.GetIconGrayscale(fish.IconX, fish.IconY);
			iconGrid.Children.Add(new Image { Source = iconSource, Width = 40, Height = 40 });

			if (!available)
			{
				iconGrid.Children.Add(new Rectangle
				{
					Width = 40,
					Height = 40,
					Fill = new SolidColorBrush(UnavailableOverlayColor),
					RadiusX = 3,
					RadiusY = 3,
				});
			}

			// Caught/uncaught and available/unavailable already read from the icon itself (grayscale
			// vs. color, and the dimmed+red-overlaid state above) — the rarity-colored border does the
			// same job the old status dot did. This corner badge instead names the fish's special
			// subtype (Crab, Shrimp, ...), the same icon Schedule's objective columns use.
			var subtypeIcon = SubtypeIcon(fish);
			if (subtypeIcon != null)
			{
				iconGrid.Children.Add(new Border
				{
					Width = 14,
					Height = 14,
					CornerRadius = new CornerRadius(3),
					Background = new SolidColorBrush(SubtypeBadgeBackground),
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Top,
					Margin = new Thickness(0, -3, -3, 0),
					Child = new Image { Source = subtypeIcon, Width = 11, Height = 11, Margin = new Thickness(1) },
				});
			}

			// DH/TH badge — DHBonus/THBonus > 1 means Double/Triple Hook nets extra copies of this
			// fish from one hookset. That's worth flagging for three independent reasons:
			//  - Achievement mode: grinding a catch-count, so any extra copy helps regardless of value.
			//  - An active mission (remaining >= 2): handled above via missionOpportunities.
			//  - Points/Auto priority: worth it only if the extra copies' points clear the hook's GP
			//    cost — the same math HookingStrategy.IsPointsWorthDoubleHook uses live. Spectral fish
			//    (usually much higher Points) clear this far more often than normal fish, which is why
			//    DH/TH should read as "mostly spectral" in practice — via the real math, not a rule
			//    that arbitrarily excludes the Normal Fish panel.
			bool dhBonusAvailable = fish.THBonus > 1 || fish.DHBonus > 1;

			bool pointsMode = OceanTripNewSettings.Instance.FishPriority == FishPriority.Points
				|| OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto;
			bool pointsWorthTriple = fish.THBonus > 1
				&& fish.Points * fish.THBonus > OceanTripPlanner.Definitions.FishingConstants.TRIPLE_HOOK_GP_COST;
			bool pointsWorthDouble = fish.DHBonus > 1
				&& fish.Points * fish.DHBonus > OceanTripPlanner.Definitions.FishingConstants.DOUBLE_HOOK_GP_COST;
			bool pointsWorthwhile = pointsMode && (pointsWorthTriple || pointsWorthDouble);

			var missionMatch = missionOpportunities?.FirstOrDefault(o =>
				(o.TugType.HasValue && fish.BiteType == o.TugType.Value) ||
				(o.CategoryTag != null && fish.Achievement == o.CategoryTag));

			// A mission match only actually speeds things up if this fish's own DH/TH bonus is > 1 —
			// that bonus IS "how many of this fish one hookset nets," so without it, DH/TH wouldn't
			// advance the mission any faster than a normal hook would, tug/category match or not.
			bool missionBoost = missionMatch != null && dhBonusAvailable;

			bool achievementMode = OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements;
			bool showDhBadge = dhBonusAvailable && (achievementMode || missionBoost || pointsWorthwhile);

			// Prefer whichever tier is actually the reason for the badge: achievement/mission cases
			// don't care about the point math, so Triple is preferred whenever available (it subsumes
			// Double); a pure points-mode badge should name the tier that actually clears its GP cost.
			string hookTier = (pointsWorthwhile && !achievementMode && !missionBoost)
				? (pointsWorthTriple ? "Triple" : "Double")
				: (fish.THBonus > 1 ? "Triple" : "Double");

			if (showDhBadge)
			{
				iconGrid.Children.Add(new Border
				{
					// Mission-relevant catches get the brighter accent so they stand out from a
					// plain "good for achievement grinding" DH/TH badge.
					Background = (Brush)Application.Current.Resources[missionBoost ? "AccentLightBrush" : "AccentBrush"],
					CornerRadius = new CornerRadius(3),
					Padding = new Thickness(3, 0, 3, 0),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Top,
					Margin = new Thickness(-2, -2, 0, 0),
					Child = new TextBlock
					{
						Text = fish.THBonus > 1 ? "TH" : "DH",
						FontSize = 8,
						FontWeight = FontWeights.Bold,
						Foreground = Brushes.White,
					}
				});
			}

			stack.Children.Add(iconGrid);
			stack.Children.Add(new TextBlock
			{
				Text = fish.FishName,
				Style = (Style)Application.Current.Resources[caught ? "BodyText" : "SecondaryText"],
				FontSize = 9,
				TextWrapping = TextWrapping.Wrap,
				TextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 3, 0, 0),
			});

			border.Child = stack;

			var statusText = !available ? "Unavailable at this stop" : caught ? "Already caught" : "Not yet caught";
			var tooltip = $"{fish.FishName}\n{(string.IsNullOrEmpty(fish.Rarity) ? "" : fish.Rarity + " · ")}{statusText}";
			if (fish.RequiresIntuition)
				tooltip += "\nRequires Fisher's Intuition";

			// Name exactly which exclusion(s) triggered and the value being checked against — makes
			// it possible to tell at a glance whether an "unavailable" reads wrong because of stale
			// upstream data (WorldManager.CurrentWeather) vs. this comparison itself being wrong.
			if (!available)
			{
				var reasons = new List<string>();
				if (fish.TimeOfDayExclusion1 == timeOfDay || fish.TimeOfDayExclusion2 == timeOfDay)
					reasons.Add($"time of day is {timeOfDay}");
				if (fish.WeatherExclusion1 == weather || fish.WeatherExclusion2 == weather)
					reasons.Add($"weather is {weather}");
				if (reasons.Count > 0)
					tooltip += $"\nExcluded: {string.Join(", ", reasons)}";
			}

			if (missionBoost)
				tooltip += $"\n{hookTier} Hook here advances \"{missionMatch.MissionText}\" ({missionMatch.Remaining} more needed)";
			else if (showDhBadge && achievementMode)
				tooltip += $"\n{hookTier} Hook worth it here for achievement progress";
			else if (showDhBadge && pointsWorthwhile)
				tooltip += $"\n{hookTier} Hook worth it here for points";
			else if (missionMatch != null)
				tooltip += $"\nCounts toward \"{missionMatch.MissionText}\" ({missionMatch.Remaining} more needed)";
			border.ToolTip = tooltip;

			return border;
		}

		/// <summary>
		/// Compact icon + two-line (muted caption / bold name) row — designed to sit stacked three
		/// deep in the banner's left column rather than as its own standalone card.
		/// </summary>
		private static FrameworkElement BuildBaitRow(string label, uint baitItemId)
		{
			var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };

			if (baitItemId != 0 && BaitIcons.TryGetValue(baitItemId, out var iconCoords))
			{
				row.Children.Add(new Image
				{
					Source = IconAtlas.GetIcon(iconCoords.X, iconCoords.Y),
					Width = 22,
					Height = 22,
					Margin = new Thickness(0, 0, 8, 0)
				});
			}

			var textStack = new StackPanel();
			textStack.Children.Add(new TextBlock
			{
				Text = label,
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 9,
				Foreground = new SolidColorBrush(Color.FromArgb(0xAA, 0xFF, 0xFF, 0xFF)),
			});
			// DataManager.ItemCache[id] can come back null for a valid item ID while the game's item
			// sheet is still warming up (e.g. right after boarding, mid zone-transition) — this page
			// polls every 5s via a live DispatcherTimer, unlike the other ItemCache call sites in this
			// project (settings dialogs opened well after the character is fully loaded), so it's the
			// one place that needs to tolerate a transient miss instead of assuming the cache is hot.
			string baitName = "—";
			if (baitItemId != 0)
				baitName = DataManager.ItemCache[baitItemId]?.CurrentLocaleName ?? "—";

			textStack.Children.Add(new TextBlock
			{
				Text = baitName,
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 12,
				FontWeight = FontWeights.SemiBold,
				Foreground = Brushes.White,
			});
			row.Children.Add(textStack);

			return row;
		}

		private static int RarityOrder(string rarity)
		{
			switch (rarity)
			{
				case "Rare": return 0;
				case "Uncommon": return 1;
				case "Common": return 2;
				default: return 3;
			}
		}
	}
}
