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

		private EventHandler<MonitorAware.Models.DpiChangedEventArgs> _onDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_onDpiChanged = (_sender, _e) =>
			{
				Message = "MonitorAware Scaling Fired";
				SystemSounds.Exclamation.Play();
			};
			MonitorProperty.WindowHandler.DpiChanged += _onDpiChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			MonitorProperty.WindowHandler.DpiChanged -= _onDpiChanged;
		}

		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);

			Message = "Built-in Scaling Fired";
			SystemSounds.Hand.Play();
		}
	}
}