using System;

namespace WpfExtendedWindow.Helper
{
	/// <summary>
	/// OS version information
	/// </summary>
	internal static class OsVersion
	{
		/// <summary>
		/// Whether OS is Windows Vista or newer
		/// </summary>
		/// <remarks>Windows Vista = version 6.0</remarks>
		public static bool IsVistaOrNewer
		{
			get => (new Version(6, 0) <= Environment.OSVersion.Version);
		}
	}
}