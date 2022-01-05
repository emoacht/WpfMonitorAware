using System;
using System.Windows;

using MonitorAware.Models;

namespace MonitorAware
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
		protected override Freezable CreateInstanceCore() => new MonitorAwareProperty();

		#endregion

		/// <summary>
		/// Gets MonitorAwareProperty instance.
		/// </summary>
		/// <param name="window">Owner Window</param>
		/// <returns>MonitorAwareProperty</returns>
		public static MonitorAwareProperty GetInstance(Window window)
		{
			return (MonitorAwareProperty)window.GetValue(InstanceProperty);
		}
		/// <summary>
		/// Sets MonitorAwareProperty instance.
		/// </summary>
		/// <param name="window">Owner Window</param>
		/// <param name="instance">MonitorAwareProperty</param>
		public static void SetInstance(Window window, MonitorAwareProperty instance)
		{
			window.SetValue(InstanceProperty, instance);
		}
		/// <summary>
		/// Attached property for Instance
		/// </summary>
		public static readonly DependencyProperty InstanceProperty =
			DependencyProperty.RegisterAttached(
				"Instance",
				typeof(MonitorAwareProperty),
				typeof(MonitorAwareProperty),
				new PropertyMetadata(
					null,
					(d, e) =>
					{
						if (d is not Window window)
							return;

						((MonitorAwareProperty)e.NewValue).OwnerWindow = window;
					}));

		private Window OwnerWindow
		{
			get => _ownerWindow;
			set
			{
				_ownerWindow = value;

				_ownerWindow.Loaded += OnWindowLoaded;
				_ownerWindow.Closed += OnWindowClosed;
			}
		}
		private Window _ownerWindow;

		/// <summary>
		/// Gets the instance of MonitorAwareProperty attached property from a specified Window.
		/// </summary>
		/// <param name="window">Window</param>
		/// <returns>MonitorAwareProperty</returns>
		public static MonitorAwareProperty GetMonitorAwareProperty(Window window)
		{
			return window.GetValue(InstanceProperty) as MonitorAwareProperty;
		}

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