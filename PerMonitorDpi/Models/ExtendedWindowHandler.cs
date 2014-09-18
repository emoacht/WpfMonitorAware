using System;

using PerMonitorDpi.Models.Win32;

namespace PerMonitorDpi.Models
{
	public class ExtendedWindowHandler : WindowHandler
	{
		#region Event

		/// <summary>
		/// Window activated changed event
		/// </summary> 
		/// <remarks>
		/// Args true: Window is being activated.
		/// Args false: Window is being deactivated.
		/// </remarks>
		internal event EventHandler<bool> WindowActivatedChanged;

		/// <summary>
		/// DWM colorization color changed event
		/// </summary>
		/// <remarks>This event will be fired when default Window chrome color has been changed.</remarks>
		internal event EventHandler DwmColorizationColorChanged;

		#endregion


		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case (int)WindowMessage.WM_ACTIVATE:
					{
						var handler = WindowActivatedChanged;
						if (handler == null)
							break;

						switch (wParam.ToInt32())
						{
							case NativeMethod.WA_ACTIVE:
							case NativeMethod.WA_CLICKACTIVE:
								handler(this, true); // Activated.
								break;

							case NativeMethod.WA_INACTIVE:
								handler(this, false); // Deactivated.
								break;
						}
					}
					break;

				case (int)WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED:
					{
						var handler = DwmColorizationColorChanged;
						if (handler != null)
							handler(this, EventArgs.Empty);
					}
					break;
			}

			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}
	}
}
