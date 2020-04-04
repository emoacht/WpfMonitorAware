using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MonitorAware.Views.Converters
{
	/// <summary>
	/// Converts doubles to summed double.
	/// </summary>
	public class DoubleSumConverter : IMultiValueConverter
	{
		/// <summary>
		/// Converts doubles to summed double.
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

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetTypes"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}