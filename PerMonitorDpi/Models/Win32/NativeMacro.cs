
namespace PerMonitorDpi.Models.Win32
{
	public static class NativeMacro
	{
		/// <summary>
		/// Equivalent to LOWORD
		/// </summary>
		public static ushort GetLoWord(uint dword)
		{
			return (ushort)(dword & 0xffff);
		}

		/// <summary>
		/// Equivalent to HIWORD
		/// </summary>
		public static ushort GetHiWord(uint dword)
		{
			return (ushort)(dword >> 16);
		}
	}
}
