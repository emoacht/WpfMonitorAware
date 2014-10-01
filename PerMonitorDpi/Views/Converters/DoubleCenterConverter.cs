using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using PerMonitorDpi.Helper;

namespace PerMonitorDpi.Views.Converters
{
	public class DoubleCenterConverter : IMultiValueConverter
	{
		private enum Order
		{
			Round,
			Floor,
			Ceiling,
		}

		/// <summary>
		/// Get top margin which locates inner FrameworkElement at the center of outer FrameworkElement.
		/// </summary>
		/// <param name="values">Lengths of outer and inner FrameworkElements</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Selection of Math.Round, Math.Floor, Math.Ceiling</param>
		/// <param name="culture"></param>
		/// <returns>Top margin</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var lengths = values.OfType<double>().ToArray();
			if (lengths.Length != 2)
				return DependencyProperty.UnsetValue;

			var buff = Math.Abs(lengths[0] - lengths[1]) / 2;

			var order = ((parameter != null) && EnumAddition.IsDefined(typeof(Order), parameter.ToString(), StringComparison.OrdinalIgnoreCase))
				? (Order)EnumAddition.Parse(typeof(Order), parameter.ToString(), StringComparison.OrdinalIgnoreCase)
				: Order.Round;

			switch (order)
			{
				case Order.Floor:
					return Math.Floor(buff);
				case Order.Ceiling:
					return Math.Ceiling(buff);
				default:
					return Math.Round(buff);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}