using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ocean_Trip.UI.Wpf.Converters
{
	/// <summary>
	/// [item count, BaitRestockThreshold] -> WarningBrush once low-stock, TextSecondaryBrush
	/// otherwise. Lets the Tackle Box row double as an at-a-glance "what needs attention" view
	/// instead of a flat inventory list.
	///
	/// Default mode compares against the threshold (a threshold of 0 means restocking is disabled —
	/// see OceanTripSettings.ValidateSettings — so nothing is ever flagged then). ConverterParameter
	/// "ZeroOnly" switches to flagging only at exactly 0, for lures (e.g. Heavy Steel Jig) that aren't
	/// consumed per-catch like regular bait and only ever need to be at least 1 — the bait-oriented
	/// restock threshold doesn't apply to them.
	/// </summary>
	public class BaitStockToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2 || !int.TryParse(values[0]?.ToString(), out int count))
				return Application.Current.Resources["TextSecondaryBrush"];

			bool lowStock;
			if (string.Equals(parameter as string, "ZeroOnly", StringComparison.OrdinalIgnoreCase))
				lowStock = count <= 0;
			else
				lowStock = int.TryParse(values[1]?.ToString(), out int threshold) && threshold > 0 && count <= threshold;

			return Application.Current.Resources[lowStock ? "WarningBrush" : "TextSecondaryBrush"];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
