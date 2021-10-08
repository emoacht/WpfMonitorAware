using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MonitorAware.Models;

namespace WpfExtendedWindow.Views.Controls
{
	/// <summary>
	/// Canvas for drawing icon
	/// </summary>
	/// <remarks>Purpose of this canvas is to control icon shape regardless of DPI.</remarks>
	public class IconCanvas : Canvas
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public IconCanvas()
		{
			// Drawing assumes that canvas size is 16x16 by default.
			this.Width = 16D;
			this.Height = 16D;
		}

		/// <summary>
		/// OnInitialized
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			SetDrawingFactor();
		}

		#region Property

		/// <summary>
		/// Icon foreground Brush
		/// </summary>
		/// <remarks>Black by OS's default.</remarks>
		public Brush Foreground
		{
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="Foreground"/>
		/// </summary>
		public static readonly DependencyProperty ForegroundProperty =
			DependencyProperty.Register(
				"Foreground",
				typeof(Brush),
				typeof(IconCanvas),
				new PropertyMetadata(
					Brushes.Black,
					(d, e) => ((IconCanvas)d).InvalidateVisual()));

		/// <summary>
		/// Drawing icon
		/// </summary>
		public IDrawingIcon DrawingIcon
		{
			get { return (IDrawingIcon)GetValue(DrawingIconProperty); }
			set { SetValue(DrawingIconProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="DrawingIcon"/>
		/// </summary>
		public static readonly DependencyProperty DrawingIconProperty =
			DependencyProperty.Register(
				"DrawingIcon",
				typeof(IDrawingIcon),
				typeof(IconCanvas),
				new PropertyMetadata(default(IDrawingIcon)));

		/// <summary>
		/// LayoutTransform of ancestor FrameworkElement
		/// </summary>
		public Transform AncestorTransform
		{
			get { return (Transform)GetValue(AncestorTransformProperty); }
			set { SetValue(AncestorTransformProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="AncestorTransform"/>
		/// </summary>
		public static readonly DependencyProperty AncestorTransformProperty =
			DependencyProperty.Register(
				"AncestorTransform",
				typeof(Transform),
				typeof(IconCanvas),
				new PropertyMetadata(
					Transform.Identity,
					(d, e) =>
					{
						var transform = e.NewValue as ScaleTransform;
						if (transform != null)
							((IconCanvas)d).SetDrawingFactor(transform);
					}));

		#endregion

		private double _drawingFactor = 1D;

		private void SetDrawingFactor(ScaleTransform transform = null)
		{
			var factorX = DpiHelper.SystemDpi.DpiScaleX;

			if (transform != null)
				factorX *= transform.ScaleX;

			_drawingFactor = factorX;
		}

		/// <summary>
		/// OnRender
		/// </summary>
		/// <param name="drawingContext"></param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			if (DrawingIcon != null)
			{
				DrawingIcon.Draw(drawingContext, _drawingFactor, Foreground);
				return;
			}

			Draw(drawingContext, _drawingFactor, Foreground);
		}

		/// <summary>
		/// Draws fallback icon.
		/// </summary>
		/// <param name="drawingContext">DrawingContext of canvas</param>
		/// <param name="factor">Factor from identity DPI</param>
		/// <param name="foreground">Icon foreground Brush</param>
		/// <remarks>This drawing assumes that canvas size is 16x16 by default.</remarks>
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