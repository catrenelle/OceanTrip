using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OceanTripPlanner;
using OceanTripPlanner.Helpers;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for SchedulePage.xaml: builds the schedule grid rows in code from
	/// Schedule.GetSchedules() and resolves objective/time-of-day icons, since DataGrid rows
	/// need plain bindable properties rather than the loose XAML's declarative bindings.
	/// </summary>
	public static class SchedulePageBehavior
	{
		// Atlas coordinates match objectiveImages() in the old FormSchedule and the
		// AchievementItems table in OceanSettingsPageBehavior — same Resources/icons.png sheet.
		// Internal (not private) so CurrentRoutePageBehavior can reuse the same icon set for its
		// per-fish subtype badge instead of maintaining a second copy of these coordinates.
		internal static readonly Dictionary<string, (int X, int Y)> ObjectiveIcons = new Dictionary<string, (int X, int Y)>
		{
			["Mantas"] = (9, 32),
			["Octopods"] = (3, 32),
			["Sharks"] = (4, 32),
			["Jellyfish"] = (5, 32),
			["Seadragons"] = (6, 32),
			["Balloons"] = (7, 32),
			["Crabs"] = (8, 32),
			["Coral Manta"] = (10, 4),
			["Sothis"] = (10, 2),
			["Elasmosaurus"] = (10, 6),
			["Stonescale"] = (10, 8),
			["Hafgufa"] = (10, 10),
			["Seafaring Toad"] = (10, 12),
			["Placodus"] = (10, 14),
			["Shellfish"] = (2, 34),
			["Squid"] = (3, 34),
			["Shrimp"] = (4, 34),
			["Taniwha"] = (10, 16),
			["Glass Dragon"] = (10, 18),
			["Hells' Claw"] = (10, 20),
			["Jewel of Plum Spring"] = (10, 22),
#if !RB_TC
			["Mantis"] = (5, 34),
			["Prehistoric"] = (6, 34),
			["Akupara"] = (7, 34),
			["Manasvin"] = (8, 34),
#endif
		};

		private static readonly Dictionary<string, BitmapImage> ImageCache = new Dictionary<string, BitmapImage>();

		// Set for the rest of this RebornBuddy session once ShellWindow's Konami-code listener sees
		// Up Up Down Down Left Right Left Right. SchedulePage is reloaded fresh via XamlLoader.Load
		// on every nav click (ShellWindow.Navigate), so this — not just the live GroupBox reference
		// below — is what makes the reveal stick across navigating away and back.
		private static bool _simulationUnlocked;

		// The currently-attached page's panel, so UnlockSimulationPanel can reveal it immediately if
		// the code is entered while already sitting on the Schedule page (AttachSimulationControls
		// already ran and set Collapsed before the code was known).
		private static GroupBox _activeSimulationGroup;

		public static void Attach(UserControl page)
		{
			var routeCombo = (ComboBox)page.FindName("RouteCombo");
			var grid = (DataGrid)page.FindName("ScheduleGrid");

			routeCombo.SelectionChanged += (s, e) => Refresh(grid, SelectedRouteName(routeCombo));

			// Setting SelectedIndex (from the unset -1) fires SelectionChanged above, which
			// performs the initial load — no separate first Refresh() call needed.
			routeCombo.SelectedIndex = OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Indigo ? 0 : 1;

			AttachSimulationControls(page);
		}

		/// <summary>
		/// "Simulate Current Route" panel — lets Current Route be exercised/designed against without
		/// actually being on the boat, by writing into the shared RouteSimulation state that
		/// CurrentRoutePageBehavior checks before falling back to live Endeavor memory reads.
		/// </summary>
		private static void AttachSimulationControls(UserControl page)
		{
			var group = (GroupBox)page.FindName("SimulateGroup");
			_activeSimulationGroup = group;
			group.Visibility = _simulationUnlocked ? Visibility.Visible : Visibility.Collapsed;

			var enabledCheck = (CheckBox)page.FindName("SimulateEnabledCheck");
			var simRouteCombo = (ComboBox)page.FindName("SimRouteCombo");
			var simLegCombo = (ComboBox)page.FindName("SimLegCombo");
			var rerollButton = (Button)page.FindName("RerollMissionsButton");

			// Turning simulation on always deals a fresh set of 3 random missions, so Current Route's
			// Missions card and DH/TH mission badges have something to show instead of reading the
			// real (inactive, all-zero) Endeavor mission slots while not actually on a voyage.
			enabledCheck.Checked += (s, e) =>
			{
				RouteSimulation.Enabled = true;
				RouteSimulation.RerollMissions();
			};
			enabledCheck.Unchecked += (s, e) => RouteSimulation.Enabled = false;
			simRouteCombo.SelectionChanged += (s, e) => RouteSimulation.Route = SelectedRouteName(simRouteCombo);
			simLegCombo.SelectionChanged += (s, e) => RouteSimulation.Leg = Math.Max(0, simLegCombo.SelectedIndex);
			rerollButton.Click += (s, e) => RouteSimulation.RerollMissions();

			enabledCheck.IsChecked = RouteSimulation.Enabled;
			simRouteCombo.SelectedIndex = RouteSimulation.Route == "Ruby" ? 1 : 0;
			simLegCombo.SelectedIndex = RouteSimulation.Leg;
		}

		/// <summary>
		/// Called by ShellWindow's Konami-code key listener once it sees the full Up Up Down Down
		/// Left Right Left Right sequence. Reveals "Simulate Current Route" immediately if Schedule
		/// is the active page, and marks it unlocked for the rest of the session either way.
		/// </summary>
		public static void UnlockSimulationPanel()
		{
			_simulationUnlocked = true;
			if (_activeSimulationGroup != null)
				_activeSimulationGroup.Visibility = Visibility.Visible;
		}

		private static string SelectedRouteName(ComboBox combo) =>
			(combo.SelectedItem as ComboBoxItem)?.Content as string ?? "Indigo";

		private static void Refresh(DataGrid grid, string routeName)
		{
			var schedules = Schedule.GetSchedules(18, routeName);
			grid.ItemsSource = schedules?.Select(BuildRow).ToList();
		}

		private static ScheduleRow BuildRow(Schedule schedule)
		{
			var objectives = schedule.objectives
				.Split(',')
				.Select(o => o.Trim())
				.Where(o => o.Length > 0)
				.ToArray();

			var objective1 = objectives.ElementAtOrDefault(0);
			var objective2 = objectives.ElementAtOrDefault(1);

			return new ScheduleRow
			{
				Date = schedule.day,
				Time = schedule.time,
				Route = schedule.routeName,
				TimeOfDayIcon = TimeOfDayImage(schedule.routeTime),
				Objective1Icon = ObjectiveImage(objective1),
				Objective1Name = objective1 ?? "",
				Objective2Icon = ObjectiveImage(objective2),
				Objective2Name = objective2 ?? "",
			};
		}

		private static ImageSource TimeOfDayImage(string routeTime) => routeTime switch
		{
			"Day" => LoadImage("day.png"),
			"Sunset" => LoadImage("sunset.png"),
			"Night" => LoadImage("night.png"),
			_ => LoadImage("day.png"),
		};

		private static ImageSource ObjectiveImage(string objective)
		{
			if (string.IsNullOrEmpty(objective) || !ObjectiveIcons.TryGetValue(objective, out var coords))
				return LoadImage("blank.png");

			return IconAtlas.GetIcon(coords.X, coords.Y);
		}

		/// <summary>
		/// Looks up an objective-category icon by label (e.g. "Crabs", "Sothis") without the
		/// "no match -> blank.png" fallback ObjectiveImage uses for DataGrid cells — callers that
		/// want to skip the badge entirely on no match (rather than show a blank square) use this.
		/// </summary>
		public static ImageSource GetSubtypeIcon(string label)
		{
			if (string.IsNullOrEmpty(label) || !ObjectiveIcons.TryGetValue(label, out var coords))
				return null;

			return IconAtlas.GetIcon(coords.X, coords.Y);
		}

		private static BitmapImage LoadImage(string fileName)
		{
			if (ImageCache.TryGetValue(fileName, out var cached))
				return cached;

			var path = BotBasesResourceLocator.Resolve("Resources", fileName);
			if (path == null)
				return null;

			var image = new BitmapImage(new Uri(path, UriKind.Absolute));
			ImageCache[fileName] = image;
			return image;
		}
	}

	public class ScheduleRow
	{
		public string Date { get; set; }
		public string Time { get; set; }
		public string Route { get; set; }
		public ImageSource TimeOfDayIcon { get; set; }
		public ImageSource Objective1Icon { get; set; }
		public string Objective1Name { get; set; }
		public ImageSource Objective2Icon { get; set; }
		public string Objective2Name { get; set; }
	}
}
