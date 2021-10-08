using System.Runtime.InteropServices;
using System.Windows.Media;

namespace WpfExtendedWindow.Helper
{
	/// <summary>
	/// Window chrome color information
	/// </summary>
	internal static class WindowChromeColor
	{
		#region Win32

		[DllImport("Dwmapi.dll", SetLastError = true)]
		private static extern int DwmIsCompositionEnabled(
			[MarshalAs(UnmanagedType.Bool)] out bool pfEnabled);

		[DllImport("Dwmapi.dll", SetLastError = true)]
		private static extern void DwmGetColorizationColor(
			out uint pcrColorization,
			[MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

		[DllImport("Dwmapi.dll", EntryPoint = "#127", SetLastError = true)] // Undocumented API
		private static extern int DwmGetColorizationParameters(
			out DWMCOLORIZATIONPARAMS parameters);

		[StructLayout(LayoutKind.Sequential)]
		private struct DWMCOLORIZATIONPARAMS
		{
			public uint colorizationColor;
			public uint colorizationAfterglow;
			public uint colorizationColorBalance;
			public uint colorizationAfterglowBalance;
			public uint colorizationBlurBalance;
			public uint colorizationGlassReflectionIntensity;
			public uint colorizationOpaqueBlend;
		}

		private const int S_OK = 0x0;

		#endregion

		/// <summary>
		/// Gets OS's window chrome color.
		/// </summary>
		/// <returns>Window chrome color</returns>
		/// <remarks>This method is intended to get window chrome color of Windows 8 or newer.</remarks>
		public static Color? GetChromeColor()
		{
			if (!OsVersion.IsVistaOrNewer)
				return null;

			if ((DwmIsCompositionEnabled(out bool isEnabled) != S_OK) || !isEnabled)
				return null;

			DWMCOLORIZATIONPARAMS parameters;
			try
			{
				// This API is undocumented and so may become unusable in future versions of OSes.
				if (DwmGetColorizationParameters(out parameters) != S_OK)
					return null;
			}
			catch
			{
				return null;
			}

			// Convert colorization color parameter to Color ignoring alpha channel.
			var targetColor = Color.FromRgb(
				(byte)(parameters.colorizationColor >> 16),
				(byte)(parameters.colorizationColor >> 8),
				(byte)parameters.colorizationColor);

			// Prepare base gray color.
			var baseColor = Color.FromRgb(217, 217, 217);

			// Blend the two colors using colorization color balance parameter.
			return targetColor.ToBlended(baseColor, (double)(100 - parameters.colorizationColorBalance));
		}
	}
}