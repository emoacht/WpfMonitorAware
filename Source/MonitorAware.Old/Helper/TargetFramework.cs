using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;

namespace MonitorAware.Helper
{
	/// <summary>
	/// Target framework (.NET Framework) information
	/// </summary>
	internal static class TargetFramework
	{
		/// <summary>
		/// Gets target framework version of a calling assembly. 
		/// </summary>
		/// <returns>If succeeded, target framework version. If failed, null.</returns>
		public static Version GetFrameworkVersion()
		{
			var assembly = Assembly.GetCallingAssembly();

			return GetFrameworkVersion(assembly);
		}

		/// <summary>
		/// Gets target framework version of the assembly which defines a specified Window class.
		/// </summary>
		/// <param name="window">Window class defined in the assembly</param>
		/// <returns>If succeeded, target framework version. If failed, null.</returns>
		public static Version GetFrameworkVersion(Window window)
		{
			if (window == null)
				return null;

			var assembly = Assembly.GetAssembly(window.GetType());

			return GetFrameworkVersion(assembly);
		}

		/// <summary>
		/// Gets target framework version of a specified assembly.
		/// </summary>
		/// <param name="assembly">Assembly to be checked</param>
		/// <returns>If succeeded, target framework version. If failed, null.</returns>
		public static Version GetFrameworkVersion(Assembly assembly)
		{
			var attributes = assembly?.GetCustomAttributes(typeof(TargetFrameworkAttribute), false);

			var attribute = attributes?.FirstOrDefault() as TargetFrameworkAttribute;
			if (attribute == null)
				return null;

			return new FrameworkName(attribute.FrameworkName).Version;
		}
	}
}