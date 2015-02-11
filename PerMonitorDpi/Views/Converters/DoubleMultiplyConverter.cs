using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PerMonitorDpi.Views.Converters
{
	/// <summary>
	/// Convert double to multiplied double.
	/// </summary>
	[ValueConversion(typeof(double), typeof(double))]
	public class DoubleMultiplyConverter : IValueConverter
	{
		/// <summary>
		/// Convert double to multiplied double.
		/// </summary>
		/// <param name="value">Source double</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Multiplier double</param>
		/// <param name="culture"></param>
		/// <returns>Multiplied double</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double source;
			if ((value == null) || !double.TryParse(value.ToString(), out source))
				return DependencyProperty.UnsetValue;

			double multiplier;
			if ((parameter == null) || !double.TryParse(parameter.ToString(), out multiplier))
				return DependencyProperty.UnsetValue;

			return source * multiplier;
		}

		/// <summary>
		/// Convert double to divided double.
		/// </summary>
		/// <param name="value">Source double</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Divider double</param>
		/// <param name="culture"></param>
		/// <returns>Divided double</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double source;
			if ((value == null) || !double.TryParse(value.ToString(), out source))
				return DependencyProperty.UnsetValue;

			double divider;
			if ((parameter == null) || !double.TryParse(parameter.ToString(), out divider))
				return DependencyProperty.UnsetValue;

			return source / divider;
		}
	}
}