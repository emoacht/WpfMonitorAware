using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using MonitorAware.Extended.Helper;

namespace MonitorAware.Extended.Views.Controls
{
	/// <summary>
	/// Caption button for <see cref="MonitorAware.Extended.Views.ExtendedWindow"/>
	/// </summary>
	public class ExtendedCaptionButton : Button
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExtendedCaptionButton()
		{
		}

		/// <summary>
		/// OnInitialized
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var window = Window.GetWindow(this) as ExtendedWindow;
			if (window == null)
				return;

			this.SetBinding(
				IsAboutActiveProperty,
				new Binding(nameof(ExtendedWindow.IsAboutActive))
				{
					Source = window,
					Mode = BindingMode.OneWay,
				});

			if (NormalBackground.IsTransparent())
			{
				this.SetBinding(
					NormalBackgroundProperty,
					new Binding(nameof(ExtendedWindow.CaptionButtonBackground))
					{
						Source = window,
						Mode = BindingMode.OneWay,
					});
			}

			if (DeactivatedBackground.IsTransparent())
			{
				this.SetBinding(
					DeactivatedBackgroundProperty,
					new Binding(nameof(ExtendedWindow.ChromeDeactivatedBackground))
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
		/// <remarks>
		/// <para>This property will be changed when the Window is about to be activated or deactivated.</para>
		/// <para>For binding only between code behind.</para>
		/// </remarks>
		internal bool IsAboutActive
		{
			get { return (bool)GetValue(IsAboutActiveProperty); }
			set { SetValue(IsAboutActiveProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="IsAboutActive"/>
		/// </summary>
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
		/// Caption button background Brush when the owner Window is activated
		/// </summary>
		/// <remarks>Default value (Transparent) is to make whole button area hit test visible.</remarks>
		public Brush NormalBackground
		{
			get { return (Brush)GetValue(NormalBackgroundProperty); }
			set { SetValue(NormalBackgroundProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="NormalBackground"/>
		/// </summary>
		public static readonly DependencyProperty NormalBackgroundProperty =
			DependencyProperty.Register(
				"NormalBackground",
				typeof(Brush),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(Brushes.Transparent));

		/// <summary>
		/// Caption button background Brush when the owner Window is deactivated
		/// </summary>
		/// <remarks>Default value (Transparent) is to make whole button area hit test visible.</remarks>
		public Brush DeactivatedBackground
		{
			get { return (Brush)GetValue(DeactivatedBackgroundProperty); }
			set { SetValue(DeactivatedBackgroundProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="DeactivatedBackground"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="IconGeometry"/>
		/// </summary>
		public static readonly DependencyProperty IconGeometryProperty =
			DependencyProperty.Register(
				"IconGeometry",
				typeof(Geometry),
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Drawing icon to be used by IconCanvas
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
				typeof(ExtendedCaptionButton),
				new FrameworkPropertyMetadata(null));

		#endregion
	}
}