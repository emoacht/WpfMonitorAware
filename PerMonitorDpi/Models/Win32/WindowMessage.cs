
namespace PerMonitorDpi.Models.Win32
{
	public enum WindowMessage
	{
		WM_DPICHANGED = 0x02E0,

		WM_ENTERSIZEMOVE = 0x0231,
		WM_EXITSIZEMOVE = 0x0232,

		WM_MOVE = 0x0003,
		WM_SIZE = 0x0005,

		WM_ACTIVATE = 0x0006,

		WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
	}
}