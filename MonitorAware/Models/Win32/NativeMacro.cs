
namespace MonitorAware.Models.Win32
{
	internal static class NativeMacro
	{
		/// <summary>
		/// Equivalent to LOWORD
		/// </summary>
		/// <param name="dword">Dword (unit)</param>
		/// <returns>Low-order Word (ushort)</returns>
		public static ushort GetLoWord(uint dword)
		{
			return (ushort)(dword & 0xffff);
		}

		/// <summary>
		/// Equivalent to HIWORD
		/// </summary>
		/// <param name="dword">Dword (uint)</param>
		/// <returns>High-order Word (ushort)</returns>
		public static ushort GetHiWord(uint dword)
		{
			return (ushort)(dword >> 16);
		}
	}
}