using System.Windows.Media;

namespace WpfExtendedWindow.Helper
{
	/// <summary>
	/// Extension method for <see cref="System.Windows.Media.Brush"/>
	/// </summary>
	internal static class BrushExtension
	{
		/// <summary>
		/// Determines whether a Brush is Transparent.
		/// </summary>
		/// <param name="source">Source Brush</param>
		/// <returns>True if Transparent</returns>
		public static bool IsTransparent(this Brush source)
		{
			return (source is SolidColorBrush solid)
				&& (solid?.Color == Colors.Transparent);
		}
	}
}