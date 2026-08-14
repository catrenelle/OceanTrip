using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OceanTripPlanner.Helpers;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for ResultHistoryPage.xaml: reads every completed voyage from
	/// VoyageHistoryStore (populated by OceanTrip.LogVoyageResult on each voyage's results
	/// screen), computes all-time stats over the FULL history (or a route-filtered subset — see
	/// the All/Indigo/Ruby pill buttons), and shows the most recent matching runs (newest first,
	/// capped at MAX_PAGED_RUNS) in the DataGrid, paged PAGE_SIZE at a time via the Prev/Next
	/// buttons. ShellWindow only reveals this page's nav button once VoyageHistoryStore.HasAny()
	/// is true, so Attach can assume at least one entry exists, but still degrades to zeros/dashes
	/// rather than throwing if that's ever not the case.
	/// </summary>
	public static class ResultHistoryPageBehavior
	{
		/// <summary>
		/// Maps IKDContentBonus objective names to real icon atlas coordinates. Verified 2026-08-13:
		/// Resources/icons.png is pixel-identical, row for row, to the sprite sheet the community
		/// site ffxiv.pf-n.co/ocean-fishing/bonuses itself uses (confirmed via a full pixel diff —
		/// only 4 unrelated cells in the very last row differ, none of them bonus icons) — that
		/// site's own JS bundle embeds the exact grid position for every content-bonus ID, which is
		/// the raw IKDContentBonus row ID. Several IDs share one objective name (the "(Target number
		/// adjusted for party size)" duplicate rows for smaller parties) and get separate sprite
		/// cells despite being the same bonus — this dictionary keeps only the canonical (lowest ID,
		/// non-adjusted) cell per unique name, since VoyageHistoryEntry only ever stores the name.
		/// </summary>
		private static readonly Dictionary<string, (int X, int Y)> AtlasBonusIcons = new Dictionary<string, (int X, int Y)>
		{
			["Ocean Fishing Amateur"] = (1, 29),
			["Ocean Fishing Enthusiast"] = (2, 29),
			["Ocean Fishing Fanatic"] = (3, 29),
			["Small Fish in a Big Pond"] = (4, 29),
			["Big Fish in a Small Pond"] = (5, 29),
			["A Rare Catch"] = (6, 29),
			["Catch of a Lifetime"] = (7, 29),
			["Give a Man a Fish"] = (8, 29),
			["Teach a Man to Fish"] = (9, 29),
			["Bream Team: Galadion Bay"] = (10, 29),
			["Bream Team: Southern Strait of Merlthor"] = (3, 30),
			["Bream Team: Cieldalaes"] = (6, 30),
			["Bream Team: Northern Strait of Merlthor"] = (9, 30),
			["Bream Team: Rhotano Sea"] = (2, 31),
			["Bream Team: Bloodbrine Sea"] = (5, 31),
			["Bream Team: Rothlyt Sound"] = (8, 31),
			["Fabled Fishers"] = (1, 32),
			["Favored by Llymlaen"] = (2, 32),
			["Octopus Travelers"] = (3, 32),
			["Certifiable Shark Hunters"] = (4, 32),
			["Jelled Together"] = (5, 32),
			["Maritime Dragonslayers"] = (6, 32),
			["Balloon Catchers"] = (7, 32),
			["Crab Boat Crew"] = (8, 32),
			["Sticking it to the Manta"] = (9, 32),
			["Bream Team: Sirensong Sea"] = (10, 32),
			["Bream Team: Kugane Coast"] = (3, 33),
			["Bream Team: Ruby Price"] = (6, 33),
			["Bream Team: Lower One River"] = (9, 33),
			["Maximum Mussel"] = (2, 34),
			["Squid Squadron"] = (3, 34),
			["Shrimp Smorgasbord"] = (4, 34),
			// Not present in the pf-n.co grid this table was sourced from (both added in a later
			// patch than that site's data) - reuse SchedulePageBehavior's existing Achievement icon
			// coordinates for the same two creatures instead of leaving them star-only.
			["Time Waits for No Mantis"] = SchedulePageBehavior.ObjectiveIcons["Mantis"],
			["Prehistoric Professionals"] = SchedulePageBehavior.ObjectiveIcons["Prehistoric"],
		};

		/// <summary>
		/// The two 7.5-patch Bream Team zones missing from the atlas — confirmed (via a pixel diff
		/// against every existing Bream Team cell, 93%+ different from all of them) to be genuinely
		/// new art, not a reuse of an existing icon, so pulled in as standalone files instead of
		/// atlas coordinates. Source: ffxiv.consolegameswiki.com's per-bonus icon images.
		/// </summary>
		private static readonly Dictionary<string, string> FileBonusIcons = new Dictionary<string, string>
		{
			["Bream Team: Unnamed Island"] = "BreamTeamUnnamedIsland.png",
			["Bream Team: Thavnairian Coast"] = "BreamTeamThavnairianCoast.png",
		};

		private static readonly Dictionary<string, ImageSource> BonusIcons = BuildBonusIcons();

		private static Dictionary<string, ImageSource> BuildBonusIcons()
		{
			var icons = new Dictionary<string, ImageSource>();

			foreach (var kvp in AtlasBonusIcons)
				icons[kvp.Key] = IconAtlas.GetIcon(kvp.Value.X, kvp.Value.Y);

			foreach (var kvp in FileBonusIcons)
			{
				var path = BotBasesResourceLocator.Resolve("Resources", kvp.Value);
				if (path != null)
					icons[kvp.Key] = new BitmapImage(new Uri(path, UriKind.Absolute));
			}

			return icons;
		}

		private const int PAGE_SIZE = 10;
		private const int MAX_PAGED_RUNS = 100;

		public static void Attach(UserControl page)
		{
			var allEntries = VoyageHistoryStore.LoadAll();

			var allButton = (Button)page.FindName("RouteFilterAll");
			var indigoButton = (Button)page.FindName("RouteFilterIndigo");
			var rubyButton = (Button)page.FindName("RouteFilterRuby");
			var filterButtons = new[] { allButton, indigoButton, rubyButton };

			var prevButton = (Button)page.FindName("ResultsPrevButton");
			var nextButton = (Button)page.FindName("ResultsNextButton");

			// Most-recent-first, capped at MAX_PAGED_RUNS — All-Time Stats below still sees the full
			// (unfiltered-by-cap) `filtered` set from ApplyFilter, only the runs table itself is
			// paged. Both re-set on every filter change; currentPage resets to page 1.
			List<VoyageHistoryEntry> pageableEntries = new List<VoyageHistoryEntry>();
			string currentRouteLabel = null;
			int currentPage = 0;

			void RenderResultsPage()
			{
				if (page.FindName("ResultsGroup") is GroupBox resultsGroup)
					resultsGroup.Header = currentRouteLabel == null ? "Runs" : $"{currentRouteLabel} Runs";

				int totalPages = Math.Max(1, (int)Math.Ceiling(pageableEntries.Count / (double)PAGE_SIZE));
				currentPage = Math.Max(0, Math.Min(currentPage, totalPages - 1));

				var grid = (DataGrid)page.FindName("ResultsGrid");
				grid.ItemsSource = pageableEntries
					.Skip(currentPage * PAGE_SIZE)
					.Take(PAGE_SIZE)
					.Select(BuildRow)
					.ToList();

				var pageLabel = (TextBlock)page.FindName("ResultsPageLabel");
				pageLabel.Text = pageableEntries.Count == 0
					? "No runs"
					: $"Page {currentPage + 1} of {totalPages} ({pageableEntries.Count} runs)";

				prevButton.IsEnabled = currentPage > 0;
				nextButton.IsEnabled = currentPage < totalPages - 1;
			}

			void ApplyFilter(Button active, string route)
			{
				foreach (var btn in filterButtons)
					btn.Tag = btn == active ? "Active" : null;

				var filtered = route == null
					? allEntries
					: allEntries.Where(e => string.Equals(e.Route, route, StringComparison.OrdinalIgnoreCase)).ToList();

				if (page.FindName("StatsGroup") is GroupBox statsGroup)
					statsGroup.Header = route == null ? "All-Time Stats" : $"{route} Stats";
				BuildStats(page, filtered);

				currentRouteLabel = route;
				pageableEntries = filtered.OrderByDescending(e => e.Timestamp).Take(MAX_PAGED_RUNS).ToList();
				currentPage = 0;
				RenderResultsPage();
			}

			prevButton.Click += (s, e) => { currentPage--; RenderResultsPage(); };
			nextButton.Click += (s, e) => { currentPage++; RenderResultsPage(); };

			allButton.Click += (s, e) => ApplyFilter(allButton, null);
			indigoButton.Click += (s, e) => ApplyFilter(indigoButton, "Indigo");
			rubyButton.Click += (s, e) => ApplyFilter(rubyButton, "Ruby");

			ApplyFilter(allButton, null);
		}

		private static void BuildStats(UserControl page, List<VoyageHistoryEntry> entries)
		{
			var panel = (UniformGrid)page.FindName("StatsPanel");
			panel.Children.Clear();

			var ranked = entries.Where(e => e.Placement.HasValue).ToList();

			panel.Children.Add(BuildStatTile(entries.Count.ToString(), "Runs Logged"));
			panel.Children.Add(BuildStatTile(
				entries.Count > 0 ? entries.Average(e => (double)e.TotalPoints).ToString("N0") : "—",
				"Avg Points"));
			panel.Children.Add(BuildStatTile(
				entries.Count > 0 ? entries.Max(e => e.TotalPoints).ToString("N0") : "—",
				"Best Run"));
			panel.Children.Add(BuildStatTile(
				ranked.Count > 0 ? ranked.Average(e => (double)e.Placement.Value).ToString("N1") : "—",
				"Avg Placement"));
			panel.Children.Add(BuildStatTile(
				entries.Count > 0 ? entries.Average(e => (double)e.CaughtFish).ToString("N1") : "—",
				"Avg Fish Caught"));
		}

		private static ResultHistoryRow BuildRow(VoyageHistoryEntry entry)
		{
			int totalBonusPercent = entry.Bonuses.Sum(b => b.Multiplier - 100);

			return new ResultHistoryRow
			{
				Timestamp = entry.Timestamp.ToString("MM/dd HH:mm"),
				Route = entry.Route,
				ZoneName = string.IsNullOrEmpty(entry.ZoneName) ? "—" : entry.ZoneName,
				TimeOfDay = string.IsNullOrEmpty(entry.TimeOfDay) ? "—" : entry.TimeOfDay,
				Points = entry.TotalPoints.ToString("N0"),
				Placement = entry.Placement.HasValue
					? $"{Ordinal(entry.Placement.Value)} of {entry.TrackedPlayerCount}"
					: "Unranked",
				CaughtFish = entry.CaughtFish,
				HasBonus = totalBonusPercent > 0,
				BonusPercentText = totalBonusPercent > 0 ? $"+{totalBonusPercent}%" : "—",
				BonusSummaryHeader = entry.Bonuses.Count > 0
					? $"Bonuses Earned (+{totalBonusPercent}% total)"
					: "No bonuses earned this run.",
				BonusDetails = entry.Bonuses
					.OrderBy(b => b.Id)
					.Select(b => new BonusDetailRow
					{
						Name = b.Name,
						PercentText = $"+{b.Multiplier - 100}%",
						IconSource = BonusIcons.TryGetValue(b.Name, out var icon) ? icon : null,
					})
					.ToList(),
			};
		}

		private static Border BuildStatTile(string value, string label)
		{
			var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
			stack.Children.Add(new TextBlock
			{
				Text = value,
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 24,
				FontWeight = FontWeights.SemiBold,
				Foreground = (Brush)Application.Current.Resources["AccentLightBrush"],
				HorizontalAlignment = HorizontalAlignment.Center
			});
			stack.Children.Add(new TextBlock
			{
				Text = label,
				Style = (Style)Application.Current.Resources["SecondaryText"],
				FontSize = 11,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 3, 0, 0)
			});

			return new Border
			{
				Child = stack,
				Background = (Brush)Application.Current.Resources["SurfaceHoverBrush"],
				CornerRadius = new CornerRadius(8),
				Padding = new Thickness(10, 12, 10, 10),
				Margin = new Thickness(4)
			};
		}

		private static string Ordinal(int n)
		{
			if (n % 100 is 11 or 12 or 13)
				return $"{n}th";

			return (n % 10) switch
			{
				1 => $"{n}st",
				2 => $"{n}nd",
				3 => $"{n}rd",
				_ => $"{n}th",
			};
		}
	}

	public class ResultHistoryRow
	{
		public string Timestamp { get; set; }
		public string Route { get; set; }
		public string ZoneName { get; set; }
		public string TimeOfDay { get; set; }
		public string Points { get; set; }
		public string Placement { get; set; }
		public int CaughtFish { get; set; }
		public bool HasBonus { get; set; }
		public string BonusPercentText { get; set; }
		public string BonusSummaryHeader { get; set; }
		public List<BonusDetailRow> BonusDetails { get; set; }
	}

	public class BonusDetailRow
	{
		public string Name { get; set; }
		public string PercentText { get; set; }

		/// <summary>Null for bonuses with no known real icon (see ResultHistoryPageBehavior.BonusIcons)
		/// — the XAML template draws a fallback star underneath, which shows through whenever this
		/// is null since nothing paints over it.</summary>
		public ImageSource IconSource { get; set; }
	}
}
