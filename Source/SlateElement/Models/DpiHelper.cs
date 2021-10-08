using System;
using System.Windows;
using System.Windows.Media;

namespace SlateElement.Models
{
	/// <summary>
	/// Utility methods for <see cref="System.Windows.DpiScale"/>
	/// </summary>
	public static class DpiHelper
	{
		/// <summary>
		/// Gets system DPI.
		/// </summary>
		/// <param name="visual">Source Visual</param>
		/// <returns>DPI scale information</returns>
		public static DpiScale GetSystemDpi(Visual visual)
		{
			if (visual is null)
				throw new ArgumentNullException(nameof(visual));

			var source = PresentationSource.FromVisual(visual);
			if (source?.CompositionTarget is null)
				return new DpiScale(1D, 1D); // Fallback

			return new DpiScale(
				source.CompositionTarget.TransformToDevice.M11,
				source.CompositionTarget.TransformToDevice.M22);
		}
	}
}