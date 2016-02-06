using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MonitorAware.Views.Converters
{
	/// <summary>
	/// Converts whether object is null to Visibility.
	/// </summary>
	[ValueConversion(typeof(object), typeof(Visibility))]
	public class ObjectToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts whether object is null to Visibility.
		/// </summary>
		/// <param name="value">Source object</param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>If not null, Visibility.Visible. If null, Visibility.Collapsed.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null) ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}