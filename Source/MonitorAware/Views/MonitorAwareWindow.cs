﻿using System;
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
		/// Handler for Window
		/// </summary>
		public WindowHandler WindowHandler { get; } = new WindowHandler();

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

		#region Property

		/// <summary>
		/// Whether to forbear scaling and leave it to the built-in functionality
		/// </summary>
		public bool ForbearScaling
		{
			get => WindowHandler.ForbearScaling;
			set => WindowHandler.ForbearScaling = value;
		}

		#endregion
	}
}