using System;
using System.Runtime.InteropServices;

namespace PerMonitorDpi.Models.Win32
{
	public static class NativeMethod
	{
		[DllImport("Gdi32.dll", SetLastError = true)]
		public static extern int GetDeviceCaps(
			IntPtr hdc,
			int nIndex);

		public const int LOGPIXELSX = 88;
		public const int LOGPIXELSY = 90;

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReleaseDC(
			IntPtr hWnd,
			IntPtr hDC);

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

			public System.Windows.Rect ToRect()
			{
				return new System.Windows.Rect(
					(double)this.left,
					(double)this.top,
					(double)(this.right - this.left),
					(double)(this.bottom - this.top));
			}
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
		public static extern int GetProcessDpiAwareness(
			IntPtr hprocess,
			out PROCESS_DPI_AWARENESS value);

		[DllImport("Shcore.dll", SetLastError = true)]
		public static extern int SetProcessDpiAwareness(
			PROCESS_DPI_AWARENESS value);

		public enum PROCESS_DPI_AWARENESS
		{
			/// <summary>
			/// Not DPI aware
			/// </summary>
			Process_DPI_Unaware = 0,

			/// <summary>
			/// System DPI aware
			/// </summary>
			Process_System_DPI_Aware = 1,

			/// <summary>
			/// Per-Monitor DPI aware
			/// </summary>
			Process_Per_Monitor_DPI_Aware = 2
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


		#region Constants for WM_ACTIVATE

		public const int WA_ACTIVE = 1;
		public const int WA_CLICKACTIVE = 2;
		public const int WA_INACTIVE = 0;

		#endregion


		#region Constants for WM_SIZE

		public const int SIZE_MAXHIDE = 4;
		public const int SIZE_MAXIMIZED = 2;
		public const int SIZE_MAXSHOW = 3;
		public const int SIZE_MINIMIZED = 1;
		public const int SIZE_RESTORED = 0;

		#endregion
	}
}
