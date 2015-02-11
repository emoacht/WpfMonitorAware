using System;
using System.Windows;
using System.Windows.Interactivity;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Behavior to make a <see cref="Window"/> Per-Monitor DPI aware
	/// </summary>
	[TypeConstraint(typeof(Window))]
	public class PerMonitorDpiBehavior : Behavior<Window>
	{
		/// <summary>
		/// OnAttached
		/// </summary>
		protected override void OnAttached()
		{
			base.OnAttached();

			this.AssociatedObject.Loaded += OnWindowLoaded;
			this.AssociatedObject.Closed += OnWindowClosed;
		}

		/// <summary>
		/// OnDetaching
		/// </summary>
		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (this.AssociatedObject == null)
				return;

			this.AssociatedObject.Loaded -= OnWindowLoaded;
			this.AssociatedObject.Closed -= OnWindowClosed;
		}

		/// <summary>
		/// Handler for <see cref="Window"/>
		/// </summary>
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