using System;
using System.Windows;
using System.Windows.Media;

namespace MonitorAware.Views.Controls
{
	/// <summary>
	/// Drawing restore icon
	/// </summary>
	public class DrawingRestoreIcon : IDrawingIcon
	{
		/// <summary>
		/// Draw restore icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from default DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that canvas size is 16x16 by default.</remarks>
		public void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var pen = new Pen(foreground, Math.Round(1D * factor) / factor); // 1 is base path thickness.

			var rect1Chrome = new Rect(3, 6, 7, 7);
			var rect1Title = new Rect(3, 6, 7, 2); // 2 is base title height.

			var rect2Chrome = new Rect(6, 3, 7, 7);
			var rect2Title = new Rect(6, 3, 7, 2); // 2 is base title height.

			var rect1ChromeActual = new Rect(
				rect1Chrome.X + pen.Thickness / 2,
				rect1Chrome.Y + pen.Thickness / 2,
				rect1Chrome.Width - pen.Thickness,
				rect1Chrome.Height - pen.Thickness);

			var rect1TitleActual = new Rect(
				rect1Title.X + pen.Thickness / 2,
				rect1Title.Y + pen.Thickness / 2,
				rect1Title.Width - pen.Thickness,
				rect1Title.Height - pen.Thickness);

			var rect2ChromeActual = new Rect(
				rect2Chrome.X + pen.Thickness / 2,
				rect2Chrome.Y + pen.Thickness / 2,
				rect2Chrome.Width - pen.Thickness,
				rect2Chrome.Height - pen.Thickness);

			var rect2TitleActual = new Rect(
				rect2Title.X + pen.Thickness / 2,
				rect2Title.Y + pen.Thickness / 2,
				rect2Title.Width - pen.Thickness,
				rect2Title.Height - pen.Thickness);

			// Create a guidelines set.
			var guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(rect1Chrome.Left);
			guidelines.GuidelinesX.Add(rect1Chrome.Right);
			guidelines.GuidelinesY.Add(rect1Chrome.Top);
			guidelines.GuidelinesY.Add(rect1Chrome.Bottom);

			guidelines.GuidelinesY.Add(rect1Title.Bottom);

			guidelines.GuidelinesX.Add(rect2Chrome.Left);
			guidelines.GuidelinesX.Add(rect2Chrome.Right);
			guidelines.GuidelinesY.Add(rect2Chrome.Top);
			guidelines.GuidelinesY.Add(rect2Chrome.Bottom);

			guidelines.GuidelinesY.Add(rect2Title.Bottom);

			drawingContext.PushGuidelineSet(guidelines);

			// Draw rectangles.
			drawingContext.DrawRectangle(null, pen, rect1ChromeActual);
			drawingContext.DrawRectangle(foreground, pen, rect1TitleActual);

			var combined = new CombinedGeometry(
				new RectangleGeometry(rect2ChromeActual),
				new RectangleGeometry(rect1ChromeActual))
			{
				GeometryCombineMode = GeometryCombineMode.Exclude
			};
			drawingContext.DrawGeometry(null, pen, combined);

			drawingContext.DrawRectangle(foreground, pen, rect2TitleActual);

			drawingContext.Pop();
		}
	}
}