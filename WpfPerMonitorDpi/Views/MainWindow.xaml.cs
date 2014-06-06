using System;
using System.Media;
using System.Windows;

using PerMonitorDpi.Views;

namespace WpfPerMonitorDpi
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : PerMonitorDpiWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private EventHandler PlaySoundWhenDpiChanged;

		private void WindowRoot_Loaded(object sender, RoutedEventArgs e)
		{
			PlaySoundWhenDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();

			DpiChanged += PlaySoundWhenDpiChanged;
		}

		private void WindowRoot_Closed(object sender, EventArgs e)
		{
			DpiChanged -= PlaySoundWhenDpiChanged;
		}
	}
}
