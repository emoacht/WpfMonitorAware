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

using PerMonitorDpi.Views;

namespace WpfPerMonitorDpiWindow.Views
{
	public partial class MainWindow : PerMonitorDpiWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private EventHandler _onDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_onDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();
			WindowHandler.DpiChanged += _onDpiChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		
			WindowHandler.DpiChanged -= _onDpiChanged;
		}
	}
}