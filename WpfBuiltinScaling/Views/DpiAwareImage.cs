using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfBuiltinScaling.Views
{
	public class DpiAwareImage : Image
	{
		public DpiAwareImage() : base()
		{
		}

		private ImageItem[] _imageItems;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var basePath = ((BitmapFrame)this.Source).Decoder.ToString(); // Actually, this.Source.ToString() will return the same.

			_imageItems = ImageHelper.GetImageItems(basePath);

			var initialDpi = VisualTreeHelper.GetDpi(this);
			SetSource(initialDpi.PixelsPerDip);
		}

		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);

			Debug.WriteLine($"Image DpiChanged: {newDpi.PixelsPerDip}");

			SetSource(newDpi.PixelsPerDip);
		}

		private void SetSource(double pixelsPerDip)
		{
			if (_imageItems?.Any() != true)
				return;

			var item = _imageItems.FirstOrDefault(x => pixelsPerDip <= x.PixelsPerDip);
			if (item == null)
				item = _imageItems.Last(); // The item of the highest PixelsPerDip will be used as fallback.

			Uri uri;
			if (!Uri.TryCreate(item.ImagePath, UriKind.RelativeOrAbsolute, out uri))
				return;

			this.Source = new BitmapImage(uri);
		}
	}
}