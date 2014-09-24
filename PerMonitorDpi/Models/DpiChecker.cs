using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using PerMonitorDpi.Helper;
using PerMonitorDpi.Models.Win32;

namespace PerMonitorDpi.Models
{
	public static class DpiChecker
	{
		#region DPI Awareness

		/// <summary>
		/// Check if current process is Per-Monitor DPI aware.
		/// </summary>
		/// <returns>True if Per-Monitor DPI aware</returns>
		public static bool IsPerMonitorDpiAware()
		{
			var awareness = GetDpiAwareness();
			if (!awareness.HasValue)
				return false;

			return (awareness.Value == NativeMethod.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
		}

		/// <summary>
		/// Get DPI awareness of current process.
		/// </summary>
		/// <returns>If succeeded, Nullable PROCESS_DPI_AWARENESS. If failed, null.</returns>
		public static NativeMethod.PROCESS_DPI_AWARENESS? GetDpiAwareness()
		{
			if (!OsVersion.IsEightOneOrNewer)
				return null;

			NativeMethod.PROCESS_DPI_AWARENESS value;
			var result = NativeMethod.GetProcessDpiAwareness(
				IntPtr.Zero, // Current process
				out value);
			if (result != 0) // 0 means S_OK.
				return null;

			return value;
		}

		#endregion


		#region System DPI

		/// <summary>
		/// Get system DPI.
		/// </summary>
		/// <param name="sourceVisual">Source Visual</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetSystemDpi(Visual sourceVisual)
		{
			var source = PresentationSource.FromVisual(sourceVisual);
			if ((source == null) || (source.CompositionTarget == null))
				return Dpi.Default;

			return new Dpi(
				(uint)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
				(uint)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
		}

		/// <summary>
		/// Get system DPI.
		/// </summary>
		/// <returns>DPI struct</returns>
		public static Dpi GetSystemDpi()
		{
			var screen = IntPtr.Zero;

			try
			{
				screen = NativeMethod.GetDC(IntPtr.Zero);

				return new Dpi(
					(uint)NativeMethod.GetDeviceCaps(screen, NativeMethod.LOGPIXELSX),
					(uint)NativeMethod.GetDeviceCaps(screen, NativeMethod.LOGPIXELSY));
			}
			finally
			{
				if (screen != IntPtr.Zero)
					NativeMethod.ReleaseDC(IntPtr.Zero, screen);
			}
		}

		#endregion


		#region Per-Monitor DPI

		/// <summary>
		/// Get Per-Monitor DPI of the monitor to which a specified Window belongs.
		/// </summary>
		/// <param name="sourceVisual">Source Window</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromVisual(Visual sourceVisual)
		{
			if (!OsVersion.IsEightOneOrNewer)
				return Dpi.Default;

			var source = PresentationSource.FromVisual(sourceVisual) as HwndSource;
			if (source == null)
				return Dpi.Default;

			var handleMonitor = NativeMethod.MonitorFromWindow(
				source.Handle,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		/// <summary>
		/// Get Per-Monitor DPI of the monitor to which a specified Rect belongs.
		/// </summary>
		/// <param name="sourceRect">Source Rect</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromRect(NativeMethod.RECT sourceRect)
		{
			if (!OsVersion.IsEightOneOrNewer)
				return Dpi.Default;

			var handleMonitor = NativeMethod.MonitorFromRect(
				ref sourceRect,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		private static Dpi GetDpi(IntPtr handleMonitor)
		{
			if (handleMonitor == IntPtr.Zero)
				return Dpi.Default;

			uint dpiX = 1;
			uint dpiY = 1;

			NativeMethod.GetDpiForMonitor(
				handleMonitor,
				NativeMethod.MONITOR_DPI_TYPE.MDT_Default,
				ref dpiX,
				ref dpiY);

			return new Dpi(dpiX, dpiY);
		}

		#endregion
	}
}