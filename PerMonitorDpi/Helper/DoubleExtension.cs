using System;

namespace PerMonitorDpi.Helper
{
	public static class DoubleExtension
	{
		/// <summary>
		/// Get a preliminarily rounded double to prevent a FrameworkElement from getting blurred by ScaleTransform.
		/// </summary>
		/// <param name="source">Source double</param>
		/// <param name="factor">Factor of ScaleTransform</param>
		/// <returns>Rounded double</returns>
		public static double ToRounded(this double source, double factor)
		{
			if (factor <= 0)
				throw new ArgumentException("factor");

			return Math.Round(source * factor) / factor;
		}
	}
}