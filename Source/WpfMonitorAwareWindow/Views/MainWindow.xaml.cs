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
using MonitorAware.Views;

namespace WpfMonitorAwareWindow.Views
{
	public partial class MainWindow : MonitorAwareWindow
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

			_onDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();
			WindowHandler.DpiChanged += _onDpiChanged;

			_onColorProfileChanged = (_sender, _e) => SystemSounds.Exclamation.Play();
			WindowHandler.ColorProfileChanged += _onColorProfileChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowHandler.DpiChanged -= _onDpiChanged;
			WindowHandler.ColorProfileChanged -= _onColorProfileChanged;
		}
	}
}