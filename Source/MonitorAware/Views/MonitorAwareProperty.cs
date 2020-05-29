using System;
using System.Windows;

using MonitorAware.Models;

namespace MonitorAware.Views
{
	/// <summary>
	/// Attached property to make a <see cref="System.Windows.Window"/> Per-Monitor DPI aware
	/// </summary>
	public class MonitorAwareProperty : Freezable
	{
		#region Freezable member

		/// <summary>
		/// Implements <see cref="Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
		/// </summary>
		/// <returns>New Freezable</returns>
		protected override Freezable CreateInstanceCore()
		{
			return new MonitorAwareProperty();
		}

		#endregion

		/// <summary>
		/// Gets AttachedProperty.
		/// </summary>
		/// <param name="window">Owner Window</param>
		/// <returns>AttachedProperty</returns>
		public static MonitorAwareProperty GetAttachedProperty(Window window)
		{
			return (MonitorAwareProperty)window.GetValue(AttachedPropertyProperty);
		}
		/// <summary>
		/// Sets AttachedProperty.
		/// </summary>
		/// <param name="window">Owner Window</param>
		/// <param name="attachedProperty">AttachedProperty</param>
		public static void SetAttachedProperty(Window window, MonitorAwareProperty attachedProperty)
		{
			window.SetValue(AttachedPropertyProperty, attachedProperty);
		}
		/// <summary>
		/// Attached property for AttachedProperty
		/// </summary>
		public static readonly DependencyProperty AttachedPropertyProperty =
			DependencyProperty.RegisterAttached(
				"AttachedProperty",
				typeof(MonitorAwareProperty),
				typeof(MonitorAwareProperty),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						var window = d as Window;
						if (window == null)
							return;

						((MonitorAwareProperty)e.NewValue).OwnerWindow = window;
					}));

		private Window OwnerWindow
		{
			get { return _ownerWindow; }
			set
			{
				_ownerWindow = value;

				_ownerWindow.Loaded += OnWindowLoaded;
				_ownerWindow.Closed += OnWindowClosed;
			}
		}
		private Window _ownerWindow;

		/// <summary>
		/// Handler for Window
		/// </summary>
		public WindowHandler WindowHandler { get; } = new WindowHandler();

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			OwnerWindow.Loaded -= OnWindowLoaded;

			WindowHandler.Initialize(OwnerWindow);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			OwnerWindow.Closed -= OnWindowClosed;

			WindowHandler.Close();
		}

		#region Property

		/// <summary>
		/// Scaling mode
		/// </summary>
		public ScaleMode ScaleMode
		{
			get => WindowHandler.ScaleMode;
			set => WindowHandler.ScaleMode = value;
		}

		#endregion
	}
}