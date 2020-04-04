using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using MonitorAware.Helper;
using MonitorAware.Models.Win32;

namespace MonitorAware.Models
{
	/// <summary>
	/// Checks DPI.
	/// </summary>
	public static class DpiChecker
	{
		#region DPI Awareness

		/// <summary>
		/// Checks if current process is Per-Monitor DPI aware.
		/// </summary>
		/// <returns>True if Per-Monitor DPI aware</returns>
		internal static bool IsPerMonitorDpiAware()
		{
			var awareness = GetDpiAwareness();
			return (awareness == NativeMethod.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
		}

		/// <summary>
		/// Gets DPI awareness of current process.
		/// </summary>
		/// <returns>If succeeded, Nullable PROCESS_DPI_AWARENESS. If failed, null.</returns>
		internal static NativeMethod.PROCESS_DPI_AWARENESS? GetDpiAwareness()
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
		/// System DPI
		/// </summary>
		/// <remarks>System DPI will not change during run time.</remarks>
		internal static readonly Dpi SystemDpi = GetSystemDpi();

		/// <summary>
		/// Gets system DPI.
		/// </summary>
		/// <param name="sourceVisual">Source Visual</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetSystemDpi(Visual sourceVisual)
		{
			if (sourceVisual == null)
				throw new ArgumentNullException(nameof(sourceVisual));

			var source = PresentationSource.FromVisual(sourceVisual);
			if (source?.CompositionTarget == null)
				return Dpi.Default;

			return new Dpi(
				(uint)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
				(uint)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
		}

		/// <summary>
		/// Gets system DPI.
		/// </summary>
		/// <returns>DPI struct</returns>
		public static Dpi GetSystemDpi()
		{
			var screen = IntPtr.Zero;

			try
			{
				screen = NativeMethod.GetDC(IntPtr.Zero);
				if (screen == IntPtr.Zero)
					return Dpi.Default;

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
		/// Gets Per-Monitor DPI of the monitor to which a specified Window belongs.
		/// </summary>
		/// <param name="sourceVisual">Source Visual</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromVisual(Visual sourceVisual)
		{
			if (sourceVisual == null)
				throw new ArgumentNullException(nameof(sourceVisual));

			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			var source = PresentationSource.FromVisual(sourceVisual) as HwndSource;
			if (source == null)
				return SystemDpi;

			var handleMonitor = NativeMethod.MonitorFromWindow(
				source.Handle,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor to which a specified Rect belongs.
		/// </summary>
		/// <param name="sourceRect">Source Rect</param>
		/// <returns>DPI struct</returns>
		public static Dpi GetDpiFromRect(Rect sourceRect)
		{
			if (sourceRect == Rect.Empty)
				throw new ArgumentNullException(nameof(sourceRect));

			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			var nativeRect = new NativeMethod.RECT(sourceRect);
			var handleMonitor = NativeMethod.MonitorFromRect(
				ref nativeRect,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		private static Dpi GetDpi(IntPtr handleMonitor)
		{
			if (handleMonitor == IntPtr.Zero)
				return SystemDpi;

			uint dpiX = 1;
			uint dpiY = 1;

			var result = NativeMethod.GetDpiForMonitor(
				handleMonitor,
				NativeMethod.MONITOR_DPI_TYPE.MDT_Default,
				ref dpiX,
				ref dpiY);
			if (result != 0) // 0 means S_OK.
				return SystemDpi;

			return new Dpi(dpiX, dpiY);
		}

		#endregion

		#region Notification Area DPI

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor in which the notification area is contained.
		/// </summary>
		/// <returns>DPI struct</returns>
		public static Dpi GetNotificationAreaDpi()
		{
			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			var handleTaskBar = NativeMethod.FindWindowEx(
				IntPtr.Zero,
				IntPtr.Zero,
				"Shell_TrayWnd",
				string.Empty);
			if (handleTaskBar == IntPtr.Zero)
				return SystemDpi;

			var handleNotificationArea = NativeMethod.FindWindowEx(
				handleTaskBar,
				IntPtr.Zero,
				"TrayNotifyWnd",
				string.Empty);
			if (handleNotificationArea == IntPtr.Zero)
				return SystemDpi;

			var handleMonitor = NativeMethod.MonitorFromWindow(
				handleNotificationArea,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);

			return GetDpi(handleMonitor);
		}

		#endregion
	}
}