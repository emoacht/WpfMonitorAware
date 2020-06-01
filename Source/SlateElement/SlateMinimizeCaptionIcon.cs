using System;
using System.Windows;
using System.Windows.Media;

namespace SlateElement
{
	/// <summary>
	/// Control for minimize caption icon image
	/// </summary>
	public class SlateMinimizeCaptionIcon : SlateCaptionIcon
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SlateMinimizeCaptionIcon()
		{
			this.Width = 10D;
			this.Height = 10D;
		}

		/// <summary>
		/// Draws minimize icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of Control</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that its size is 10x10.</remarks>
		protected override void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var line = new Pen(foreground, Math.Round(1D * factor) / factor); // 1 is base path thickness.
			var lineRadius = line.Thickness / 2;

			var startPoint = new Point(0, 5 - lineRadius);
			var endPoint = new Point(10, 5 - lineRadius);

			// Create a guidelines set.
			var guidelines = new GuidelineSet();
			guidelines.GuidelinesY.Add(startPoint.Y);

			drawingContext.PushGuidelineSet(guidelines);

			// Draw line.
			drawingContext.DrawLine(line, startPoint, endPoint);

			drawingContext.Pop();
		}
	}
}