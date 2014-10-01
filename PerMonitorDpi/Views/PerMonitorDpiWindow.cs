using System;
using System.Windows;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Per-Monitor DPI aware window
	/// </summary>
	public class PerMonitorDpiWindow : Window
	{
		public WindowHandler WindowHandler
		{
			get { return _windowhandler ?? (_windowhandler = new WindowHandler()); }
		}
		private WindowHandler _windowhandler;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowHandler.Initialize(this);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowHandler.Close();
		}
	}
}