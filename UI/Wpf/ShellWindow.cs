using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using OceanTripPlanner.Helpers;

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
			var missingFishPanel = (StackPanel)window.FindName("MissingFishPanel");
			var missingFishText = (TextBlock)window.FindName("MissingFishText");
			var resyncFishGlyph = (TextBlock)window.FindName("ResyncFishGlyph");

			var navIdleActivities = (Button)window.FindName("NavIdleActivities");
			var navOceanSettings = (Button)window.FindName("NavOceanSettings");
			var navSchedule = (Button)window.FindName("NavSchedule");
			var navCurrentRoute = (Button)window.FindName("NavCurrentRoute");
			var navResultHistory = (Button)window.FindName("NavResultHistory");
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

			var navButtons = new[] { navIdleActivities, navOceanSettings, navSchedule, navCurrentRoute, navResultHistory };

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
			navResultHistory.Click += (s, e) => Navigate(navResultHistory, "Pages/ResultHistoryPage.xaml", ResultHistoryPageBehavior.Attach);

			navLisbeth.Click += (s, e) => OceanTripPlanner.Helpers.Lisbeth.OpenWindow();
			navLlamaMarket.Click += (s, e) => OceanTripPlanner.Helpers.LlamaMarket.OpenMarketSettings();

			// Konami-code listener for Schedule's "Simulate Current Route" panel — a developer-only
			// feature (drives Current Route without actually being on the boat) that stays hidden from
			// normal use until this exact arrow-key sequence is typed. PreviewKeyDown (tunneling, on
			// the Window) rather than a page-level KeyDown, so it still sees the keys even when focus
			// is inside a ComboBox/DataGrid that would otherwise consume arrow-key input itself.
			var konamiSequence = new[] { Key.Up, Key.Up, Key.Down, Key.Down, Key.Left, Key.Right, Key.Left, Key.Right };
			int konamiProgress = 0;
			window.PreviewKeyDown += (s, e) =>
			{
				bool matched = e.Key == konamiSequence[konamiProgress];
				konamiProgress = matched ? konamiProgress + 1 : (e.Key == konamiSequence[0] ? 1 : 0);

				if (konamiProgress == konamiSequence.Length)
				{
					konamiProgress = 0;
					SchedulePageBehavior.UnlockSimulationPanel();
				}
			};

			// Status-bar Fish Guide reconcile icon: the click can't open the Fish Guide itself (that's a
			// framethread game action), so it just queues a request the botbase drains between casts — see
			// FishingLog.ProcessPendingResync. Spin the glyph until the request clears.
			var resyncRotate = new RotateTransform();
			resyncFishGlyph.RenderTransform = resyncRotate;
			resyncFishGlyph.RenderTransformOrigin = new Point(0.5, 0.5);

			void StartResyncSpinner()
			{
				resyncFishGlyph.Opacity = 1.0;
				var spin = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1))) { RepeatBehavior = RepeatBehavior.Forever };
				resyncRotate.BeginAnimation(RotateTransform.AngleProperty, spin);
			}

			void StopResyncSpinner()
			{
				resyncRotate.BeginAnimation(RotateTransform.AngleProperty, null);
				resyncRotate.Angle = 0;
			}

			resyncFishGlyph.MouseLeftButtonUp += (s, e) =>
			{
				// Guarded to the same condition RefreshStatus enables the glyph under; a stale click that
				// slips through (e.g. bot stopped mid-frame) is a harmless no-op the coroutine never drains.
				if (ff14bot.Core.Me == null || !ff14bot.TreeRoot.IsRunning || global::OceanTrip.FishingLog.ResyncPending)
					return;

				global::OceanTrip.FishingLog.RequestResync();
				StartResyncSpinner();
				resyncFishGlyph.ToolTip = "Reconciling with the in-game Fish Guide…";

				// Poll for completion off the request flag. 300ms × 400 ≈ 2min safety cap: during a long
				// spectral current the pole may not return to PoleReady for a while, so don't give up early —
				// but if the bot stops before draining it, stop spinning eventually. A still-queued request
				// runs later regardless, and the 5s status tick picks up the new count.
				var poll = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
				int ticks = 0;
				poll.Tick += (ts, te) =>
				{
					ticks++;
					if (!global::OceanTrip.FishingLog.ResyncPending || ticks > 400)
					{
						poll.Stop();
						StopResyncSpinner();
						resyncFishGlyph.ToolTip = "Reconcile with the in-game Fish Guide";
						RefreshStatus();
					}
				};
				poll.Start();
			};

			void RefreshStatus()
			{
				bool connected = ff14bot.Core.Me != null;
				statusDot.Fill = (Brush)Application.Current.Resources[connected ? "AccentBrush" : "TextSecondaryBrush"];
				statusText.Text = connected ? $"Connected · {ff14bot.Core.Me.Name}" : "Not Connected";

				// Fish Log progress, visible from every page — MissingFish() is null until a
				// character is connected and InitializeFishLog() has run, not just "zero missing".
				var missingFish = connected ? global::OceanTrip.FishingLog.MissingFish() : null;
				if (missingFish == null)
				{
					missingFishPanel.Visibility = Visibility.Collapsed;
				}
				else
				{
					int total = Ocean_Trip.Definitions.FishDataCache.GetFish().Count;
					missingFishText.Text = missingFish.Count == 0
						? "No missing fish"
						: $"{missingFish.Count}/{total} fish missing";
					missingFishPanel.Visibility = Visibility.Visible;

					// Refresh glyph is clickable only while a resync isn't already running and the botbase is
					// running (the only time the queued request can actually be drained) — dim + non-interactive
					// otherwise, with a tooltip that says why. While a resync is pending the spinner owns the
					// glyph, so leave its opacity/hit-testing to the click handler.
					if (global::OceanTrip.FishingLog.ResyncPending)
					{
						resyncFishGlyph.IsHitTestVisible = false;
					}
					else
					{
						bool canResync = ff14bot.TreeRoot.IsRunning;
						resyncFishGlyph.IsHitTestVisible = canResync;
						resyncFishGlyph.Opacity = canResync ? 1.0 : 0.35;
						resyncFishGlyph.ToolTip = canResync
							? "Reconcile with the in-game Fish Guide"
							: "Start the bot to reconcile the Fish Guide";
					}
				}

				// One-way reveal: once a voyage's ever been logged, VoyageHistoryStore's backing file
				// never goes away, so there's no case where this needs to go back to Collapsed.
				// Skipping the File.Exists check once already visible avoids re-hitting disk every tick.
				if (navResultHistory.Visibility != Visibility.Visible && VoyageHistoryStore.HasAny())
					navResultHistory.Visibility = Visibility.Visible;
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
			var path = BotBasesResourceLocator.Resolve("Resources", "OceanTripNewLogo.png");
			return path == null ? null : new BitmapImage(new Uri(path, UriKind.Absolute));
		}
	}
}
