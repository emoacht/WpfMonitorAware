using System.Windows.Media;

namespace MonitorAware.Views.Controls
{
	/// <summary>
	/// Interface for drawing icon
	/// </summary>
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