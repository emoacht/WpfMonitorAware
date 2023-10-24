using System;
using System.Windows;
using System.Windows.Media;

namespace SlateElement
{
	/// <summary>
	/// Control for restore caption icon image
	/// </summary>
	public class SlateRestoreCaptionIcon : SlateCaptionIcon
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SlateRestoreCaptionIcon()
		{
			this.Width = 10D;
			this.Height = 10D;
		}

		/// <summary>
		/// Draws restore icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of Control</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that its size is 10x10.</remarks>
		protected override void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var line = new Pen(foreground, Math.Round(1D * factor, MidpointRounding.AwayFromZero) / factor); // 1 is base path thickness.
			var lineRadius = line.Thickness / 2;

			var rect1Chrome = new Rect(0, 2, 8, 8);

			var rect2Chrome = new Rect(2, 0, 8, 8);

			var rect1ChromeActual = new Rect(
				rect1Chrome.X + lineRadius,
				rect1Chrome.Y + lineRadius,
				rect1Chrome.Width - lineRadius * 2,
				rect1Chrome.Height - lineRadius * 2);

			var rect2ChromeActual = new Rect(
				rect2Chrome.X + lineRadius,
				rect2Chrome.Y + lineRadius,
				rect2Chrome.Width - lineRadius * 2,
				rect2Chrome.Height - lineRadius * 2);

			// Set guidelines.
			var guidelines1 = new GuidelineSet();
			guidelines1.GuidelinesX.Add(rect1Chrome.Left);
			guidelines1.GuidelinesX.Add(rect1Chrome.Right);
			guidelines1.GuidelinesY.Add(rect1Chrome.Top);
			guidelines1.GuidelinesY.Add(rect1Chrome.Bottom);

			var guidelines2 = new GuidelineSet();
			guidelines2.GuidelinesX.Add(rect2Chrome.Left);
			guidelines2.GuidelinesX.Add(rect2Chrome.Right);
			guidelines2.GuidelinesY.Add(rect2Chrome.Top);
			guidelines2.GuidelinesY.Add(rect2Chrome.Bottom);

			drawingContext.PushGuidelineSet(guidelines1);
			drawingContext.PushGuidelineSet(guidelines2);

			// Draw rectangles.
			drawingContext.DrawRectangle(null, line, rect1ChromeActual);

			var combinedActual = new CombinedGeometry(
				GeometryCombineMode.Exclude,
				new RectangleGeometry(rect2ChromeActual),
				new RectangleGeometry(rect1ChromeActual));

			drawingContext.DrawGeometry(null, line, combinedActual);

			drawingContext.Pop();
		}
	}
}