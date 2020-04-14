using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

using MonitorAware.Models.Win32;

namespace MonitorAware.Models
{
	/// <summary>
	/// Utility methods for DPI changed event
	/// </summary>
	internal static class DpiChangedEventHelper
	{
		/// <summary>
		/// Converts WM_DPICHANGED message's wParam value to DPI.
		/// </summary>
		/// <param name="wParam">wParam value</param>
		/// <returns>DPI scale information</returns>
		internal static DpiScale ConvertPointerToDpi(IntPtr wParam)
		{
			var dword = (uint)wParam;
			var dpiX = (ushort)(dword & 0xffff);
			var dpiY = (ushort)(dword >> 16);

			return new DpiScale(dpiX / DpiHelper.DefaultPixelsPerInch, dpiY / DpiHelper.DefaultPixelsPerInch);
		}

		/// <summary>
		/// Converts WM_DPICHANGED message's lParam value to Rect.
		/// </summary>
		/// <param name="lParam">lParam value</param>
		/// <returns>Rect</returns>
		internal static Rect ConvertPointerToRect(IntPtr lParam) => Marshal.PtrToStructure<NativeMethod.RECT>(lParam);

		private static ConstructorInfo _constructorInfo;

		/// <summary>
		/// Creates DpiChangedEventArgs with specified information.
		/// </summary>
		/// <param name="window">Source Window</param>
		/// <param name="oldDpi">Old DPI scale information</param>
		/// <param name="newDpi">New DPI scale information</param>
		/// <returns>DpiChangedEventArgs</returns>
		public static DpiChangedEventArgs Create(Window window, DpiScale oldDpi, DpiScale newDpi)
		{
			_constructorInfo ??= typeof(DpiChangedEventArgs).GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				new Type[] { typeof(DpiScale), typeof(DpiScale), typeof(RoutedEvent), typeof(object) },
				null);

			if (_constructorInfo is null)
				return default;

			return (DpiChangedEventArgs)_constructorInfo?.Invoke(new object[] { oldDpi, newDpi, Window.DpiChangedEvent, window });
		}
	}
}