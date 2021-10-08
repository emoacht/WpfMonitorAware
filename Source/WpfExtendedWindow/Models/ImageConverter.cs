using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfExtendedWindow.Models
{
	public class ImageConverter
	{
		/// <summary>
		/// Converts byte array of an image to BitmapSource reflecting color profiles.
		/// </summary>
		/// <param name="sourceData">Byte array of source image</param>
		/// <param name="colorProfilePath">Color profile file path used by a monitor</param>
		/// <returns>BitmapSource</returns>
		public static async Task<BitmapSource> ConvertImageAsync(byte[] sourceData, string colorProfilePath)
		{
			using (var ms = new MemoryStream())
			{
				await ms.WriteAsync(sourceData, 0, sourceData.Length).ConfigureAwait(false);
				ms.Seek(0, SeekOrigin.Begin);

				var frame = BitmapFrame.Create(
					ms,
					BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.OnLoad);

				var ccb = new ColorConvertedBitmap();
				ccb.BeginInit();
				ccb.Source = frame;

				ccb.SourceColorContext = (frame.ColorContexts?.Any() == true)
					? frame.ColorContexts.First()
					: new ColorContext(PixelFormats.Bgra32); // Fallback color profile

				ccb.DestinationColorContext = !string.IsNullOrEmpty(colorProfilePath)
					? new ColorContext(new Uri(colorProfilePath))
					: new ColorContext(PixelFormats.Bgra32); // Fallback color profile

				ccb.DestinationFormat = PixelFormats.Bgra32;
				ccb.EndInit();
				ccb.Freeze();

				return ccb;
			}
		}
	}
}