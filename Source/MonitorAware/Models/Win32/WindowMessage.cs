
namespace MonitorAware.Models.Win32
{
	internal static class WindowMessage
	{
		public const int WM_DPICHANGED = 0x02E0;
		public const int WM_ENTERSIZEMOVE = 0x0231;
		public const int WM_EXITSIZEMOVE = 0x0232;
		public const int WM_MOVE = 0x0003;
		public const int WM_SIZE = 0x0005;

		#region Constants for WM_SIZE

		public const int SIZE_MAXHIDE = 4;
		public const int SIZE_MAXIMIZED = 2;
		public const int SIZE_MAXSHOW = 3;
		public const int SIZE_MINIMIZED = 1;
		public const int SIZE_RESTORED = 0;

		#endregion
	}
}