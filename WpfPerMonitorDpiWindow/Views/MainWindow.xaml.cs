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

		private EventHandler OnDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			OnDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();
			WindowHandler.DpiChanged += OnDpiChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		
			WindowHandler.DpiChanged -= OnDpiChanged;
		}
	}
}