using System.Windows.Media;

namespace WpfExtendedWindow.Views.Controls
{
	/// <summary>
	/// Interface for drawing icon
	/// </summary>
	public interface IDrawingIcon
	{
		/// <summary>
		/// Draws icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		void Draw(DrawingContext drawingContext, double factor, Brush foreground);
	}
}