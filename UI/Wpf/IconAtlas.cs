using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using OceanTripPlanner.Helpers;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Crops icons from Resources/icons.png (a 10x34 sprite sheet), same math as
	/// UIElements.getIconImage, but reading the loose file from disk like every other
	/// Resources/* asset in this codebase instead of relying on embedded .resx resources.
	/// </summary>
	public static class IconAtlas
	{
		private const int Columns = 10;
		private const int Rows = 34;

		private static BitmapImage _sheet;

		/// <summary>
		/// Gets icon at 1-based atlas coordinates (x, y), matching UIElements.getIconImage(x, y).
		/// </summary>
		public static CroppedBitmap GetIcon(int x, int y)
		{
			var sheet = GetSheet();
			int cellWidth = sheet.PixelWidth / Columns;
			int cellHeight = sheet.PixelHeight / Rows;

			var rect = new Int32Rect((x - 1) * cellWidth, (y - 1) * cellHeight, cellWidth, cellHeight);
			return new CroppedBitmap(sheet, rect);
		}

		/// <summary>
		/// Same icon, converted to grayscale — used for "not yet achieved" indicators.
		/// </summary>
		public static FormatConvertedBitmap GetIconGrayscale(int x, int y)
		{
			var color = GetIcon(x, y);
			return new FormatConvertedBitmap(color, System.Windows.Media.PixelFormats.Gray8, null, 0);
		}

		private static BitmapImage GetSheet()
		{
			if (_sheet == null)
			{
				var path = ResolvePath("icons.png");
				_sheet = new BitmapImage(new Uri(path, UriKind.Absolute));
			}

			return _sheet;
		}

		private static string ResolvePath(string fileName)
		{
			return BotBasesResourceLocator.Resolve("Resources", fileName)
				?? throw new FileNotFoundException($"Resource file not found: {fileName}");
		}
	}
}
