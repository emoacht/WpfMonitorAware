using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorAware.Models.Win32
{
	internal static class NativeMethod
	{
		#region Common

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromRect(
			ref RECT lprc,
			MONITOR_DEFAULTTO dwFlags);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromWindow(
			IntPtr hwnd,
			MONITOR_DEFAULTTO dwFlags);

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

		[DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfo(
			IntPtr hMonitor,
			ref MONITORINFOEX lpmi);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct MONITORINFOEX
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // CCHDEVICENAME is defined to be 32.
			public string szDevice;
		}

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

		#endregion


		#region DPI

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
		public static extern int GetDpiForMonitor(
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

		#endregion


		#region Color Profile

		[DllImport("Gdi32.dll", EntryPoint = "GetICMProfileW", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetICMProfile(
			IntPtr hDC,
			ref uint lpcbName,
			StringBuilder lpszFilename);

		[DllImport("Gdi32.dll", SetLastError = true)]
		public static extern IntPtr CreateDC(
			string lpszDriver,
			string lpszDevice,
			string lpszOutput,
			IntPtr lpInitData);

		[DllImport("Gdi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteDC(IntPtr hdc);

		#endregion


		#region Constants for WM_ACTIVATE

		/// <summary>
		/// Activated by some method other than a mouse click.
		/// </summary>
		public const int WA_ACTIVE = 1;

		/// <summary>
		/// Activated by a mouse click.
		/// </summary>
		public const int WA_CLICKACTIVE = 2;

		/// <summary>
		/// Deactivated.
		/// </summary>
		public const int WA_INACTIVE = 0;

		#endregion


		#region Constants for WM_SIZE

		/// <summary>
		/// Some other window is maximized.
		/// </summary>
		public const int SIZE_MAXHIDE = 4;

		/// <summary>
		/// The window has been maximized.
		/// </summary> 
		public const int SIZE_MAXIMIZED = 2;

		/// <summary>
		/// Some other window has been restored to its former size.
		/// </summary>
		public const int SIZE_MAXSHOW = 3;

		/// <summary>
		/// The window has been minimized.
		/// </summary>
		public const int SIZE_MINIMIZED = 1;

		/// <summary>
		/// The window has been resized, but neither the SIZE_MINIMIZED nor SIZE_MAXIMIZED value applies.
		/// </summary>
		public const int SIZE_RESTORED = 0;

		#endregion
	}
}