using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

using MonitorAware.Models;

namespace MonitorAware
{
	/// <summary>
	/// Behavior to make a <see cref="System.Windows.Window"/> Per-Monitor DPI aware
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
		/// Handler for Window
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