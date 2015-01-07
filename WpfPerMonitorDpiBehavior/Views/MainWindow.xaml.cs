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

namespace WpfPerMonitorDpiBehavior.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private EventHandler OnDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			OnDpiChanged = (_sender, _e) => SystemSounds.Exclamation.Play();
			DpiBehavior.WindowHandler.DpiChanged += OnDpiChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			DpiBehavior.WindowHandler.DpiChanged -= OnDpiChanged;
		}
	}
}