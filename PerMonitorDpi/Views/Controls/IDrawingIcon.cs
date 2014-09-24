using System.Windows.Media;

namespace PerMonitorDpi.Views.Controls
{
	public interface IDrawingIcon
	{
		/// <summary>
		/// Draw icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from default DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		void Draw(DrawingContext drawingContext, double factor, Brush foreground);
	}
}
