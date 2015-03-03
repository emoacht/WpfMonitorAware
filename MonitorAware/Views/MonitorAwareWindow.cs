using System;
using System.Windows;

using MonitorAware.Models;

namespace MonitorAware.Views
{
	/// <summary>
	/// Per-Monitor DPI aware window
	/// </summary>
	public class MonitorAwareWindow : Window
	{
		/// <summary>
		/// Handler for <see cref="Window"/>
		/// </summary>
		public WindowHandler WindowHandler
		{
			get { return _windowhandler ?? (_windowhandler = new WindowHandler()); }
		}
		private WindowHandler _windowhandler;

		/// <summary>
		/// OnSourceInitialized
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowHandler.Initialize(this);
		}

		/// <summary>
		/// OnClosed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowHandler.Close();
		}
	}
}