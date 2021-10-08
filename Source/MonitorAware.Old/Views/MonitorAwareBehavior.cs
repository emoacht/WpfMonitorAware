using System;
using System.Windows;
using System.Windows.Interactivity;

using MonitorAware.Models;

namespace MonitorAware.Views
{
	/// <summary>
	/// Behavior to make a <see cref="Window"/> Per-Monitor DPI aware
	/// </summary>
	[TypeConstraint(typeof(Window))]
	public class MonitorAwareBehavior : Behavior<Window>
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
		public WindowHandler WindowHandler { get; } = new WindowHandler();

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			WindowHandler.Initialize(this.AssociatedObject);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			WindowHandler.Close();
		}

		#region Property

		/// <summary>
		/// Whether to forbear scaling if it is unnecessary because built-in scaling is enabled
		/// </summary>
		public bool WillForbearScalingIfUnnecessary
		{
			get { return WindowHandler.WillForbearScalingIfUnnecessary; }
			set { WindowHandler.WillForbearScalingIfUnnecessary = value; }
		}

		#endregion
	}
}