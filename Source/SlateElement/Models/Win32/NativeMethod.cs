using System;
using System.Runtime.InteropServices;

namespace SlateElement.Models.Win32
{
	internal static class NativeMethod
	{
		#region Common

		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint(
			POINT pt,
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

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfo(
			IntPtr hMonitor,
			ref MONITORINFO lpmi);

		[StructLayout(LayoutKind.Sequential)]
		public struct MONITORINFO
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
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

		public const uint MONITORINFOF_PRIMARY = 0x00000001;

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(
			IntPtr hWnd,
			out RECT lpRect);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowPlacement(
			IntPtr hWnd,
			ref WINDOWPLACEMENT lpwndpl);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPlacement(
			IntPtr hWnd,
			[In] ref WINDOWPLACEMENT lpwndpl);

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT
		{
			public uint length;
			public uint flags;
			public uint showCmd;
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}

		public const uint SW_SHOWNORMAL = 1;

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}

		#endregion

		#region DPI

		// This method never succeeds.
		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PhysicalToLogicalPointForPerMonitorDPI(
			IntPtr hWnd,
			ref POINT lpPoint);

		// This method never succeeds.
		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LogicalToPhysicalPointForPerMonitorDPI(
			IntPtr hWnd,
			ref POINT lpPoint);

		#endregion
	}
}