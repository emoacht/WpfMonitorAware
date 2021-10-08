using System;

namespace MonitorAware.Models
{
	/// <summary>
	/// Color profile changed event args
	/// </summary>
	public class ColorProfileChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Old color profile path
		/// </summary>
		public string OldPath { get; }

		/// <summary>
		/// New color profile path
		/// </summary>
		public string NewPath { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="oldPath">Old color profile path</param>
		/// <param name="newPath">New color profile path</param>
		public ColorProfileChangedEventArgs(string oldPath, string newPath)
		{
			this.OldPath = oldPath;
			this.NewPath = newPath;
		}
	}
}