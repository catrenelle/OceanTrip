using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// New WPF settings shell — replaces FormSettings' "master layout with a sub-window loaded
	/// into a content panel" pattern. Where FormSettings faked embedding a Form inside a Panel
	/// (TopLevel=false, FormBorderStyle=None, Dock=Fill), WPF's ContentControl does this natively:
	/// nav clicks just swap .Content for a different UserControl.
	/// </summary>
	public static class ShellWindow
	{
		private static Window _instance;

		public static void Show()
		{
			if (_instance != null)
			{
				_instance.Show();
				_instance.Activate();
				return;
			}

			var window = XamlLoader.Load<Window>("Shell.xaml");

			var titleBar = (Grid)window.FindName("TitleBar");
			var minimizeButton = (Button)window.FindName("MinimizeButton");
			var maximizeButton = (Button)window.FindName("MaximizeButton");
			var closeButton = (Button)window.FindName("CloseButton");
			var logoImage = (Image)window.FindName("LogoImage");
			var pageContent = (ContentControl)window.FindName("PageContent");
			var statusDot = (System.Windows.Shapes.Ellipse)window.FindName("StatusDot");
			var statusText = (TextBlock)window.FindName("StatusText");

			var navIdleActivities = (Button)window.FindName("NavIdleActivities");
			var navOceanSettings = (Button)window.FindName("NavOceanSettings");
			var navSchedule = (Button)window.FindName("NavSchedule");
			var navCurrentRoute = (Button)window.FindName("NavCurrentRoute");
			var navLisbeth = (Button)window.FindName("NavLisbeth");
			var navLlamaMarket = (Button)window.FindName("NavLlamaMarket");

			logoImage.Source = TryLoadLogoImage();

			// Drag-to-move, but not when the click originated on a title-bar button (e.g. Close) —
			// otherwise DragMove()'s blocking move-loop eats the button's MouseUp/Click.
			titleBar.MouseLeftButtonDown += (s, e) =>
			{
				if (e.OriginalSource is DependencyObject source && IsDescendantOfButton(source))
					return;

				window.DragMove();
			};

			minimizeButton.Click += (s, e) => SystemCommands.MinimizeWindow(window);
			maximizeButton.Click += (s, e) =>
			{
				if (window.WindowState == WindowState.Maximized)
					SystemCommands.RestoreWindow(window);
				else
					SystemCommands.MaximizeWindow(window);
			};
			closeButton.Click += (s, e) => window.Close();

			var navButtons = new[] { navIdleActivities, navOceanSettings, navSchedule, navCurrentRoute };

			void Navigate(Button active, string pagePath, Action<UserControl> postLoad = null)
			{
				foreach (var btn in navButtons)
					btn.Tag = btn == active ? "Active" : null;

				var page = XamlLoader.Load<UserControl>(pagePath);
				postLoad?.Invoke(page);
				pageContent.Content = page;
			}

			navIdleActivities.Click += (s, e) => Navigate(navIdleActivities, "Pages/IdleActivitiesPage.xaml", IdleActivitiesPageBehavior.Attach);
			navOceanSettings.Click += (s, e) => Navigate(navOceanSettings, "Pages/OceanSettingsPage.xaml", OceanSettingsPageBehavior.Attach);
			navSchedule.Click += (s, e) => Navigate(navSchedule, "Pages/SchedulePage.xaml", SchedulePageBehavior.Attach);
			navCurrentRoute.Click += (s, e) => Navigate(navCurrentRoute, "Pages/CurrentRoutePage.xaml", CurrentRoutePageBehavior.Attach);

			navLisbeth.Click += (s, e) => OceanTripPlanner.Helpers.Lisbeth.OpenWindow();
			navLlamaMarket.Click += (s, e) => OceanTripPlanner.Helpers.LlamaMarket.OpenMarketSettings();

			void RefreshStatus()
			{
				bool connected = ff14bot.Core.Me != null;
				statusDot.Fill = (Brush)Application.Current.Resources[connected ? "AccentBrush" : "TextSecondaryBrush"];
				statusText.Text = connected ? $"Connected · {ff14bot.Core.Me.Name}" : "Not Connected";
			}

			RefreshStatus();
			var statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
			statusTimer.Tick += (s, e) => RefreshStatus();
			statusTimer.Start();

			window.Closed += (s, e) =>
			{
				statusTimer.Stop();
				_instance = null;
			};

			Navigate(navIdleActivities, "Pages/IdleActivitiesPage.xaml", IdleActivitiesPageBehavior.Attach);

			_instance = window;
			window.Show();
		}

		private static bool IsDescendantOfButton(DependencyObject source)
		{
			while (source != null)
			{
				if (source is Button)
					return true;

				source = VisualTreeHelper.GetParent(source) ?? LogicalTreeHelper.GetParent(source);
			}

			return false;
		}

		private static BitmapImage TryLoadLogoImage()
		{
			var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };

			foreach (var dir in possibleDirectories)
			{
				var potentialPath = Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "OceanTripNewLogo.png");
				if (File.Exists(potentialPath))
					return new BitmapImage(new Uri(potentialPath, UriKind.Absolute));
			}

			return null;
		}
	}
}
