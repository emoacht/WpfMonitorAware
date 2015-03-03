using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MonitorAware.Models;

namespace WpfMonitorAwareProperty.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private EventHandler<DpiChangedEventArgs> _onDpiChanged;
		private EventHandler<ColorProfileChangedEventArgs> _onColorProfileChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_onDpiChanged = (_sender, _e) => SystemSounds.Exclamation.Play();
			MonitorProperty.WindowHandler.DpiChanged += _onDpiChanged;

			_onColorProfileChanged = (_sender, _e) => SystemSounds.Exclamation.Play();
			MonitorProperty.WindowHandler.ColorProfileChanged += _onColorProfileChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			MonitorProperty.WindowHandler.DpiChanged -= _onDpiChanged;

			MonitorProperty.WindowHandler.ColorProfileChanged -= _onColorProfileChanged;
		}
	}
}