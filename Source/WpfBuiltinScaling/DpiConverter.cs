using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfBuiltinScaling
{
	public static class DpiConverter
	{
		public static DpiScale ToDpiScale(this MonitorAware.Models.Dpi source) =>
			new DpiScale(source.X / 96D, source.Y / 96D);

		public static MonitorAware.Models.Dpi ToDpi(this DpiScale source) =>
			new MonitorAware.Models.Dpi((uint)source.PixelsPerInchX, (uint)source.PixelsPerInchY);
	}
}