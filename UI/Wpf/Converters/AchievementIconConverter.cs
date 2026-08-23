using System;
using System.Globalization;
using System.Windows.Data;

namespace Ocean_Trip.UI.Wpf.Converters
{
	/// <summary>
	/// Bool (achieved/not) + "x,y" atlas coordinates (as ConverterParameter) -> color icon if
	/// achieved, grayscale otherwise. Used for the live achievement-progress indicators, which
	/// stay bound so the icon updates in place as FFXIV_Databinds' flags change during play.
	/// </summary>
	public class AchievementIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool achieved = value is bool b && b;

			var parts = parameter?.ToString().Split(',');
			if (parts == null || parts.Length != 2)
				return null;
			if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
				return null;

			return achieved ? (object)IconAtlas.GetIcon(x, y) : IconAtlas.GetIconGrayscale(x, y);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
