using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace MonitorAware.Helper
{
	/// <summary>
	/// WPF built-in functions information
	/// </summary>
	internal static class BuiltinFunction
	{
		/// <summary>
		/// Checks if scaling for Per-Monitor DPI is supported 
		/// </summary>
		/// <param name="window">Window class defined in the assembly</param>
		/// <returns>True if could not find any indication that built-in scaling is not supported</returns>
		/// <remarks>
		/// The prerequisites for built-in scaling are the following:
		/// * OS is Windows 10 Anniversary Update (Redstone 1) or newer.
		/// * Target framework of assembly is .NET Framework 4.6.2 or newer.
		/// * dpiAwareness in the application manifest is set to PerMonitor.
		///   (dpiAwareness value cannot be checked directly at run time.)
		/// 
		/// In addition, if Switch.System.Windows.DoNotScaleForDpiChanges is specified in
		/// the application configuration, it will have the following effects:
		/// * True - DISABLE built-in scaling even if the above conditions are met.
		/// * False - ENABLE built-in scaling even if target framework is older than 4.6.2.
		/// </remarks>
		public static bool IsScalingSupported(Window window)
		{
			if (!OsVersion.IsRedstoneOneOrNewer)
				return false;

			if (window == null)
				return true;

			var doNotScaleForDpiChanges = GetDoNotScaleForDpiChanges(window);
			if (doNotScaleForDpiChanges.HasValue)
				return !doNotScaleForDpiChanges.Value;

			var targetFrameworkVersion = TargetFramework.GetFrameworkVersion(window);
			if (targetFrameworkVersion == null)
				return true;

			return (new Version(4, 6, 2) <= targetFrameworkVersion);
		}

		private static bool? GetDoNotScaleForDpiChanges(Window window)
		{
			var assembly = Assembly.GetAssembly(window.GetType());

			var exeUri = new UriBuilder(assembly.CodeBase);
			var exePath = Uri.UnescapeDataString(exeUri.Path);

			try
			{
				var config = ConfigurationManager.OpenExeConfiguration(exePath);
				return GetDoNotScaleForDpiChanges(config);
			}
			catch
			{
				return null;
			}
		}

		private static bool? GetDoNotScaleForDpiChanges(Configuration config)
		{
			var xml = config.GetSection("runtime")?.SectionInformation?.GetRawXml();
			if (xml == null)
				return null;

			var section = XDocument.Parse(xml);
			var element = section?.Descendants()?.SingleOrDefault(x => x.Name.LocalName.Equals("AppContextSwitchOverrides", StringComparison.OrdinalIgnoreCase));
			var attribute = element?.Attributes()?.SingleOrDefault(x => x.Name.LocalName.Equals("value", StringComparison.OrdinalIgnoreCase));

			var fields = attribute.Value.Split('=').Select(x => x.Trim()).ToArray();
			if (fields.Length != 2)
				return null;

			if (!fields[0].Equals("Switch.System.Windows.DoNotScaleForDpiChanges", StringComparison.OrdinalIgnoreCase))
				return null;

			bool value;
			if (!bool.TryParse(fields[1], out value))
				return null;

			return value;
		}
	}
}