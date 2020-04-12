using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorAware.Extended.Models.Win32
{
	internal static class NativeMethod
	{
		#region Constants for WM_ACTIVATE

		/// <summary>
		/// Activated by some method other than a mouse click.
		/// </summary>
		public const int WA_ACTIVE = 1;

		/// <summary>
		/// Activated by a mouse click.
		/// </summary>
		public const int WA_CLICKACTIVE = 2;

		/// <summary>
		/// Deactivated.
		/// </summary>
		public const int WA_INACTIVE = 0;

		#endregion
	}
}