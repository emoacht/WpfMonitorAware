using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfExtendedWindow.Views.Converters
{
	/// <summary>
	/// Converts double to multiplied double.
	/// </summary>
	[ValueConversion(typeof(double), typeof(double))]
	public class DoubleMultiplyConverter : IValueConverter
	{
		/// <summary>
		/// Converts double to multiplied double.
		/// </summary>
		/// <param name="value">Source double</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Multiplier double</param>
		/// <param name="culture"></param>
		/// <returns>Multiplied double</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!double.TryParse(value?.ToString(), out double source))
				return DependencyProperty.UnsetValue;

			if (!double.TryParse(parameter?.ToString(), out double multiplier))
				return DependencyProperty.UnsetValue;

			return source * multiplier;
		}

		/// <summary>
		/// Converts double to divided double.
		/// </summary>
		/// <param name="value">Source double</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Divider double</param>
		/// <param name="culture"></param>
		/// <returns>Divided double</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!double.TryParse(value?.ToString(), out double source))
				return DependencyProperty.UnsetValue;

			if (!double.TryParse(parameter?.ToString(), out double divider))
				return DependencyProperty.UnsetValue;

			return source / divider;
		}
	}
}