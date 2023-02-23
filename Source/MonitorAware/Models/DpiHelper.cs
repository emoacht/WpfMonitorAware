using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using MonitorAware.Helper;
using static MonitorAware.Models.Win32.NativeMethod;

namespace MonitorAware.Models
{
	/// <summary>
	/// Utility methods for <see cref="System.Windows.DpiScale"/>
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
			return (awareness == PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
		}

		/// <summary>
		/// Gets DPI awareness of the current process.
		/// </summary>
		/// <returns>If successfully gets, PROCESS_DPI_AWARENESS. Otherwise, null.</returns>
		internal static PROCESS_DPI_AWARENESS? GetDpiAwareness()
		{
			if (!OsVersion.IsEightOneOrNewer)
				return null;

			var result = GetProcessDpiAwareness(
				IntPtr.Zero, // Current process
				out PROCESS_DPI_AWARENESS value);
			if (result != S_OK)
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
			if (OsVersion.IsRedstoneOneOrNewer)
			{
				double pixelsPerInch = GetDpiForSystem();
				return new DpiScale(
					pixelsPerInch / DefaultPixelsPerInch,
					pixelsPerInch / DefaultPixelsPerInch);
			}

			var deviceHandle = IntPtr.Zero;
			try
			{
				deviceHandle = GetDC(IntPtr.Zero);
				if (deviceHandle == IntPtr.Zero)
					return Identity; // Fallback

				return new DpiScale(
					GetDeviceCaps(deviceHandle, LOGPIXELSX) / DefaultPixelsPerInch,
					GetDeviceCaps(deviceHandle, LOGPIXELSY) / DefaultPixelsPerInch);
			}
			finally
			{
				if (deviceHandle != IntPtr.Zero)
					ReleaseDC(IntPtr.Zero, deviceHandle);
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

			var monitorHandle = MonitorFromWindow(
				source.Handle,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(monitorHandle);
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

			var monitorHandle = MonitorFromPoint(
				point,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(monitorHandle);
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

			RECT buffer = rect;
			var monitorHandle = MonitorFromRect(
				ref buffer,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			return GetDpi(monitorHandle);
		}

		private static DpiScale GetDpi(IntPtr monitorHandle)
		{
			if (monitorHandle == IntPtr.Zero)
				return SystemDpi;

			var result = GetDpiForMonitor(
				monitorHandle,
				MONITOR_DPI_TYPE.MDT_Default,
				out uint dpiX,
				out uint dpiY);
			if (result != S_OK)
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

			var taskBarHandle = FindWindowEx(
				IntPtr.Zero,
				IntPtr.Zero,
				"Shell_TrayWnd",
				string.Empty);
			if (taskBarHandle == IntPtr.Zero)
				return SystemDpi;

			var notificationAreaHandle = FindWindowEx(
				taskBarHandle,
				IntPtr.Zero,
				"TrayNotifyWnd",
				string.Empty);
			if (notificationAreaHandle == IntPtr.Zero)
				return SystemDpi;

			var monitorHandle = MonitorFromWindow(
				notificationAreaHandle,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);

			return GetDpi(monitorHandle);
		}

		#endregion
	}
}