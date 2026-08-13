using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using OceanTripPlanner.Helpers;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Loads plain .xaml files from the deployed BotBase folder via XamlReader.Parse at runtime,
	/// since RebornBuddy's Roslyn-scripting compiler can't run MSBuild's XAML markup-compile step
	/// (confirmed: it only references assemblies already loaded in-process, not files on disk, and
	/// has no XAML build task at all). Same Resources/*.json path-resolution pattern already used
	/// by FishDataCache/RouteDataCache/MissionDataCache.
	/// </summary>
	public static class XamlLoader
	{
		private static bool _stylesLoaded;

		/// <summary>
		/// Loads a .xaml file (path relative to UI/Wpf/Xaml) and parses it into a live object tree.
		/// Ensures the shared Styles.xaml dictionary is available via StaticResource/DynamicResource
		/// lookups by merging it into Application.Current.Resources (created once, never Run() —
		/// we're hosted inside RebornBuddy's own message loop, not a standalone WPF app).
		/// </summary>
		public static T Load<T>(string relativeXamlPath) where T : class
		{
			EnsureApplicationInitialized();

			var xaml = File.ReadAllText(ResolvePath(relativeXamlPath));
			if (XamlReader.Parse(xaml) is not T element)
				throw new InvalidOperationException($"'{relativeXamlPath}' did not parse to a {typeof(T).Name}.");

			if (element is DependencyObject root)
				ApplyExplicitControlStyles(root);

			return element;
		}

		/// <summary>
		/// Implicit (TargetType-only, no x:Key) styles from the merged Application.Resources
		/// dictionary don't reliably auto-apply to RadioButton/CheckBox/TextBox elements parsed
		/// via XamlReader.Parse in this RebornBuddy-hosted setup — they silently fall back to stock
		/// OS control chrome instead of our custom templates. Named x:Key styles resolved via
		/// DynamicResource/StaticResource (Card, NavButton, etc.) aren't affected, so the fix is to
		/// walk the freshly-parsed logical tree and assign the equivalent named style explicitly.
		/// </summary>
		private static void ApplyExplicitControlStyles(DependencyObject node)
		{
			switch (node)
			{
				case RadioButton radioButton:
					radioButton.Style = (Style)Application.Current.Resources["AppRadioButton"];
					break;
				case CheckBox checkBox:
					checkBox.Style = (Style)Application.Current.Resources["AppCheckBox"];
					break;
				case TextBox textBox:
					textBox.Style = (Style)Application.Current.Resources["AppTextBox"];
					break;
				case ScrollViewer scrollViewer:
					HookScrollBarStyling(scrollViewer);
					break;
				case DataGrid dataGrid:
					// DataGrid scrolls via its own internal ScrollViewer template part, exactly like
					// the bare ScrollViewer case above — same fix, same reason.
					HookScrollBarStyling(dataGrid);
					break;
			}

			// LogicalTreeHelper (not VisualTreeHelper) — the visual tree for template-driven
			// children isn't realized yet at this point since template application is exactly
			// what we're working around, but the logical (XAML-declared) content tree is available
			// immediately after parsing.
			foreach (var child in LogicalTreeHelper.GetChildren(node))
			{
				if (child is DependencyObject childObject)
					ApplyExplicitControlStyles(childObject);
			}
		}

		/// <summary>
		/// ScrollBar is a different case from RadioButton/CheckBox/TextBox above: it isn't hand-authored
		/// in any page's XAML at all, so it never appears in the logical-tree walk above — ScrollViewer
		/// (and DataGrid, which hosts its own internal ScrollViewer template part) manufactures its
		/// PART_VerticalScrollBar/PART_HorizontalScrollBar internally when ITS OWN control template is
		/// applied. For an element that starts Visibility="Collapsed" until data loads (e.g.
		/// CurrentRoutePage's ContentScroll), that can happen well after this page's initial Loaded —
		/// so both Loaded and the Collapsed-to-Visible IsVisibleChanged transition are hooked. Each
		/// handler walks the VISUAL tree (VisualTreeHelper, not LogicalTreeHelper — the scrollbar parts
		/// only exist there) rooted at this specific element, not the whole page.
		/// </summary>
		private static void HookScrollBarStyling(FrameworkElement element)
		{
			element.Loaded += (_, _) => ApplyScrollBarStyle(element);
			element.IsVisibleChanged += (_, _) => ApplyScrollBarStyle(element);
		}

		private static void ApplyScrollBarStyle(DependencyObject node)
		{
			if (node is ScrollBar scrollBar)
				scrollBar.Style = (Style)Application.Current.Resources["AppScrollBar"];

			int childCount = VisualTreeHelper.GetChildrenCount(node);
			for (int i = 0; i < childCount; i++)
				ApplyScrollBarStyle(VisualTreeHelper.GetChild(node, i));
		}

		private static void EnsureApplicationInitialized()
		{
			if (Application.Current == null)
				new Application();

			if (_stylesLoaded)
				return;

			var stylesXaml = File.ReadAllText(ResolvePath("Styles.xaml"));
			var styles = (ResourceDictionary)XamlReader.Parse(stylesXaml);

			// Registered here (not as XAML markup) to avoid relying on clr-namespace assembly-name
			// resolution for a loose, non-compiled XAML file — this assembly's runtime name under
			// RebornBuddy's compiler isn't something we can reliably hardcode into a XAML xmlns.
			styles["EnumConverter"] = new Converters.EnumToBooleanConverter();
			styles["AchievementIconConverter"] = new Converters.AchievementIconConverter();

			Application.Current.Resources.MergedDictionaries.Add(styles);
			_stylesLoaded = true;
		}

		private static string ResolvePath(string relativeXamlPath)
		{
			return BotBasesResourceLocator.Resolve("UI", "Wpf", "Xaml", relativeXamlPath)
				?? throw new FileNotFoundException($"WPF XAML file not found: {relativeXamlPath}");
		}
	}
}
