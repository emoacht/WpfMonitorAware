using System;
using System.Windows;

using static MonitorAware.Models.Win32.NativeMethod;

namespace MonitorAware.Models
{
	/// <summary>
	/// Utility methods for <see cref="System.Windows.Window"/>
	/// </summary>
	public static class WindowHelper
	{
		/// <summary>
		/// Attemps to get a specified Window's rectangle.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="windowRect">Window's rectangle</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetWindowRect(IntPtr windowHandle, out Rect windowRect)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			if (GetWindowRect(
				windowHandle,
				out RECT buffer))
			{
				windowRect = buffer;
				return true;
			}
			windowRect = default;
			return false;
		}

		/// <summary>
		/// Sets a specified Window's rectangle.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="windowRect">Window's rectangle</param>
		/// <returns>True if successfully sets. False otherwise.</returns>
		public static bool SetWindowRect(IntPtr windowHandle, Rect windowRect)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			return SetWindowPos(
				windowHandle,
				IntPtr.Zero,
				(int)windowRect.X,
				(int)windowRect.Y,
				(int)windowRect.Width,
				(int)windowRect.Height,
				0);
		}

		/// <summary>
		/// Sets a specified Window's location.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="X">Window's X coordinate</param>
		/// <param name="Y">Window's Y coordinate</param>
		/// <returns>True if successfully sets. False otherwise.</returns>
		public static bool SetWindowLocation(IntPtr windowHandle, double X, double Y)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			return SetWindowPos(
				windowHandle,
				IntPtr.Zero,
				(int)X,
				(int)Y,
				0,
				0,
				SWP.SWP_NOSIZE);
		}

		/// <summary>
		/// Sets a specified Window's size.
		/// </summary>
		/// <param name="windowHandle">Window's handle</param>
		/// <param name="size">Window's size</param>
		/// <returns>True if successfully sets. False otherwise.</returns>
		public static bool SetWindowSize(IntPtr windowHandle, Size size)
		{
			if (windowHandle == IntPtr.Zero)
				throw new ArgumentNullException(nameof(windowHandle));

			return SetWindowPos(
				windowHandle,
				IntPtr.Zero,
				0,
				0,
				(int)size.Width,
				(int)size.Height,
				SWP.SWP_NOMOVE);
		}
	}
}