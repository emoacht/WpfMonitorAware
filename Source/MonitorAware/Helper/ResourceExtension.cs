using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MonitorAware.Helper
{
	/// <summary>
	/// Extension method for <see cref="FrameworkElement"/> to manage resources
	/// </summary>
	internal static class ResourceExtension
	{
		/// <summary>
		/// Adds/Removes resources to/from <see cref="Application"/> resources.
		/// </summary>
		/// <param name="app">Target Application</param>
		/// <param name="newUriString">Uri string of new resources to be added</param>
		/// <param name="oldUriString">Uri string of old resources to be removed (case-sensitive)</param>
		public static void ApplyResource(this Application app, string newUriString, string oldUriString = "")
		{
			if (!string.IsNullOrWhiteSpace(oldUriString))
			{
				var oldDictionary = app.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.OriginalString == oldUriString);
				if (oldDictionary != null)
					app.Resources.MergedDictionaries.Remove(oldDictionary);
			}

			if (string.IsNullOrWhiteSpace(newUriString))
				return;

			var newDictionary = new ResourceDictionary();

			try
			{
				newDictionary.Source = new Uri(newUriString, UriKind.RelativeOrAbsolute);
			}
			catch (IOException ex)
			{
				Debug.WriteLine($"Failed to apply resources to Application resources.\r\n{ex}");
				return;
			}

			app.Resources.MergedDictionaries.Add(newDictionary);
		}

		/// <summary>
		/// Adds/Removes resources to/from <see cref="ContentControl"/> resources.
		/// </summary>
		/// <param name="control">Target ContentControl</param>
		/// <param name="newUriString">Uri string of new resources to be added</param>
		/// <param name="oldUriString">Uri string of old resources to be removed (case-sensitive)</param>
		public static void ApplyResource(this ContentControl control, string newUriString, string oldUriString = "")
		{
			if (!string.IsNullOrWhiteSpace(oldUriString))
			{
				var oldDictionary = control.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.OriginalString == oldUriString);
				if (oldDictionary != null)
					control.Resources.MergedDictionaries.Remove(oldDictionary);
			}

			if (string.IsNullOrWhiteSpace(newUriString))
				return;

			var newDictionary = new ResourceDictionary();

			try
			{
				newDictionary.Source = new Uri(newUriString, UriKind.RelativeOrAbsolute);
			}
			catch (IOException ex)
			{
				Debug.WriteLine($"Failed to apply resources to ContentControl resources.\r\n{ex}");
				return;
			}

			control.Resources.MergedDictionaries.Add(newDictionary);
		}
	}
}