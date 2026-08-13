using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Post-load wiring for ResultHistoryPage.xaml: reads every completed voyage from
	/// VoyageHistoryStore (populated by OceanTrip.LogVoyageResult on each voyage's results
	/// screen), computes all-time stats over the FULL history (or a route-filtered subset — see
	/// the All/Indigo/Ruby pill buttons), and shows the 10 most recent matching runs in the
	/// DataGrid. ShellWindow only reveals this page's nav button once VoyageHistoryStore.HasAny()
	/// is true, so Attach can assume at least one entry exists, but still degrades to zeros/dashes
	/// rather than throwing if that's ever not the case.
	/// </summary>
	public static class ResultHistoryPageBehavior
	{
		public static void Attach(UserControl page)
		{
			var allEntries = VoyageHistoryStore.LoadAll();

			var allButton = (Button)page.FindName("RouteFilterAll");
			var indigoButton = (Button)page.FindName("RouteFilterIndigo");
			var rubyButton = (Button)page.FindName("RouteFilterRuby");
			var filterButtons = new[] { allButton, indigoButton, rubyButton };

			void ApplyFilter(Button active, string route)
			{
				foreach (var btn in filterButtons)
					btn.Tag = btn == active ? "Active" : null;

				var filtered = route == null
					? allEntries
					: allEntries.Where(e => string.Equals(e.Route, route, StringComparison.OrdinalIgnoreCase)).ToList();

				BuildStats(page, filtered);
				BuildRouteBreakdown(page, filtered, showWhenSingleRoute: route == null);
				BuildResultsGrid(page, filtered, route);
			}

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

		private static void BuildRouteBreakdown(UserControl page, List<VoyageHistoryEntry> entries, bool showWhenSingleRoute)
		{
			var group = (GroupBox)page.FindName("RouteBreakdownGroup");
			var panel = (StackPanel)page.FindName("RouteBreakdownPanel");
			panel.Children.Clear();

			var byRoute = entries
				.GroupBy(e => string.IsNullOrEmpty(e.Route) ? "Unknown" : e.Route)
				.OrderByDescending(g => g.Count())
				.ToList();

			// Not meaningful once a single route is already the filter in effect — nothing left to
			// compare against — nor with only one route in the unfiltered history.
			if (!showWhenSingleRoute || byRoute.Count < 2)
			{
				group.Visibility = Visibility.Collapsed;
				return;
			}

			foreach (var routeGroup in byRoute)
			{
				double avgPoints = routeGroup.Average(e => (double)e.TotalPoints);
				int count = routeGroup.Count();
				panel.Children.Add(new TextBlock
				{
					Style = (Style)Application.Current.Resources["BodyText"],
					Margin = new Thickness(0, 0, 0, 6),
					Text = $"{routeGroup.Key} — {count} run{(count == 1 ? "" : "s")}, avg {avgPoints:N0} points"
				});
			}

			group.Visibility = Visibility.Visible;
		}

		private static void BuildResultsGrid(UserControl page, List<VoyageHistoryEntry> entries, string routeFilter)
		{
			if (page.FindName("ResultsGroup") is GroupBox resultsGroup)
				resultsGroup.Header = routeFilter == null ? "Last 10 Runs" : $"Last 10 {routeFilter} Runs";

			var grid = (DataGrid)page.FindName("ResultsGrid");
			grid.ItemsSource = entries
				.OrderByDescending(e => e.Timestamp)
				.Take(10)
				.Select(BuildRow)
				.ToList();
		}

		private static ResultHistoryRow BuildRow(VoyageHistoryEntry entry)
		{
			int totalBonusPercent = entry.Bonuses.Sum(b => b.Multiplier - 100);

			return new ResultHistoryRow
			{
				Timestamp = entry.Timestamp.ToString("MM/dd HH:mm"),
				Route = entry.Route,
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
					.Select(b => new BonusDetailRow { Name = b.Name, PercentText = $"+{b.Multiplier - 100}%" })
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
	}
}
