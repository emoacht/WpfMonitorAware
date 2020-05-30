using System;
using System.Windows;

using MonitorAware.Models;

using WpfExtendedWindow.Models.Win32;

namespace WpfExtendedWindow.Models
{
	/// <summary>
	/// Handler for <see cref="WpfExtendedWindow.Views.ExtendedWindow"/>
	/// </summary>
	public class ExtendedWindowHandler : WindowHandler
	{
		#region Event

		/// <summary>
		/// Occurs when DWM colorization color is changed.
		/// </summary>
		/// <remarks>
		/// This event is fired when default Window chrome color is changed.
		/// </remarks>
		internal event EventHandler DwmColorizationColorChanged;

		#endregion

		internal new void Initialize(Window window, FrameworkElement element = null)
		{
			base.Initialize(window, element);
		}

		/// <summary>
		/// Handles window messages.
		/// </summary>
		/// <param name="hwnd">The window handle</param>
		/// <param name="msg">The message ID</param>
		/// <param name="wParam">The message's wParam value</param>
		/// <param name="lParam">The message's lParam value</param>
		/// <param name="handled">Whether the message was handled</param>
		/// <returns>Return value depending on the particular message</returns>
		/// <remarks>This is an implementation of System.Windows.Interop.HwndSourceHook.</remarks>
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED:
					DwmColorizationColorChanged?.Invoke(this, EventArgs.Empty);
					break;
			}

			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		internal new void Close()
		{
			base.Close();
		}
	}
}