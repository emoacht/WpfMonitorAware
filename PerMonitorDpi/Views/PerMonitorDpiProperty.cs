using System;
using System.Windows;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Attached property to make a window Per-Monitor DPI aware
	/// </summary>
	public class PerMonitorDpiProperty : DependencyObject
	{
		public static PerMonitorDpiProperty GetAttachedProperty(DependencyObject obj)
		{
			return (PerMonitorDpiProperty)obj.GetValue(AttachedPropertyProperty);
		}
		public static void SetAttachedProperty(DependencyObject obj, PerMonitorDpiProperty value)
		{
			obj.SetValue(AttachedPropertyProperty, value);
		}
		public static readonly DependencyProperty AttachedPropertyProperty =
			DependencyProperty.RegisterAttached(
				"AttachedProperty",
				typeof(PerMonitorDpiProperty),
				typeof(PerMonitorDpiProperty),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						var window = d as Window;
						if (window == null)
							return;

						((PerMonitorDpiProperty)e.NewValue).ownerWindow = window;
					}));

		private Window ownerWindow
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

		public WindowHandler WindowHandler
		{
			get { return _windowhandler ?? (_windowhandler = new WindowHandler()); }
		}
		private WindowHandler _windowhandler;

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			ownerWindow.Loaded -= OnWindowLoaded;

			WindowHandler.Initialize(ownerWindow);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			ownerWindow.Closed -= OnWindowClosed;

			WindowHandler.Close();
		}
	}
}
