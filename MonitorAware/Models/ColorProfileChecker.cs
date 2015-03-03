using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using MonitorAware.Models.Win32;

namespace MonitorAware.Models
{
	/// <summary>
	/// Check color profile.
	/// </summary>
	public static class ColorProfileChecker
	{
		/// <summary>
		/// Get color profile file path used by the monior to which a specified Window belongs.
		/// </summary>
		/// <param name="sourceVisual">Source Window</param>
		/// <returns>Color profile file path</returns>
		public static string GetColorProfilePath(Visual sourceVisual)
		{
			var source = PresentationSource.FromVisual(sourceVisual) as HwndSource;
			if (source == null)
				return null;

			var handleMonitor = NativeMethod.MonitorFromWindow(
				source.Handle,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			var monitorInfo = new NativeMethod.MONITORINFOEX
			{
				cbSize = (uint)Marshal.SizeOf(typeof(NativeMethod.MONITORINFOEX))
			};

			if (!NativeMethod.GetMonitorInfo(handleMonitor, ref monitorInfo))
				return null;

			IntPtr deviceContext = IntPtr.Zero;

			try
			{
				deviceContext = NativeMethod.CreateDC(
					monitorInfo.szDevice,
					monitorInfo.szDevice,
					null,
					IntPtr.Zero);

				if (deviceContext == IntPtr.Zero)
					return null;

				// The maximum file path length is 260 which is defined as MAX_PATH. It may be longer in
				// Unicode versions of some functions while no detailed information on GetICMProfileW.
				var lpcbName = 260U;
				while (true)
				{
					var sb = new StringBuilder((int)lpcbName);
					if (NativeMethod.GetICMProfile(deviceContext, ref lpcbName, sb))
						return sb.ToString();
				}
			}
			finally
			{
				if (deviceContext != IntPtr.Zero)
					NativeMethod.DeleteDC(deviceContext);
			}
		}
	}
}