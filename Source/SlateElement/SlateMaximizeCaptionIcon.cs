using System;
using System.Windows;
using System.Windows.Media;

namespace SlateElement
{
	/// <summary>
	/// Control for maximize caption icon image
	/// </summary>
	public class SlateMaximizeCaptionIcon : SlateCaptionIcon
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SlateMaximizeCaptionIcon()
		{
			this.Width = 10D;
			this.Height = 10D;
		}

		/// <summary>
		/// Draws maximize icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of Control</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that its size is 10x10.</remarks>
		protected override void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var line = new Pen(foreground, Math.Round(1D * factor) / factor); // 1 is base path thickness.
			var lineRadius = line.Thickness / 2;

			var rectChrome = new Rect(0, 0, 10, 10);

			var rectChromeActual = new Rect(
				rectChrome.X + lineRadius,
				rectChrome.Y + lineRadius,
				rectChrome.Width - lineRadius,
				rectChrome.Height - lineRadius);

			// Create a guidelines set.
			var guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(rectChrome.Left);
			guidelines.GuidelinesX.Add(rectChrome.Right);
			guidelines.GuidelinesY.Add(rectChrome.Top);
			guidelines.GuidelinesY.Add(rectChrome.Bottom);

			drawingContext.PushGuidelineSet(guidelines);

			// Draw rectangles.
			drawingContext.DrawRectangle(null, line, rectChromeActual);

			drawingContext.Pop();
		}
	}
}