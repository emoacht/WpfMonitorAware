using System.Runtime.InteropServices;
using System.Windows.Media;

namespace SlateElement.Helper
{
	/// <summary>
	/// Window chrome color information
	/// </summary>
	public static class WindowChromeColor
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
		/// Attemps to get OS's window chrome Color.
		/// </summary>
		/// <param name="chromeColor">Window chrome Color</param>
		/// <returns>True if successfully gets. False otherwise.</returns>		
		public static bool TryGetChromeColor(out Color chromeColor)
		{
			var buffer = GetChromeColor();
			if (buffer.HasValue)
			{
				chromeColor = buffer.Value;
				return true;
			}
			chromeColor = default;
			return false;
		}

		/// <summary>
		/// Gets OS's window chrome Color.
		/// </summary>
		/// <returns>If successfully gets, window chrome Color. If not, null.</returns>
		/// <remarks>This method is intended to get window chrome Color of Windows 8 or newer.</remarks>
		public static Color? GetChromeColor()
		{
			// DWM APIs are supported in Windows Vista or newer.
			try
			{
				if ((DwmIsCompositionEnabled(out bool isEnabled) != S_OK) || !isEnabled)
					return null;

				// This API is undocumented and so may become unusable in future versions of OSes.
				if (DwmGetColorizationParameters(out DWMCOLORIZATIONPARAMS parameters) != S_OK)
					return null;

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
			catch
			{
				return null;
			}
		}
	}
}