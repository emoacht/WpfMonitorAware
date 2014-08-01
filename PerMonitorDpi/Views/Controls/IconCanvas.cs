using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views.Controls
{
	/// <summary>
	/// Draw icon in canvas.
	/// </summary>
	/// <remarks>Purpose of this canvas is to control icon shape regardless of DPI.</remarks>
	public class IconCanvas : Canvas
	{
		public IconCanvas()
		{
			// Drawing assumes that canvas size is 16x16 by default.
			this.Width = 16D;
			this.Height = 16D;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			SetDrawingFactor();
		}


		#region Property

		/// <summary>
		/// Icon foreground brush
		/// </summary>
		/// <remarks>Black by OS's default.</remarks>
		public Brush Foreground
		{
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public static readonly DependencyProperty ForegroundProperty =
			DependencyProperty.Register(
				"Foreground",
				typeof(Brush),
				typeof(IconCanvas),
				new FrameworkPropertyMetadata(
					Brushes.Black,
					(d, e) => ((IconCanvas)d).InvalidateVisual()));

		/// <summary>
		/// Drawing for icon
		/// </summary>
		public IDrawingIcon IconDrawing
		{
			get { return (IDrawingIcon)GetValue(IconDrawingProperty); }
			set { SetValue(IconDrawingProperty, value); }
		}
		public static readonly DependencyProperty IconDrawingProperty =
			DependencyProperty.Register(
				"IconDrawing",
				typeof(IDrawingIcon),
				typeof(IconCanvas),
				new FrameworkPropertyMetadata(null));

		/// <summary>
		/// LayoutTransform of ancestor FrameworkElement
		/// </summary>
		public Transform AncestorTransform
		{
			get { return (Transform)GetValue(AncestorTransformProperty); }
			set { SetValue(AncestorTransformProperty, value); }
		}
		public static readonly DependencyProperty AncestorTransformProperty =
			DependencyProperty.Register(
				"AncestorTransform",
				typeof(Transform),
				typeof(IconCanvas),
				new FrameworkPropertyMetadata(
					Transform.Identity,
					(d, e) =>
					{
						var transform = e.NewValue as ScaleTransform;
						if (transform != null)
							((IconCanvas)d).SetDrawingFactor(transform);
					}));

		#endregion


		private static readonly Dpi systemDpi = DpiChecker.GetSystemDpi();
		private double drawingFactor = 1D;

		private void SetDrawingFactor(ScaleTransform transform = null)
		{
			var factorX = (double)systemDpi.X / Dpi.Default.X;

			if (transform != null)
				factorX *= transform.ScaleX;

			drawingFactor = factorX;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (IconDrawing != null)
			{
				IconDrawing.Draw(drawingContext, drawingFactor, Foreground);
				return;
			}

			Draw(drawingContext, drawingFactor, Foreground);
		}

		/// <summary>
		/// Draw fall back icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from default DPI</param>
		/// <param name="foreground">Icon foreground brush</param>
		private void Draw(DrawingContext drawingContext, double factor, Brush foreground)
		{
			var pen = new Pen(foreground, Math.Round(1D * factor) / factor); // 1 is base path thickness.

			var rectChrome = new Rect(3, 4, 10, 8);

			var rectChromeActual = new Rect(
				rectChrome.X + pen.Thickness / 2,
				rectChrome.Y + pen.Thickness / 2,
				rectChrome.Width - pen.Thickness,
				rectChrome.Height - pen.Thickness);

			// Create a guidelines set.
			var guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(rectChrome.Left);
			guidelines.GuidelinesX.Add(rectChrome.Right);
			guidelines.GuidelinesY.Add(rectChrome.Top);
			guidelines.GuidelinesY.Add(rectChrome.Bottom);

			drawingContext.PushGuidelineSet(guidelines);

			// Draw rectangles.
			drawingContext.DrawRectangle(null, pen, rectChromeActual);

			drawingContext.Pop();
		}
	}
}
