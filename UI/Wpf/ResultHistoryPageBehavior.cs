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
	/// screen), computes all-time stats over the FULL history, and shows the 10 most recent runs
	/// in the DataGrid. ShellWindow only reveals this page's nav button once
	/// VoyageHistoryStore.HasAny() is true, so Attach can assume at least one entry exists, but
	/// still degrades to zeros/dashes rather than throwing if that's ever not the case.
	/// </summary>
	public static class ResultHistoryPageBehavior
	{
		public static void Attach(UserControl page)
		{
			var entries = VoyageHistoryStore.LoadAll();

			BuildStats(page, entries);
			BuildRouteBreakdown(page, entries);
			BuildResultsGrid(page, entries);
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

		private static void BuildRouteBreakdown(UserControl page, List<VoyageHistoryEntry> entries)
		{
			var group = (GroupBox)page.FindName("RouteBreakdownGroup");
			var panel = (StackPanel)page.FindName("RouteBreakdownPanel");
			panel.Children.Clear();

			var byRoute = entries
				.GroupBy(e => string.IsNullOrEmpty(e.Route) ? "Unknown" : e.Route)
				.OrderByDescending(g => g.Count())
				.ToList();

			if (byRoute.Count < 2)
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

		private static void BuildResultsGrid(UserControl page, List<VoyageHistoryEntry> entries)
		{
			var grid = (DataGrid)page.FindName("ResultsGrid");
			grid.ItemsSource = entries
				.OrderByDescending(e => e.Timestamp)
				.Take(10)
				.Select(BuildRow)
				.ToList();
		}

		private static ResultHistoryRow BuildRow(VoyageHistoryEntry entry) => new ResultHistoryRow
		{
			Timestamp = entry.Timestamp.ToString("MM/dd HH:mm"),
			Route = entry.Route,
			Points = entry.TotalPoints.ToString("N0"),
			Placement = entry.Placement.HasValue
				? $"{Ordinal(entry.Placement.Value)} of {entry.TrackedPlayerCount}"
				: "Unranked",
			CaughtFish = entry.CaughtFish,
			Bonuses = entry.Bonuses.Count > 0 ? string.Join(", ", entry.Bonuses) : "None",
		};

		private static Border BuildStatTile(string value, string label)
		{
			var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
			stack.Children.Add(new TextBlock
			{
				Text = value,
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 22,
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
				Margin = new Thickness(0, 2, 0, 0)
			});
			return new Border { Child = stack, Padding = new Thickness(4, 6, 4, 2) };
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
		public string Bonuses { get; set; }
	}
}
