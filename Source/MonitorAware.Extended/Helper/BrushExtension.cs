using System.Windows.Media;

namespace MonitorAware.Extended.Helper
{
	/// <summary>
	/// Extension method for <see cref="System.Windows.Media.Brush"/>
	/// </summary>
	internal static class BrushExtension
	{
		/// <summary>
		/// Checks if a Brush is Transparent.
		/// </summary>
		/// <param name="source">Source Brush</param>
		/// <returns>True if Transparent</returns>
		public static bool IsTransparent(this Brush source)
		{
			var solid = source as SolidColorBrush;
			return (solid?.Color == Colors.Transparent);
		}
	}
}