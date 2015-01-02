using System.Windows.Media;

namespace PerMonitorDpi.Helper
{
	internal static class BrushExtenstion
	{
		/// <summary>
		/// Check if a Brush is Transparent.
		/// </summary>
		/// <param name="source">Source Brush</param>
		/// <returns>True if Transparent</returns>
		public static bool IsTransparent(this Brush source)
		{
			var solid = source as SolidColorBrush;
			if (solid == null)
				return false;

			return (solid.Color == Colors.Transparent);
		}
	}
}