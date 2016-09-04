using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfBuiltinScaling.Views
{
	internal class ImageHelper
	{
		public static ImageItem[] GetImageItems(string basePath)
		{
			return EnumerateImageItems(basePath).OrderBy(x => x.PixelsPerDip).ToArray();
		}

		private static IEnumerable<ImageItem> EnumerateImageItems(string basePath)
		{
			if (string.IsNullOrWhiteSpace(basePath))
				yield break;

			var basePattern = new Regex(@"(?<pack>.+;component/)(?<path>.+)\.scale-[0-9]{3}\.(?<extension>[a-zA-Z]{3})$");

			var baseMatch = basePattern.Match(basePath);
			if (!baseMatch.Success)
				yield break;

			var pack = baseMatch.Groups["pack"].Value;
			var path = baseMatch.Groups["path"].Value;
			var extension = baseMatch.Groups["extension"].Value;

			var resourcePattern = new Regex($@"({path}|{path.ToLowerInvariant()})\.scale-(?<percent>[0-9]{{3}})\.{extension}$");

			foreach (var resourcePath in _resourcePaths.Value)
			{
				var resourceMatch = resourcePattern.Match(resourcePath);
				if (!resourceMatch.Success)
					continue;

				var percent = resourceMatch.Groups["percent"].Value;
				var imagePath = $@"{pack}{path}.scale-{percent}.{extension}";
				var pixelsPerDip = double.Parse(percent) / 100D;

				yield return new ImageItem(imagePath, pixelsPerDip);
			}
		}

		private static Lazy<string[]> _resourcePaths = new Lazy<string[]>(() => GetResourcePaths(Assembly.GetExecutingAssembly()));

		private static string[] GetResourcePaths(Assembly assembly)
		{
			var resourceName = assembly.GetName().Name + ".g.resources";

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			using (var reader = new ResourceReader(stream))
			{
				return reader.Cast<DictionaryEntry>().Select(x => (string)x.Key).ToArray();
			}
		}
	}
}