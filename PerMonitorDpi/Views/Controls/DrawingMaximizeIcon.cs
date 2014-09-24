using System;
using System.Windows;
using System.Windows.Media;

namespace PerMonitorDpi.Views.Controls
{
	public class DrawingMaximizeIcon : IDrawingIcon
	{
		/// <summary>
		/// Draw maximize icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from default DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that canvas size is 16x16 by default.</remarks>
		public void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var pen = new Pen(foreground, Math.Round(1D * factor) / factor); // 1 is base path thickness.

			var rectChrome = new Rect(3, 4, 10, 8);
			var rectTitle = new Rect(3, 4, 10, 2); // 2 is base title height.

			var rectChromeActual = new Rect(
				rectChrome.X + pen.Thickness / 2,
				rectChrome.Y + pen.Thickness / 2,
				rectChrome.Width - pen.Thickness,
				rectChrome.Height - pen.Thickness);

			var rectTitleActual = new Rect(
				rectTitle.X + pen.Thickness / 2,
				rectTitle.Y + pen.Thickness / 2,
				rectTitle.Width - pen.Thickness,
				rectTitle.Height - pen.Thickness);

			// Create a guidelines set.
			var guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(rectChrome.Left);
			guidelines.GuidelinesX.Add(rectChrome.Right);
			guidelines.GuidelinesY.Add(rectChrome.Top);
			guidelines.GuidelinesY.Add(rectChrome.Bottom);

			guidelines.GuidelinesY.Add(rectTitle.Bottom);

			drawingContext.PushGuidelineSet(guidelines);

			// Draw rectangles.
			drawingContext.DrawRectangle(null, pen, rectChromeActual);
			drawingContext.DrawRectangle(foreground, pen, rectTitleActual);

			drawingContext.Pop();
		}
	}
}