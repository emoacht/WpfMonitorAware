using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PerMonitorDpi.Views.Converters
{
	[ValueConversion(typeof(object), typeof(Visibility))]
	public class ObjectToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Convert whether object is null to Visibility.
		/// </summary>
		/// <param name="value">Source object</param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>If not null, return Visibility.Visible. If null, return Visibility.Collapsed.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
