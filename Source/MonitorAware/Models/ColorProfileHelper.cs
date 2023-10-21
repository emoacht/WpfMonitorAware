using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using static MonitorAware.Models.Win32.NativeMethod;

namespace MonitorAware.Models
{
	/// <summary>
	/// Utility methods for color profile.
	/// </summary>
	public static class ColorProfileHelper
	{
		/// <summary>
		/// Attempts to get color profile file path used by the monitor to which a specified Window belongs.
		/// </summary>
		/// <param name="window">Source Window</param>
		/// <param name="profilePath">Color profile file path</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetColorProfilePath(Visual window, out string profilePath)
		{
			profilePath = GetColorProfilePath(window);
			return (profilePath is not null);
		}

		/// <summary>
		/// Gets color profile file path used by the monitor to which a specified Window belongs.
		/// </summary>
		/// <param name="window">Source Window</param>
		/// <returns>Color profile file path</returns>
		public static string GetColorProfilePath(Visual window)
		{
			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source is null)
				return null;

			var monitorHandle = MonitorFromWindow(
				source.Handle,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			var monitorInfo = new MONITORINFOEX { cbSize = (uint)Marshal.SizeOf<MONITORINFOEX>() };
			if (!GetMonitorInfo(monitorHandle, ref monitorInfo))
				return null;

			var deviceHandle = IntPtr.Zero;
			try
			{
				deviceHandle = CreateDC(
					monitorInfo.szDevice,
					monitorInfo.szDevice,
					null,
					IntPtr.Zero);
				if (deviceHandle == IntPtr.Zero)
					return null;

				// The maximum file path length is 260 which is defined as MAX_PATH. It may be longer in Unicode
				// versions of some functions while no detailed information on GetICMProfileW.
				var lpcbName = 260U;
				while (true)
				{
					var buffer = new StringBuilder((int)lpcbName);
					if (GetICMProfile(deviceHandle, ref lpcbName, buffer))
						return buffer.ToString();
				}
			}
			finally
			{
				if (deviceHandle != IntPtr.Zero)
					DeleteDC(deviceHandle);
			}
		}
	}
}