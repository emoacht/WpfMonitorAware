using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using PerMonitorDpi.Helper;

namespace PerMonitorDpi.Views.Controls
{
	public class ExtendedCaptionButton : Button
	{
		public ExtendedCaptionButton()
		{
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var window = Window.GetWindow(this) as ExtendedWindow;
			if (window == null)
				return;

			this.SetBinding(
				IsAboutActiveProperty,
				new Binding("IsAboutActive")
				{
					Source = window,
					Mode = BindingMode.OneWay,
				});

			if (NormalBackground.IsTransparent())
			{
				this.SetBinding(
					NormalBackgroundProperty,
					new Binding("CaptionButtonBackground")
					{
						Source = window,
						Mode = BindingMode.OneWay,
					});
			}

			if (DeactivatedBackground.IsTransparent())
			{
				this.SetBinding(
					DeactivatedBackgroundProperty,
					new Binding("ChromeDeactivatedBackground")
					{
						Source = window,
						Mode = BindingMode.OneWay,
					});
			}
		}


		#region Property

		/// <summary>
		/// Whether the owner Window is about to be activated (internal)
		/// </summary>
		/// <remarks>This property will be changed when the Window is about to be activated or deactivated.
		/// For binding only between code behinds.</remarks>
		internal bool IsAboutActive
		{
			get { return (bool)GetValue(IsAboutActiveProperty); }
			set { SetValue(IsAboutActiveProperty, value); }
		}
		internal static readonly DependencyProperty IsAboutActiveProperty =
			DependencyProperty.Register(
				"IsAboutActive",
				typeof(bool),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(
					false,
					(d, e) =>
					{
						var button = (ExtendedCaptionButton)d;
						button.Background = (bool)e.NewValue
							? button.NormalBackground.Clone() // Clone method is not to make reference.
							: button.DeactivatedBackground.Clone(); // Clone method is not to make reference.
					}));

		/// <summary>
		/// Caption button background brush when the owner Window is activated
		/// </summary>
		/// <remarks>Default value (Transparent) is to make whole button area hit test visible.</remarks>
		public Brush NormalBackground
		{
			get { return (Brush)GetValue(NormalBackgroundProperty); }
			set { SetValue(NormalBackgroundProperty, value); }
		}
		public static readonly DependencyProperty NormalBackgroundProperty =
			DependencyProperty.Register(
				"NormalBackground",
				typeof(Brush),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(Brushes.Transparent));

		/// <summary>
		/// Caption button background brush when the owner Window is deactivated
		/// </summary>
		/// <remarks>Default value (Transparent) is to make whole button area hit test visible.</remarks>
		public Brush DeactivatedBackground
		{
			get { return (Brush)GetValue(DeactivatedBackgroundProperty); }
			set { SetValue(DeactivatedBackgroundProperty, value); }
		}
		public static readonly DependencyProperty DeactivatedBackgroundProperty =
			DependencyProperty.Register(
				"DeactivatedBackground",
				typeof(Brush),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(Brushes.Transparent));

		/// <summary>
		/// Geometry for button icon
		/// </summary>
		public Geometry IconGeometry
		{
			get { return (Geometry)GetValue(IconGeometryProperty); }
			set { SetValue(IconGeometryProperty, value); }
		}
		public static readonly DependencyProperty IconGeometryProperty =
			DependencyProperty.Register(
				"IconGeometry",
				typeof(Geometry),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Drawing for button icon to be used by IconCanvas
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
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(null));

		#endregion
	}
}
