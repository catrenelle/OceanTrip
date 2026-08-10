using System;
using System.Globalization;
using System.Windows.Data;

namespace Ocean_Trip.UI.Wpf.Converters
{
	/// <summary>
	/// Binds a RadioButton's IsChecked to one value of an enum- or int-typed property.
	/// ConverterParameter is the value (as a string) this specific RadioButton represents —
	/// e.g. ConverterParameter="Points" for a FishPriority enum property, or ConverterParameter="3"
	/// for an int-typed property like IndigoAchievementFocus storing an AchievementType value.
	/// </summary>
	public class EnumToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return false;

			return value.ToString() == parameter.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not bool isChecked || !isChecked)
				return Binding.DoNothing;

			if (targetType.IsEnum)
				return Enum.Parse(targetType, parameter.ToString());

			return System.Convert.ChangeType(parameter, targetType, culture);
		}
	}
}
