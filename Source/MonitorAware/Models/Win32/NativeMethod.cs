using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorAware.Models.Win32
{
	internal static class NativeMethod
	{
		#region Common

		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint(
			POINT pt,
			MONITOR_DEFAULTTO dwFlags);

		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromRect(
			ref RECT lprc,
			MONITOR_DEFAULTTO dwFlags);

		[DllImport("User32.dll")]
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
		public struct POINT
		{
			public int x;
			public int y;

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public static implicit operator POINT(System.Windows.Point point) => new POINT((int)point.X, (int)point.Y);
			public static implicit operator System.Windows.Point(POINT point) => new System.Windows.Point(point.x, point.y);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public int Width => right - left;
			public int Height => bottom - top;

			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public static implicit operator RECT(System.Windows.Rect rect) => new RECT((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
			public static implicit operator System.Windows.Rect(RECT rect) => new System.Windows.Rect(rect.left, rect.top, rect.Width, rect.Height);
		}

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(
			IntPtr hwndParent,
			IntPtr hwndChildAfter,
			string lpszClass,
			string lpszWindow);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(
			IntPtr hWnd,
			out RECT lpRect);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int X,
			int Y,
			int cx,
			int cy,
			SWP uFlags);

		[Flags]
		public enum SWP
		{
			SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOZORDER = 0x0004,
			SWP_NOREDRAW = 0x0008,
			SWP_NOACTIVATE = 0x0010,
			SWP_DRAWFRAME = 0x0020,
			SWP_FRAMECHANGED = 0x0020,
			SWP_SHOWWINDOW = 0x0040,
			SWP_HIDEWINDOW = 0x0080,
			SWP_NOCOPYBITS = 0x0100,
			SWP_NOOWNERZORDER = 0x0200,
			SWP_NOREPOSITION = 0x0200,
			SWP_NOSENDCHANGING = 0x0400,
			SWP_DEFERERASE = 0x2000,
			SWP_ASYNCWINDOWPOS = 0x4000
		}

		public const int S_OK = 0x0;

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
			out uint dpiX,
			out uint dpiY);

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
	}
}