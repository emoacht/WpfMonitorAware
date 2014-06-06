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
		#region System DPI

		/// <summary>
		/// Get system DPI.
		/// </summary>
		/// <param name="targetVisual">Target Window</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetSystemDpi(Visual targetVisual)
		{
			var source = PresentationSource.FromVisual(targetVisual);
			if ((source == null) || (source.CompositionTarget == null))
				return Dpi.Default;

			return new Dpi(
				(uint)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
				(uint)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
		}

		#endregion


		#region Per-Monitor DPI

		/// <summary>
		/// Get Per-Monitor DPI of the monitor to which a specified Window belongs.
		/// </summary>
		/// <param name="targetVisual">Target Window</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromVisual(Visual targetVisual)
		{
			if (!OsVersion.IsEightOneOrNewer)
				return Dpi.Default;

			var source = PresentationSource.FromVisual(targetVisual) as HwndSource;
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
		/// <param name="targetRect">Target Rect</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromRect(NativeMethod.RECT targetRect)
		{
			if (!OsVersion.IsEightOneOrNewer)
				return Dpi.Default;

			var handleMonitor = NativeMethod.MonitorFromRect(
				ref targetRect,
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
