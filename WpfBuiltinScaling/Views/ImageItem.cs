using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBuiltinScaling.Views
{
	internal class ImageItem
	{
		public string ImagePath { get; }
		public double PixelsPerDip { get; }

		public ImageItem(string imagePath, double pixelsPerDip)
		{
			this.ImagePath = imagePath;
			this.PixelsPerDip = pixelsPerDip;
		}
	}
}