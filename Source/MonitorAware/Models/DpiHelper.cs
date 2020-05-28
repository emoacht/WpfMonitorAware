using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using MonitorAware.Helper;
using MonitorAware.Models.Win32;

namespace MonitorAware.Models
{
	/// <summary>
	/// Utility methods for DPI
	/// </summary>
	public static class DpiHelper
	{
		#region Default

		/// <summary>
		/// Default pixels per inch
		/// </summary>
		public const double DefaultPixelsPerInch = 96D;

		/// <summary>
		/// Identity DPI
		/// </summary>
		public static DpiScale Identity { get; } = new DpiScale(1D, 1D);

		/// <summary>
		/// Determines whether a specified DPI indicates identity scaling.
		/// </summary>
		/// <param name="dpi">DPI scale information</param>
		/// <returns>True if identity scaling. False otherwise.</returns>
		public static bool IsIdentity(this DpiScale dpi) => dpi.Equals(Identity);

		#endregion

		#region DPI Awareness

		/// <summary>
		/// Determines whether the current process is Per-Monitor DPI aware.
		/// </summary>
		/// <returns>True if Per-Monitor DPI aware. False otherwise.</returns>
		public static bool IsPerMonitorDpiAware()
		{
			var awareness = GetDpiAwareness();
			return (awareness == NativeMethod.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
		}

		/// <summary>
		/// Gets DPI awareness of the current process.
		/// </summary>
		/// <returns>If successfully gets, PROCESS_DPI_AWARENESS. Otherwise, null.</returns>
		internal static NativeMethod.PROCESS_DPI_AWARENESS? GetDpiAwareness()
		{
			if (!OsVersion.IsEightOneOrNewer)
				return null;

			var result = NativeMethod.GetProcessDpiAwareness(
				IntPtr.Zero, // Current process
				out NativeMethod.PROCESS_DPI_AWARENESS value);
			if (result != NativeMethod.S_OK)
				return null;

			return value;
		}

		#endregion

		#region System DPI

		/// <summary>
		/// System DPI
		/// </summary>
		/// <remarks>System DPI will not change during run time.</remarks>
		public static DpiScale SystemDpi { get; } = GetSystemDpi();

		/// <summary>
		/// Gets system DPI.
		/// </summary>
		/// <returns>DPI scale information</returns>
		internal static DpiScale GetSystemDpi()
		{
			var screen = IntPtr.Zero;
			try
			{
				screen = NativeMethod.GetDC(IntPtr.Zero);
				if (screen == IntPtr.Zero)
					return Identity; // Fallback

				return new DpiScale(
					NativeMethod.GetDeviceCaps(screen, NativeMethod.LOGPIXELSX) / DefaultPixelsPerInch,
					NativeMethod.GetDeviceCaps(screen, NativeMethod.LOGPIXELSY) / DefaultPixelsPerInch);
			}
			finally
			{
				if (screen != IntPtr.Zero)
					NativeMethod.ReleaseDC(IntPtr.Zero, screen);
			}
		}

		/// <summary>
		/// Gets system DPI.
		/// </summary>
		/// <param name="visual">Source Visual</param>
		/// <returns>DPI scale information</returns>
		internal static DpiScale GetSystemDpi(Visual visual)
		{
			if (visual is null)
				throw new ArgumentNullException(nameof(visual));

			var source = PresentationSource.FromVisual(visual);
			if (source?.CompositionTarget is null)
				return Identity; // Fallback

			return new DpiScale(
				source.CompositionTarget.TransformToDevice.M11,
				source.CompositionTarget.TransformToDevice.M22);
		}

		#endregion

		#region Per-Monitor DPI

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor to which a specified Visual belongs.
		/// </summary>
		/// <param name="visual">Source Visual</param>
		/// <returns>DPI scale information</returns>
		/// <remarks>This method is equivalent to VisualTreeHelper.GetDpi method.</remarks>
		public static DpiScale GetDpiFromVisual(Visual visual)
		{
			if (visual is null)
				throw new ArgumentNullException(nameof(visual));

			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			var source = PresentationSource.FromVisual(visual) as HwndSource;
			if (source is null)
				return SystemDpi;

			var handleMonitor = NativeMethod.MonitorFromWindow(
				source.Handle,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor to which a specified Point belongs.
		/// </summary>
		/// <param name="point">Source Point</param>
		/// <returns>DPI scale information</returns>
		public static DpiScale GetDpiFromPoint(Point point)
		{
			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			var handleMonitor = NativeMethod.MonitorFromPoint(
				point,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor to which a specified Rect belongs.
		/// </summary>
		/// <param name="rect">Source Rect</param>
		/// <returns>DPI scale information</returns>
		public static DpiScale GetDpiFromRect(Rect rect)
		{
			if (rect == Rect.Empty)
				throw new ArgumentNullException(nameof(rect));

			if (!OsVersion.IsEightOneOrNewer)
				return SystemDpi;

			NativeMethod.RECT nativeRect = rect;
			var handleMonitor = NativeMethod.MonitorFromRect(
				ref nativeRect,
				NativeMethod.MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(handleMonitor);
		}

		private static DpiScale GetDpi(IntPtr handleMonitor)
		{
			if (handleMonitor == IntPtr.Zero)
				return SystemDpi;

			var result = NativeMethod.GetDpiForMonitor(
				handleMonitor,
				NativeMethod.MONITOR_DPI_TYPE.MDT_Default,
				out uint dpiX,
				out uint dpiY);
			if (result != NativeMethod.S_OK)
				return SystemDpi;

			return new DpiScale(dpiX / DefaultPixelsPerInch, dpiY / DefaultPixelsPerInch);
		}

		#endregion

		#region Notification Area DPI

		/// <summary>
		/// Gets Per-Monitor DPI of the monitor in which the notification area is contained.
		/// </summary>
		/// <returns>DPI scale information</returns>
		public static DpiScale GetNotificationAreaDpi()
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