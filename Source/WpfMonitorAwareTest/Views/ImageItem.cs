using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMonitorAwareTest.Views
{
	internal class ImageItem
	{
		/// <summary>
		/// Resource path of associated image
		/// </summary>
		public string ImagePath { get; }

		/// <summary>
		/// Pixels per Device Independent Pixel (DIP) for which associated image is intended
		/// </summary>
		public double PixelsPerDip { get; }

		public ImageItem(string imagePath, double pixelsPerDip)
		{
			this.ImagePath = imagePath;
			this.PixelsPerDip = pixelsPerDip;
		}
	}
}