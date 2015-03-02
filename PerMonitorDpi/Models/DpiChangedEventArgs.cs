using System;

namespace PerMonitorDpi.Models
{
	/// <summary>
	/// DPI changed event args
	/// </summary>
	public class DpiChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Old DPI
		/// </summary>
		public Dpi OldDpi { get; private set; }

		/// <summary>
		/// New DPI
		/// </summary>
		public Dpi NewDpi { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="oldDpi">Old DPI</param>
		/// <param name="newDpi">New DPI</param>
		public DpiChangedEventArgs(Dpi oldDpi, Dpi newDpi)
		{
			this.OldDpi = oldDpi;
			this.NewDpi = newDpi;
		}
	}
}