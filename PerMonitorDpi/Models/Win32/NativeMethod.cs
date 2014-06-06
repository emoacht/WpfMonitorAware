using System;
using System.Runtime.InteropServices;

namespace PerMonitorDpi.Models.Win32
{
	public static class NativeMethod
	{
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromRect(
			ref RECT lprc,
			MONITOR_DEFAULTTO dwFlags);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromWindow(
			IntPtr hwnd,
			MONITOR_DEFAULTTO dwFlags);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public RECT(int left, int top, int right, int bottom)
				: this()
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public RECT(System.Windows.Rect rect)
				: this((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom)
			{ }
		}

		public enum MONITOR_DEFAULTTO : uint
		{
			/// <summary>
			/// If no display monitor intersects, returns null.
			/// </summary>
			MONITOR_DEFAULTTONULL = 0x00000000,

			/// <summary>
			/// If no display monitor intersects, returns a handle to the primary display monitor.
			/// </summary>
			MONITOR_DEFAULTTOPRIMARY = 0x00000001,

			/// <summary>
			/// If no display monitor intersects, returns a handle to the display monitor that is nearest to the rectangle.
			/// </summary>
			MONITOR_DEFAULTTONEAREST = 0x00000002,
		}

		[DllImport("Shcore.dll", SetLastError = true)]
		public static extern void GetDpiForMonitor(
			IntPtr hmonitor,
			MONITOR_DPI_TYPE dpiType,
			ref uint dpiX,
			ref uint dpiY);

		public enum MONITOR_DPI_TYPE
		{
			/// <summary>
			/// Effective DPI that incorporates accessibility overrides and matches what Desktop Window Manage (DWM) uses to scale desktop applications
			/// </summary>
			MDT_Effective_DPI = 0,

			/// <summary>
			/// DPI that ensures rendering at a compliant angular resolution on the screen, without incorporating accessibility overrides
			/// </summary>
			MDT_Angular_DPI = 1,

			/// <summary>
			/// Linear DPI of the screen as measured on the screen itself
			/// </summary>
			MDT_Raw_DPI = 2,

			/// <summary>
			/// Default DPI
			/// </summary>
			MDT_Default = MDT_Effective_DPI
		}
	}
}
