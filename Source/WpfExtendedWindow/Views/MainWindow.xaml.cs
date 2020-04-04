using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using WpfExtendedWindow.Models;
using WpfExtendedWindow.Views.Controls;

namespace WpfExtendedWindow.Views
{
	public partial class MainWindow : ExtendedWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private MouseButtonEventHandler _onContentMouseLeftDown;
		private EventHandler<DpiChangedEventArgs> _onDpiChanged;
		private EventHandler<ColorProfileChangedEventArgs> _onColorProfileChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_onContentMouseLeftDown = (_sender, _e) => _e.Handled = true;
			ContentRoot.MouseLeftButtonDown += _onContentMouseLeftDown;

			_onDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();
			WindowHandler.DpiChanged += _onDpiChanged;

			_onColorProfileChanged = (_sender, _e) =>
			{
				ColorProfilePath = _e.NewPath;
				SystemSounds.Exclamation.Play();
			};
			WindowHandler.ColorProfileChanged += _onColorProfileChanged;

			PrepareAnimation();

			Debug.WriteLine($"Notification Area DPI: {DpiChecker.GetNotificationAreaDpi()}");
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			ContentRoot.MouseLeftButtonDown -= _onContentMouseLeftDown;
			WindowHandler.DpiChanged -= _onDpiChanged;
			WindowHandler.ColorProfileChanged -= _onColorProfileChanged;

			base.OnClosing(e);
		}

		#region Theme

		public ExtendedTheme MainTheme
		{
			get { return (ExtendedTheme)GetValue(MainThemeProperty); }
			set { SetValue(MainThemeProperty, value); }
		}
		public static readonly DependencyProperty MainThemeProperty =
			DependencyProperty.Register(
				"MainTheme",
				typeof(ExtendedTheme),
				typeof(MainWindow),
				new FrameworkPropertyMetadata(
					ExtendedTheme.Default,
					(d, e) =>
					{
						var window = (MainWindow)d;

						window.Theme = (ExtendedTheme)e.NewValue;
						window.OnThemeChanged();
					}));

		private void OnThemeChanged()
		{
			switch (Theme)
			{
				case ExtendedTheme.Plain:
					SetContentBrushes(Brushes.Transparent, Brushes.Transparent, Brushes.Black);
					break;
				case ExtendedTheme.Light:
					SetContentBrushes(new SolidColorBrush(Color.FromArgb(63, 192, 192, 192)), Brushes.Transparent, Brushes.Black);
					break;
				case ExtendedTheme.Dark:
					SetContentBrushes(new SolidColorBrush(Color.FromArgb(63, 0, 0, 0)), Brushes.Transparent, Brushes.White);
					break;
				default:
					SetContentBrushes(new SolidColorBrush(Color.FromArgb(63, 255, 255, 255)), Brushes.White, Brushes.Black);
					break;
			}
		}

		private void SetContentBrushes(Brush contentBackground, Brush contentTextBackground, Brush contentTextForeground)
		{
			ContentRoot.Background = contentBackground;

			ContentGrid.Children.OfType<Label>()
				.ToList()
				.ForEach(x => x.Foreground = contentTextForeground);

			ContentGrid.Children.OfType<TextBox>()
				.ToList()
				.ForEach(x =>
				{
					x.Background = contentTextBackground;
					x.Foreground = contentTextForeground;
				});
		}

		#endregion

		#region Animation

		private Button _optionButton;
		private Canvas _backgroundCanvas;

		private void PrepareAnimation()
		{
			// Add the option button.
			_optionButton = new Button
			{
				Style = this.FindResource("OptionButtonStyle") as Style,
				Width = 60,
				Height = 20,
				Margin = new Thickness(0, 0, 8, 0),
				Content = "Rain",
			};
			_optionButton.Click += async (sender, e) => await PerformAnimation();
			this.TitleBarOptionGrid.Children.Add(_optionButton);

			// Insert the background canvas.
			_backgroundCanvas = new Canvas
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			Grid.SetZIndex(_backgroundCanvas, 1);
			this.ChromeGrid.Children.Add(_backgroundCanvas);

			Grid.SetZIndex(this.ChromeBorder, 2);
		}

		private bool _isAnimating;
		private const int _maxCount = 5;
		private DateTime _lastAddedTime;

		private readonly Dictionary<ExtendedTheme, Brush> themeBrushMap = new Dictionary<ExtendedTheme, Brush>()
		{
			{ExtendedTheme.Default, Brushes.White},
			{ExtendedTheme.Plain, Brushes.DarkViolet},
			{ExtendedTheme.Light, Brushes.DarkTurquoise},
			{ExtendedTheme.Dark, Brushes.DarkTurquoise},
		};

		private async Task PerformAnimation()
		{
			if (!_isAnimating)
			{
				if (0 < _backgroundCanvas.Children.Count)
					return;

				// Start animation.
				_isAnimating = true;
				_lastAddedTime = DateTime.MinValue;
			}
			else
			{
				// Stop animation.
				_isAnimating = false;
				return;
			}

			var random = new Random();

			do
			{
				if ((_backgroundCanvas.Children.Count < _maxCount) &&
					(_lastAddedTime.AddSeconds(1) < DateTime.Now))
				{
					_backgroundCanvas.Children.Add(new DropCircle());
					_lastAddedTime = DateTime.Now;
				}

				var circles = _backgroundCanvas.Children.Cast<DropCircle>()
					.Where(x => !x.IsAnimating)
					.ToList();

				circles.ForEach(circle =>
				{
					if (_isAnimating)
					{
						circle.Foreground = themeBrushMap[Theme];
						Canvas.SetLeft(circle, (double)random.Next(-80, Math.Max((int)_backgroundCanvas.ActualWidth - 120, 80)));
						Canvas.SetTop(circle, (double)random.Next(-80, Math.Max((int)_backgroundCanvas.ActualHeight - 120, 80)));
						circle.IsAnimating = true;
					}
					else
					{
						_backgroundCanvas.Children.Remove(circle);
					}
				});

				await Task.Delay(TimeSpan.FromSeconds(0.2));
			}
			while (0 < _backgroundCanvas.Children.Count);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);

			_isAnimating = false;
		}

		#endregion

		#region Image

		public string ColorProfilePath
		{
			get { return (string)GetValue(ColorProfilePathProperty); }
			set { SetValue(ColorProfilePathProperty, value); }
		}
		public static readonly DependencyProperty ColorProfilePathProperty =
			DependencyProperty.Register(
				"ColorProfilePath",
				typeof(string),
				typeof(MainWindow),
				new PropertyMetadata(
					null,
					async (d, e) => await ((MainWindow)d).ConvertImageAsync()));

		public BitmapSource ConvertedImage
		{
			get { return (BitmapSource)GetValue(ConvertedImageProperty); }
			set { SetValue(ConvertedImageProperty, value); }
		}
		public static readonly DependencyProperty ConvertedImageProperty =
			DependencyProperty.Register(
				"ConvertedImage",
				typeof(BitmapSource),
				typeof(MainWindow),
				new PropertyMetadata(null));

		private byte[] SourceData { get; set; }

		private async void OnDrop(object sender, DragEventArgs e)
		{
			var filePaths = ((DataObject)e.Data).GetFileDropList().Cast<string>();

			foreach (var filePath in filePaths)
			{
				if (!File.Exists(filePath))
					continue;

				try
				{
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					using (var ms = new MemoryStream())
					{
						await fs.CopyToAsync(ms);
						SourceData = ms.ToArray();
					}
				}
				catch
				{
					continue;
				}

				await ConvertImageAsync();
				return;
			}
		}

		private async Task ConvertImageAsync()
		{
			if (SourceData == null)
				return;

			ConvertedImage = await ImageConverter.ConvertImageAsync(SourceData, ColorProfilePath);
		}

		#endregion
	}
}