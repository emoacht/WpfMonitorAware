using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using PerMonitorDpi.Views;

using WpfExtendedWindow.Views.Controls;

namespace WpfExtendedWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ExtendedWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private MouseButtonEventHandler OnContentMouseLeftDown;
		private EventHandler OnDpiChanged;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			OnContentMouseLeftDown = (_sender, _e) => _e.Handled = true;
			ContentRoot.MouseLeftButtonDown += OnContentMouseLeftDown;

			OnDpiChanged = (_sender, _e) => SystemSounds.Hand.Play();
			WindowHandler.DpiChanged += OnDpiChanged;

			PrepareAnimation();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			ContentRoot.MouseLeftButtonDown -= OnContentMouseLeftDown;

			WindowHandler.DpiChanged -= OnDpiChanged;

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

		private Button optionButton;
		private Canvas backgroundCanvas;

		private void PrepareAnimation()
		{
			// Add the option button.
			optionButton = new Button()
			{
				Style = this.FindResource("OptionButtonStyle") as Style,
				Width = 60,
				Height = 20,
				Margin = new Thickness(0, 0, 8, 0),
				Content = "Rain",
			};
			optionButton.Click += async (sender, e) => await PerformAnimation();
			this.TitleBarOptionGrid.Children.Add(optionButton);

			// Insert the background canvas.
			backgroundCanvas = new Canvas()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			Grid.SetZIndex(backgroundCanvas, 1);
			this.ChromeGrid.Children.Add(backgroundCanvas);

			Grid.SetZIndex(this.ChromeBorder, 2);
		}

		private bool isAnimating = false;
		private const int maxNumber = 5;
		private DateTime lastAddedTime;

		private readonly Dictionary<ExtendedTheme, Brush> themeBrushMap = new Dictionary<ExtendedTheme, Brush>()
		{
			{ExtendedTheme.Default, Brushes.White},
			{ExtendedTheme.Plain, Brushes.DarkViolet},
			{ExtendedTheme.Light, Brushes.DarkTurquoise},
			{ExtendedTheme.Dark, Brushes.DarkTurquoise},
		};

		private async Task PerformAnimation()
		{
			if (!isAnimating)
			{
				if (0 < backgroundCanvas.Children.Count)
					return;

				// Start animation.
				isAnimating = true;
				lastAddedTime = DateTime.MinValue;
			}
			else
			{
				// Stop animation.
				isAnimating = false;
				return;
			}

			var randomizer = new Random();

			do
			{
				if ((backgroundCanvas.Children.Count < maxNumber) &&
					(lastAddedTime.AddSeconds(1) < DateTime.Now))
				{
					backgroundCanvas.Children.Add(new DropCircle());
					lastAddedTime = DateTime.Now;
				}

				var circles = backgroundCanvas.Children.Cast<DropCircle>()
					.Where(x => !x.IsAnimating)
					.ToArray();

				for (int i = circles.Length - 1; i >= 0; i--)
				{
					if (isAnimating)
					{
						circles[i].Foreground = themeBrushMap[Theme];
						Canvas.SetLeft(circles[i], (double)randomizer.Next(-80, Math.Max((int)backgroundCanvas.ActualWidth - 120, 80)));
						Canvas.SetTop(circles[i], (double)randomizer.Next(-80, Math.Max((int)backgroundCanvas.ActualHeight - 120, 80)));
						circles[i].IsAnimating = true;
					}
					else
					{
						backgroundCanvas.Children.Remove(circles[i]);
					}
				}

				await Task.Delay(TimeSpan.FromSeconds(0.2));

			} while (0 < backgroundCanvas.Children.Count);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);

			isAnimating = false;
		}

		#endregion
	}
}
