using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PerMonitorDpi.Views.Converters
{
	public class DoubleSumConverter : IMultiValueConverter
	{
		/// <summary>
		/// Sum doubles.
		/// </summary>
		/// <param name="values">Source doubles</param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>Sum of doubles</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values.OfType<double>().Sum();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}