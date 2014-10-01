using System;
using System.Windows;
using System.Windows.Interactivity;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Behavior to make a window Per-Monitor DPI aware
	/// </summary>
	public class PerMonitorDpiBehavior : Behavior<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			this.AssociatedObject.Loaded += OnWindowLoaded;
			this.AssociatedObject.Closed += OnWindowClosed;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (this.AssociatedObject == null)
				return;

			this.AssociatedObject.Loaded -= OnWindowLoaded;
			this.AssociatedObject.Closed -= OnWindowClosed;
		}

		public WindowHandler WindowHandler
		{
			get { return _windowhandler ?? (_windowhandler = new WindowHandler()); }
		}
		private WindowHandler _windowhandler;

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			WindowHandler.Initialize(this.AssociatedObject);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			WindowHandler.Close();
		}
	}
}