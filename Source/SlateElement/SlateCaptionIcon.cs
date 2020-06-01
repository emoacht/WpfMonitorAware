using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SlateElement
{
	/// <summary>
	/// Control for caption icon image
	/// </summary>
	/// <remarks>This Control is for maintaining caption icon shape regardless of DPI.</remarks>
	public abstract class SlateCaptionIcon : Control
	{
		/// <summary>
		/// OnInitialized
		/// </summary>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			SetDrawingFactor();
		}

		/// <summary>
		/// OnDpiChanged
		/// </summary>
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);

			SetDrawingFactor(newDpi);
		}

		private double _drawingFactor = 1D;

		private void SetDrawingFactor(DpiScale dpi = default)
		{
			if (dpi.Equals(default(DpiScale)))
				dpi = VisualTreeHelper.GetDpi(this);

			_drawingFactor = dpi.DpiScaleX;
		}

		/// <summary>
		/// OnRender
		/// </summary>
		protected override void OnRender(DrawingContext drawingContext)
		{
			Draw(drawingContext, _drawingFactor, Foreground);

			base.OnRender(drawingContext);
		}

		/// <summary>
		/// Draws icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of Control</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		protected abstract void Draw(DrawingContext drawingContext, double factor, Brush foreground);
	}
}