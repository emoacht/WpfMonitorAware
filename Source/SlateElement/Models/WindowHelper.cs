using System;
using System.Runtime.InteropServices;
using System.Windows;

using static SlateElement.Models.Win32.NativeMethod;

namespace SlateElement.Models
{
	/// <summary>
	/// Utility methods for <see cref="System.Windows.Window"/>
	/// </summary>
	public static class WindowHelper
	{
		/// <summary>
		/// Attemps to get the mouse cursor point in screen.
		/// </summary>
		/// <param name="point">Mouse cursor point</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetMousePoint(out Point point)
		{
			if (GetCursorPos(out POINT buffer))
			{
				point = buffer;
				return true;
			}
			point = default;
			return false;
		}

		/// <summary>
		/// Attemps to get a specified Window's current rectangle.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="rect">Window's rectangle</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetWindowRect(IntPtr windowHandle, out Rect rect)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			if (GetWindowRect(windowHandle, out RECT buffer))
			{
				rect = buffer;
				return true;
			}
			rect = default;
			return false;
		}

		/// <summary>
		/// Attemps to get a specified Window's normal (restore) rectangle.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="rect">Window's rectangle</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetWindowNormalRect(IntPtr windowHandle, out Rect rect)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			var placement = new WINDOWPLACEMENT
			{
				length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>()
			};
			if (GetWindowPlacement(windowHandle, ref placement))
			{
				rect = placement.rcNormalPosition;
				return true;
			}
			rect = default;
			return false;
		}

		/// <summary>
		/// Sets a specified Window's normal rectangle changing the window to normal state.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="rect">Window's rectangle</param>
		/// <returns>True if successfully sets. False otherwise.</returns>
		public static bool SetWindowNormalRect(IntPtr windowHandle, Rect rect)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			var placement = new WINDOWPLACEMENT
			{
				length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
				showCmd = SW_SHOWNORMAL,
				rcNormalPosition = rect
			};
			return SetWindowPlacement(windowHandle, ref placement);
		}

		/// <summary>
		/// Attemps to get the monitor's rectangles to which a specified Window belongs.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="monitorRect">Monitor's (full) rectangle</param>
		/// <param name="workRect">Work area's rectangle</param>
		/// <param name="isPrimary">Whether the monitor is the primary monitor</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetMonitorRect(IntPtr windowHandle, out Rect monitorRect, out Rect workRect, out bool isPrimary)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			var monitorHandle = MonitorFromWindow(
				windowHandle,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

			var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
			if (GetMonitorInfo(monitorHandle, ref monitorInfo))
			{
				monitorRect = monitorInfo.rcMonitor;
				workRect = monitorInfo.rcWork;
				isPrimary = (monitorInfo.dwFlags == MONITORINFOF_PRIMARY);
				return true;
			}
			monitorRect = default;
			workRect = default;
			isPrimary = default;
			return false;
		}
	}
}