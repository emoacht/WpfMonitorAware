using System;

namespace MonitorAware.Extended.Helper
{
	/// <summary>
	/// OS version information
	/// </summary>
	internal static class OsVersion
	{
		private static readonly Version _ver = Environment.OSVersion.Version;

		/// <summary>
		/// Whether OS is Windows Vista or newer
		/// </summary>
		/// <remarks>Windows Vista = version 6.0</remarks>
		public static bool IsVistaOrNewer
		{
			get { return (6 <= _ver.Major); }
		}
	}
}