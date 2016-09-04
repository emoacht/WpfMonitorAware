using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfBuiltinScaling.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		#region Property

		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register(
				"Message",
				typeof(string),
				typeof(MainWindow),
				new PropertyMetadata(null));

		#endregion

		private EventHandler<MonitorAware.Models.DpiChangedEventArgs> _onMonitorDpiChanged;
		private bool _isMonitorDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_onMonitorDpiChanged = (_sender, _e) =>
			{
				Message = "MonitorAware Scaling Fired";
				SystemSounds.Exclamation.Play();

				try
				{
					_isMonitorDpiChanged = true;
					VisualTreeHelper.SetRootDpi(this, _e.NewDpi.ToDpiScale());
				}
				finally
				{
					_isMonitorDpiChanged = false;
				}
			};
			MonitorProperty.WindowHandler.DpiChanged += _onMonitorDpiChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			MonitorProperty.WindowHandler.DpiChanged -= _onMonitorDpiChanged;
		}

		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);

			if (!_isMonitorDpiChanged)
			{
				Message = "Built-in Scaling Fired";
				SystemSounds.Hand.Play();
			}

			Debug.WriteLine($"Window DpiChanged: {newDpi.PixelsPerDip}");
		}
	}
}