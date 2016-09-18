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
		/// <summary>
		/// Gets image items from application resources.
		/// </summary>
		/// <param name="referencePath">File path of reference image</param>
		/// <returns>Array of image items</returns>
		public static ImageItem[] GetImageItems(string referencePath)
		{
			return EnumerateImageItems(referencePath).OrderBy(x => x.PixelsPerDip).ToArray();
		}

		/// <summary>
		/// Enumerates image items from application resources.
		/// </summary>
		/// <param name="referencePath">File bath of reference image</param>
		/// <returns>Sequence of image items</returns>
		/// <remarks>
		/// File name of images must be in the following format:
		/// XXX.scale-YYY.ZZZ (XXX is image name, YYY is percent of intended DIP, ZZZ is file extension)
		/// </remarks>
		private static IEnumerable<ImageItem> EnumerateImageItems(string referencePath)
		{
			if (string.IsNullOrWhiteSpace(referencePath))
				yield break;

			var basePattern = new Regex(@"(?<pack>.+;component/)(?<path>.+)\.scale-[0-9]{2,3}\.(?<extension>[a-zA-Z]{3})$");

			var baseMatch = basePattern.Match(referencePath);
			if (!baseMatch.Success)
				yield break;

			var pack = baseMatch.Groups["pack"].Value;
			var path = baseMatch.Groups["path"].Value;
			var extension = baseMatch.Groups["extension"].Value;

			var resourcePattern = new Regex($@"({path}|{path.ToLowerInvariant()})\.scale-(?<percent>[0-9]{{2,3}})\.{extension}$");

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