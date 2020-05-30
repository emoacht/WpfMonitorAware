using System;

namespace MonitorAware.Helper
{
	/// <summary>
	/// OS version information
	/// </summary>
	public static class OsVersion
	{
		/// <summary>
		/// Whether OS is Windows Vista or newer
		/// </summary>
		/// <remarks>Windows Vista = version 6.0</remarks>
		public static bool IsVistaOrNewer => (_isVistaOrNewer ??= IsEqualToOrNewer(6, 0));
		private static bool? _isVistaOrNewer;

		/// <summary>
		/// Whether OS is Windows 8.1 or newer
		/// </summary>
		/// <remarks>Windows 8.1 = version 6.3</remarks>
		public static bool IsEightOneOrNewer => (_isEightOneOrNewer ??= IsEqualToOrNewer(6, 3));
		private static bool? _isEightOneOrNewer;

		/// <summary>
		/// Whether OS is Windows 10 Anniversary Update (Redstone 1) or newer
		/// </summary>
		/// <remarks>Windows 10 Anniversary Update (1607) = version 10.0.14393</remarks>
		public static bool IsRedstoneOneOrNewer => (_isRedstoneOneOrNewer ??= IsEqualToOrNewer(10, 0, 14393));
		private static bool? _isRedstoneOneOrNewer;

		/// <summary>
		/// Whether OS is Windows 10 Creators Update (Redstone 2) or newer
		/// </summary>
		/// <remarks>Windows 10 Creators Update (1703) = version 10.0.15063</remarks>
		public static bool IsRedstoneTwoOrNewer => (_isRedstoneTwoOrNewer ??= IsEqualToOrNewer(10, 0, 15063));
		private static bool? _isRedstoneTwoOrNewer;

		private static bool IsEqualToOrNewer(int major, int minor, int build = 0) =>
			(new Version(major, minor, build) <= Environment.OSVersion.Version);
	}
}