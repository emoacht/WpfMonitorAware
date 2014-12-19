using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

using PerMonitorDpi.Helper;
using PerMonitorDpi.Models;
using PerMonitorDpi.Views.Controls;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Per-Monitor DPI aware and customizable chrome Window
	/// </summary>
	[TemplatePart(Name = "PART_ChromeExtraBorder", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ChromeGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_ChromeBorder", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ChromeContentGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarShadowGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarOptionGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarCaptionButtonPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_WindowContentBorder", Type = typeof(Border))]
	public class ExtendedWindow : Window
	{
		public ExtendedWindow()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedWindow), new FrameworkPropertyMetadata(typeof(ExtendedWindow)));

			Application.Current.ApplyResource("/PerMonitorDpi;component/Themes/Generic.xaml");
			this.Style = Application.Current.FindResource(typeof(ExtendedWindow)) as Style;

			RegisterCommands();
		}

		public ExtendedWindowHandler WindowHandler
		{
			get { return _windowHandler ?? (_windowHandler = new ExtendedWindowHandler()); }
		}
		private ExtendedWindowHandler _windowHandler;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			System.Diagnostics.Debug.WriteLine("OnSourceInitialized");

			WindowHandler.Initialize(this, ChromeGrid);

			WindowHandler.DpiChanged += OnDpiChanged;
			WindowHandler.WindowActivatedChanged += OnWindowActivatedChanged;
			WindowHandler.DwmColorizationColorChanged += OnDwmColorizationColorChanged;

			AdjustLayout();
			ManageCommands();
		}

		private void OnDpiChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}

		private void OnWindowActivatedChanged(object sender, bool e)
		{
			if (e)
				SetActivated();
			else
				SetDeactivated();

			// Force this Window to render immediately.
			this.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
		}

		private void OnDwmColorizationColorChanged(object sender, EventArgs e)
		{
			if (UsesDefaultChromeBackground)
				isDueCheckDefaultChromeBackground = true;
		}

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			AdjustLayout();
			ManageCommands();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			// Prevent default Window chrome from being shown at closing.
			this.WindowStyle = WindowStyle.None;

			base.OnClosing(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowHandler.Close();

			WindowHandler.DpiChanged -= OnDpiChanged;
			WindowHandler.WindowActivatedChanged -= OnWindowActivatedChanged;
			WindowHandler.DwmColorizationColorChanged -= OnDwmColorizationColorChanged;

			RemoveDragHandler();
		}


		#region Template Part

		private Border ChromeExtraBorder // For private use only
		{
			get { return _chromeExtraBorder ?? (_chromeExtraBorder = this.GetTemplateChild("PART_ChromeExtraBorder") as Border); }
		}
		private Border _chromeExtraBorder;

		protected Grid ChromeGrid
		{
			get { return _chromeGrid ?? (_chromeGrid = this.GetTemplateChild("PART_ChromeGrid") as Grid); }
		}
		private Grid _chromeGrid;

		protected Border ChromeBorder
		{
			get { return _chromeBorder ?? (_chromeBorder = this.GetTemplateChild("PART_ChromeBorder") as Border); }
		}
		private Border _chromeBorder;

		protected Grid ChromeContentGrid
		{
			get { return _chromeContentGrid ?? (_chromeContentGrid = this.GetTemplateChild("PART_ChromeContentGrid") as Grid); }
		}
		private Grid _chromeContentGrid;

		protected Grid TitleBarShadowGrid
		{
			get { return _titleBarShadowGrid ?? (_titleBarShadowGrid = this.GetTemplateChild("PART_TitleBarShadowGrid") as Grid); }
		}
		private Grid _titleBarShadowGrid;

		protected Grid TitleBarGrid
		{
			get { return _titleBarGrid ?? (_titleBarGrid = this.GetTemplateChild("PART_TitleBarGrid") as Grid); }
		}
		private Grid _titleBarGrid;

		protected Grid TitleBarOptionGrid
		{
			get { return _titleBarOptionGrid ?? (_titleBarOptionGrid = this.GetTemplateChild("PART_TitleBarOptionGrid") as Grid); }
		}
		private Grid _titleBarOptionGrid;

		private StackPanel TitleBarCaptionButtonPanel // For private use only
		{
			get { return _titleBarCaptionButtonPanel ?? (_titleBarCaptionButtonPanel = this.GetTemplateChild("PART_TitleBarCaptionButtonPanel") as StackPanel); }
		}
		private StackPanel _titleBarCaptionButtonPanel;

		protected Border WindowContentBorder
		{
			get { return _windowContentBorder ?? (_windowContentBorder = this.GetTemplateChild("PART_WindowContentBorder") as Border); }
		}
		private Border _windowContentBorder;

		#endregion


		#region Resource

		private static readonly Dictionary<ExtendedTheme, string> themeUriMap = new Dictionary<ExtendedTheme, string>() 
		{
			{ExtendedTheme.Default, String.Empty},
			{ExtendedTheme.Plain, @"/PerMonitorDpi;component/Views/Themes/PlainTheme.xaml"},
			{ExtendedTheme.Light, @"/PerMonitorDpi;component/Views/Themes/LightTheme.xaml"},
			{ExtendedTheme.Dark, @"/PerMonitorDpi;component/Views/Themes/DarkTheme.xaml"},
		};

		private const string defaultCaptionThemeUriString = @"/PerMonitorDpi;component/Views/Themes/DefaultCaptionTheme.xaml";

		/// <summary>
		/// Window theme from ExtendedTheme
		/// </summary>
		public ExtendedTheme Theme
		{
			get { return (ExtendedTheme)GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}
		public static readonly DependencyProperty ThemeProperty =
			DependencyProperty.Register(
				"Theme",
				typeof(ExtendedTheme),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					ExtendedTheme.Default,
					(d, e) => ((ExtendedWindow)d).ThemeUri = themeUriMap[(ExtendedTheme)e.NewValue]));

		/// <summary>
		/// Window theme Uri
		/// </summary>
		public string ThemeUri
		{
			get { return (string)GetValue(ThemeUriProperty); }
			set { SetValue(ThemeUriProperty, value); }
		}
		public static readonly DependencyProperty ThemeUriProperty =
			DependencyProperty.Register(
				"ThemeUri",
				typeof(string),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					String.Empty,
					(d, e) =>
					{
						var window = (ExtendedWindow)d;

						window.ApplyResource((string)e.NewValue, (string)e.OldValue);
						window.AdjustLayout();

						try
						{
							window.isChangingTheme = true;
							window.CheckBackground();
						}
						finally
						{
							window.isChangingTheme = false;
						}

						if (window.IsActive)
						{
							window.IsAboutActive = false; // This is required to make caption buttons' normal color conform to window chrome color.
							window.SetActivated();
						}
					}));

		private bool isChangingTheme = false;

		private void ReflectCaptionTheme(Brush background)
		{
			if (!isChangingTheme && !BlendsCaptionButtonVisualStyle)
				return;

			ApplyCaptionTheme(Application.Current.Resources, this.Resources, background);

			if (this.IsLoaded)
				RecreateCaptionButtons();
		}

		private void ApplyCaptionTheme(ResourceDictionary targetDictionary, ResourceDictionary sourceDictionary, Brush background)
		{
			var defaultDictionary = new ResourceDictionary() { Source = new Uri(defaultCaptionThemeUriString, UriKind.Relative) };

			foreach (var key in defaultDictionary.Keys)
			{
				Color? newColor = null;

				if (sourceDictionary.Contains(key))
				{
					var sourceValue = sourceDictionary[key];
					if (sourceValue is Color)
					{
						newColor = (Color)sourceValue;

						if (BlendsCaptionButtonVisualStyle)
						{
							var solid = background as SolidColorBrush;
							if (solid != null)
							{
								newColor = newColor.Value.ToBlended(solid.Color.ToOpaque(), (double)(255 - newColor.Value.A) / 255D * 100D);
							}
						}
					}
				}

				object newValue = (newColor.HasValue)
					? newColor.Value
					: defaultDictionary[key];

				if (!targetDictionary.Contains(key))
				{
					// Add key and value.
					targetDictionary.Add(key, newValue);
				}
				else
				{
					// Update value.
					targetDictionary[key] = newValue;
				}
			}
		}

		private void RecreateCaptionButtons()
		{
			if (TitleBarCaptionButtonPanel != null)
			{
				TitleBarCaptionButtonPanel.Children.Clear();

				TitleBarCaptionButtonPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.MinimizeStyle") as Style });
				TitleBarCaptionButtonPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.MaximizeStyle") as Style });
				TitleBarCaptionButtonPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.RestoreStyle") as Style });
				TitleBarCaptionButtonPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.CloseStyle") as Style });
			}
		}

		#endregion


		#region Command

		private void RegisterCommands()
		{
			this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeExecuted));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeExecuted, CanMaximizeExecute));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreExecuted, CanRestoreExecute));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseExecuted));
		}

		private void ManageCommands()
		{
			switch (this.ResizeMode)
			{
				case ResizeMode.NoResize:
					IsMinimizeVisible = false;
					IsMaximizeVisible = false;
					IsRestoreVisible = false;
					break;

				case ResizeMode.CanMinimize:
					IsMinimizeVisible = true;
					IsMaximizeVisible = true;
					IsRestoreVisible = false;
					break;

				default: // ResizeMode.CanResize, ResizeMode.CanResizeWithGrip
					IsMinimizeVisible = true;
					IsMaximizeVisible = (this.WindowState != WindowState.Maximized);
					IsRestoreVisible = !IsMaximizeVisible;
					break;
			}

			CommandManager.InvalidateRequerySuggested();
		}


		#region SystemCommands.MinimizeWindow

		private void MinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.MinimizeWindow(this);
		}

		public bool IsMinimizeVisible
		{
			get { return (bool)GetValue(IsMinimizeVisibleProperty); }
			set { SetValue(IsMinimizeVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsMinimizeVisibleProperty =
			DependencyProperty.Register(
				"IsMinimizeVisible",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(true));

		#endregion


		#region SystemCommands.MaximizeWindow

		private void MaximizeExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.WindowState != WindowState.Maximized)
				SystemCommands.MaximizeWindow(this);
		}

		private void CanMaximizeExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if ((this.ResizeMode == ResizeMode.NoResize) || (this.ResizeMode == ResizeMode.CanMinimize))
				e.CanExecute = false;
			else
				e.CanExecute = (this.WindowState != WindowState.Maximized);
		}

		public bool IsMaximizeVisible
		{
			get { return (bool)GetValue(IsMaximizeVisibleProperty); }
			set { SetValue(IsMaximizeVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsMaximizeVisibleProperty =
			DependencyProperty.Register(
				"IsMaximizeVisible",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(true));

		#endregion


		#region SystemCommands.RestoreWindow

		private void RestoreExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
				SystemCommands.RestoreWindow(this);
		}

		private void CanRestoreExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if ((this.ResizeMode == ResizeMode.NoResize) || (this.ResizeMode == ResizeMode.CanMinimize))
				e.CanExecute = false;
			else
				e.CanExecute = (this.WindowState == WindowState.Maximized);
		}

		public bool IsRestoreVisible
		{
			get { return (bool)GetValue(IsRestoreVisibleProperty); }
			set { SetValue(IsRestoreVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsRestoreVisibleProperty =
			DependencyProperty.Register(
				"IsRestoreVisible",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(false));

		#endregion


		#region SystemCommands.CloseWindow

		private void CloseExecuted(object target, ExecutedRoutedEventArgs e)
		{
			SystemCommands.CloseWindow(this);
		}

		#endregion

		#endregion


		#region Layout

		/// <summary>
		/// Whether to keep title and content margin even when a Window is maximized
		/// </summary>
		/// <remarks>False by OS's default.</remarks>
		public bool KeepsTitleContentMargin
		{
			get { return (bool)GetValue(KeepsTitleContentMarginProperty); }
			set { SetValue(KeepsTitleContentMarginProperty, value); }
		}
		public static readonly DependencyProperty KeepsTitleContentMarginProperty =
			DependencyProperty.Register(
				"KeepsTitleContentMargin",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Whether to blend colors for caption buttons' visual style with their normal color
		/// </summary>
		/// <remarks>This blending is for the case the colors are translucent.</remarks>
		public bool BlendsCaptionButtonVisualStyle
		{
			get { return (bool)GetValue(BlendsCaptionButtonVisualStyleProperty); }
			set { SetValue(BlendsCaptionButtonVisualStyleProperty, value); }
		}
		public static readonly DependencyProperty BlendsCaptionButtonVisualStyleProperty =
			DependencyProperty.Register(
				"BlendsCaptionButtonVisualStyle",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Whether to use OS's default chrome background
		/// </summary>
		/// <remarks>Default value must be false to execute initial check.</remarks>
		public bool UsesDefaultChromeBackground
		{
			get { return (bool)GetValue(UsesDefaultChromeBackgroundProperty); }
			set { SetValue(UsesDefaultChromeBackgroundProperty, value); }
		}
		public static readonly DependencyProperty UsesDefaultChromeBackgroundProperty =
			DependencyProperty.Register(
				"UsesDefaultChromeBackground",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					false,
					(d, e) => ((ExtendedWindow)d).isDueCheckDefaultChromeBackground = (bool)e.NewValue));

		/// <summary>
		/// OS's default chrome background Brush when a Window is activated (public readonly)
		/// </summary>
		/// <remarks>Default value (SystemColors.ActiveCaptionBrush) is merely for fall back.</remarks>
		public Brush DefaultChromeBackground
		{
			get { return (Brush)GetValue(DefaultChromeBackgroundProperty); }
			private set { SetValue(DefaultChromeBackgroundPropertyKey, value); }
		}
		private static readonly DependencyPropertyKey DefaultChromeBackgroundPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"DefaultChromeBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(SystemColors.ActiveCaptionBrush));
		public static readonly DependencyProperty DefaultChromeBackgroundProperty = DefaultChromeBackgroundPropertyKey.DependencyProperty;

		/// <summary>
		/// Chrome background Brush when a Window is activated
		/// </summary>
		public Brush ChromeBackground
		{
			get { return (Brush)GetValue(ChromeBackgroundProperty); }
			set { SetValue(ChromeBackgroundProperty, value); }
		}
		public static readonly DependencyProperty ChromeBackgroundProperty =
			DependencyProperty.Register(
				"ChromeBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((ExtendedWindow)d).isDueCheckBackground = true));

		/// <summary>
		/// Chrome foreground Brush when a Window is activated
		/// </summary>
		/// <remarks>Black by OS's default.</remarks>
		public Brush ChromeForeground
		{
			get { return (Brush)GetValue(ChromeForegroundProperty); }
			set { SetValue(ChromeForegroundProperty, value); }
		}
		public static readonly DependencyProperty ChromeForegroundProperty =
			DependencyProperty.Register(
				"ChromeForeground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(Brushes.Black));

		/// <summary>
		/// Chrome background Brush when a Window is deactivated
		/// </summary>
		/// <remarks>Default value (#FFebebeb) is for OS's default.</remarks>
		public Brush ChromeDeactivatedBackground
		{
			get { return (Brush)GetValue(ChromeDeactivatedBackgroundProperty); }
			set { SetValue(ChromeDeactivatedBackgroundProperty, value); }
		}
		private static readonly Brush _deactivatedBackground = new SolidColorBrush(Color.FromRgb(235, 235, 235));
		public static readonly DependencyProperty ChromeDeactivatedBackgroundProperty =
			DependencyProperty.Register(
				"ChromeDeactivatedBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(_deactivatedBackground));

		/// <summary>
		/// Chrome foreground Brush when a Window is deactivated
		/// </summary>
		/// <remarks>Black by OS's default.</remarks>
		public Brush ChromeDeactivatedForeground
		{
			get { return (Brush)GetValue(ChromeDeactivatedForegroundProperty); }
			set { SetValue(ChromeDeactivatedForegroundProperty, value); }
		}
		public static readonly DependencyProperty ChromeDeactivatedForegroundProperty =
			DependencyProperty.Register(
				"ChromeDeactivatedForeground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(Brushes.Black));

		/// <summary>
		/// Chrome border thickness (chrome outer border)
		/// </summary>
		/// <remarks>1 by OS's default.</remarks>
		public Thickness ChromeBorderThickness
		{
			get { return (Thickness)GetValue(ChromeBorderThicknessProperty); }
			set { SetValue(ChromeBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty ChromeBorderThicknessProperty =
			DependencyProperty.Register(
				"ChromeBorderThickness",
				typeof(Thickness),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(new Thickness(1D)));

		/// <summary>
		/// Chrome border Brush
		/// </summary>
		/// <remarks>Default value (#3C000000) is for reproducing OS's default.</remarks>
		public Brush ChromeBorderBrush
		{
			get { return (Brush)GetValue(ChromeBorderBrushProperty); }
			set { SetValue(ChromeBorderBrushProperty, value); }
		}
		private static readonly Brush _chromeBorderBrush = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0));
		public static readonly DependencyProperty ChromeBorderBrushProperty =
			DependencyProperty.Register(
				"ChromeBorderBrush",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(_chromeBorderBrush));

		/// <summary>
		/// Title bar background Brush
		/// </summary>
		public Brush TitleBarBackground
		{
			get { return (Brush)GetValue(TitleBarBackgroundProperty); }
			set { SetValue(TitleBarBackgroundProperty, value); }
		}
		public static readonly DependencyProperty TitleBarBackgroundProperty =
			DependencyProperty.Register(
				"TitleBarBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((ExtendedWindow)d).isDueCheckBackground = true));

		/// <summary>
		/// Title bar height when a Window is other than maximized
		/// </summary>
		/// <remarks>This height includes neither chrome outer border thickness nor chrome inner border thickness. 
		/// By OS's default, title bar height (when a window is maximized) will be sum of 
		/// SystemParameters.WindowResizeBorderThickness.Top and SystemParameters.WindowNonClientFrameThickness.Top
		/// which includes chrome border thickness.</remarks>
		public double TitleBarNormalHeight
		{
			get { return (double)GetValue(TitleBarNormalHeightProperty); }
			set { SetValue(TitleBarNormalHeightProperty, value); }
		}
		public static readonly DependencyProperty TitleBarNormalHeightProperty =
			DependencyProperty.Register(
				"TitleBarNormalHeight",
				typeof(double),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(SystemParameters.CaptionHeight));

		/// <summary>
		/// Title bar height when a Window is maximized
		/// </summary>
		public double TitleBarMaximizedHeight
		{
			get { return (double)GetValue(TitleBarMaximizedHeightProperty); }
			set { SetValue(TitleBarMaximizedHeightProperty, value); }
		}
		public static readonly DependencyProperty TitleBarMaximizedHeightProperty =
			DependencyProperty.Register(
				"TitleBarMaximizedHeight",
				typeof(double),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(SystemParameters.CaptionHeight));

		/// <summary>
		/// Title bar padding at the left side
		/// </summary>
		public double TitleBarPaddingLeft
		{
			get { return (double)GetValue(TitleBarPaddingLeftProperty); }
			set { SetValue(TitleBarPaddingLeftProperty, value); }
		}
		public static readonly DependencyProperty TitleBarPaddingLeftProperty =
			DependencyProperty.Register(
				"TitleBarPaddingLeft",
				typeof(double),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(0D));

		/// <summary>
		/// Title bar padding at the right side
		/// </summary>
		public double TitleBarPaddingRight
		{
			get { return (double)GetValue(TitleBarPaddingRightProperty); }
			set { SetValue(TitleBarPaddingRightProperty, value); }
		}
		public static readonly DependencyProperty TitleBarPaddingRightProperty =
			DependencyProperty.Register(
				"TitleBarPaddingRight",
				typeof(double),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(0D));

		/// <summary>
		/// Size of icon (Window.Icon) in title bar
		/// </summary>
		public Size IconSize
		{
			get { return (Size)GetValue(IconSizeProperty); }
			set { SetValue(IconSizeProperty, value); }
		}
		public static readonly DependencyProperty IconSizeProperty =
			DependencyProperty.Register(
				"IconSize",
				typeof(Size),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(new Size(16, 16)));

		/// <summary>
		/// Alignment of title (Window.Title) in title bar
		/// </summary>
		public HorizontalAlignment TitleAlignment
		{
			get { return (HorizontalAlignment)GetValue(TitleAlignmentProperty); }
			set { SetValue(TitleAlignmentProperty, value); }
		}
		public static readonly DependencyProperty TitleAlignmentProperty =
			DependencyProperty.Register(
				"TitleAlignment",
				typeof(HorizontalAlignment),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(HorizontalAlignment.Left));

		/// <summary>
		/// Font size of title (Window.Title) in title bar
		/// </summary>
		/// <remarks>Default value (14) is for OS's default.</remarks>
		public double TitleFontSize
		{
			get { return (double)GetValue(TitleFontSizeProperty); }
			set { SetValue(TitleFontSizeProperty, value); }
		}
		public static readonly DependencyProperty TitleFontSizeProperty =
			DependencyProperty.Register(
				"TitleFontSize",
				typeof(double),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(14D));

		/// <summary>
		/// Content (Window.Content) margin 
		/// </summary>
		/// <remarks>Default value (6,0,6,6) is for reproducing OS's default.</remarks>
		public Thickness ContentMargin
		{
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public static readonly DependencyProperty ContentMarginProperty =
			DependencyProperty.Register(
				"ContentMargin",
				typeof(Thickness),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(new Thickness(6D, 0D, 6D, 6D)));

		/// <summary>
		/// Content border thickness (chrome inner border)
		/// </summary>
		/// <remarks>1 by OS's default.</remarks>
		public Thickness ContentBorderThickness
		{
			get { return (Thickness)GetValue(ContentBorderThicknessProperty); }
			set { SetValue(ContentBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty ContentBorderThicknessProperty =
			DependencyProperty.Register(
				"ContentBorderThickness",
				typeof(Thickness),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(new Thickness(1D)));

		/// <summary>
		/// Content border Brush
		/// </summary>
		/// <remarks>Default value is the same as that of ChromeBorderBrush.</remarks>
		public Brush ContentBorderBrush
		{
			get { return (Brush)GetValue(ContentBorderBrushProperty); }
			set { SetValue(ContentBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty ContentBorderBrushProperty =
			DependencyProperty.Register(
				"ContentBorderBrush",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(_chromeBorderBrush));

		#endregion


		#region Method

		/// <summary>
		/// Adjust layout of Window parts.
		/// </summary>
		protected void AdjustLayout()
		{
			var factorFromDefaultX = (double)WindowHandler.WindowDpi.X / Dpi.Default.X;
			var factorFromDefaultY = (double)WindowHandler.WindowDpi.Y / Dpi.Default.Y;
			var factorFromSystemX = (double)WindowHandler.WindowDpi.X / WindowHandler.SystemDpi.X;
			var factorFromSystemY = (double)WindowHandler.WindowDpi.Y / WindowHandler.SystemDpi.Y;

			var captionHeight = 0D;

			// Manage chrome border.
			if (this.WindowState == WindowState.Maximized)
			{
				if (ChromeExtraBorder != null)
				{
					ChromeExtraBorder.BorderThickness = new Thickness(
						SystemParameters.WindowResizeBorderThickness.Left + SystemParameters.WindowNonClientFrameThickness.Left,
						SystemParameters.WindowResizeBorderThickness.Left + SystemParameters.WindowNonClientFrameThickness.Left, // Use Left values.
						SystemParameters.WindowResizeBorderThickness.Right + SystemParameters.WindowNonClientFrameThickness.Right,
						SystemParameters.WindowResizeBorderThickness.Bottom + SystemParameters.WindowNonClientFrameThickness.Bottom);

					captionHeight += ChromeExtraBorder.BorderThickness.Top;
				}

				if (ChromeBorder != null)
				{
					ChromeBorder.BorderThickness = new Thickness(0D);
				}
			}
			else
			{
				if (ChromeExtraBorder != null)
				{
					ChromeExtraBorder.BorderThickness = new Thickness(0D);

					captionHeight += ChromeBorderThickness.Top;
				}

				if (ChromeBorder != null)
				{
					ChromeBorder.BorderThickness = new Thickness(
						ChromeBorderThickness.Left / factorFromDefaultX,
						ChromeBorderThickness.Top / factorFromDefaultY,
						ChromeBorderThickness.Right / factorFromDefaultX,
						ChromeBorderThickness.Bottom / factorFromDefaultY);
				}
			}

			// Manage title bar and content border.						
			if ((this.WindowState == WindowState.Maximized) && !KeepsTitleContentMargin)
			{
				if (TitleBarGrid != null)
				{
					TitleBarGrid.Height = TitleBarMaximizedHeight.ToRounded(factorFromDefaultY);
					TitleBarPaddingLeft = 0D;
				}

				if (WindowContentBorder != null)
				{
					WindowContentBorder.Margin = new Thickness(0D);
					WindowContentBorder.BorderThickness = new Thickness(
						0D,
						ContentBorderThickness.Top / factorFromDefaultY,
						0D,
						0D);
				}
			}
			else
			{
				if (TitleBarGrid != null)
				{
					TitleBarGrid.Height = TitleBarNormalHeight.ToRounded(factorFromDefaultY);
					TitleBarPaddingLeft = Math.Ceiling(Math.Max(TitleBarGrid.Height - IconSize.Height, 0D) / 2).ToRounded(factorFromDefaultX);
				}

				if (WindowContentBorder != null)
				{
					WindowContentBorder.Margin = new Thickness(
						ContentMargin.Left.ToRounded(factorFromDefaultX),
						ContentMargin.Top.ToRounded(factorFromDefaultY),
						ContentMargin.Right.ToRounded(factorFromDefaultX),
						ContentMargin.Bottom.ToRounded(factorFromDefaultY));
					WindowContentBorder.BorderThickness = new Thickness(
						ContentBorderThickness.Left / factorFromDefaultX,
						ContentBorderThickness.Top / factorFromDefaultY,
						ContentBorderThickness.Right / factorFromDefaultX,
						ContentBorderThickness.Bottom / factorFromDefaultY);
				}
			}

			// Set caption height for WindowChrome.CaptionHeight.
			if (TitleBarGrid != null)
				captionHeight += Math.Round((TitleBarGrid.Height + ContentBorderThickness.Top) * factorFromSystemY);

			var windowChrome = WindowChrome.GetWindowChrome(this);
			if (windowChrome != null)
				windowChrome.CaptionHeight = Math.Max(captionHeight - SystemParameters.WindowResizeBorderThickness.Top, 0D);
		}

		/// <summary>
		/// Whether checking background is due.
		/// </summary>
		/// <remarks>Default value must be true to execute initial check.</remarks>
		private bool isDueCheckBackground = true;

		/// <summary>
		/// Whether checking default chrome background is due.
		/// </summary>
		private bool isDueCheckDefaultChromeBackground = false;

		/// <summary>
		/// Chrome background color to be actually used
		/// </summary>
		private Brush chromeBackgroundActual = Brushes.Transparent;

		/// <summary>
		/// Caption button background Brush (internal)
		/// </summary>
		/// <remarks>For binding only between code behind.</remarks>
		internal Brush CaptionButtonBackground
		{
			get { return (Brush)GetValue(CaptionButtonBackgroundProperty); }
			set { SetValue(CaptionButtonBackgroundProperty, value); }
		}
		internal static readonly DependencyProperty CaptionButtonBackgroundProperty =
			DependencyProperty.Register(
				"CaptionButtonBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((ExtendedWindow)d).ReflectCaptionTheme((Brush)e.NewValue)));

		/// <summary>
		/// Whether a Window is about to be activated (internal)
		/// </summary>
		/// <remarks>This property will be changed when the Window is about to be activated or deactivated.
		/// For binding only between code behind.</remarks>
		internal bool IsAboutActive
		{
			get { return (bool)GetValue(IsAboutActiveProperty); }
			set { SetValue(IsAboutActiveProperty, value); }
		}
		internal static readonly DependencyProperty IsAboutActiveProperty =
			DependencyProperty.Register(
				"IsAboutActive",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(false));

		private void SetActivated()
		{
			CheckBackground();

			IsAboutActive = true;

			this.Background = chromeBackgroundActual;
			this.Foreground = ChromeForeground;

			if (TitleBarShadowGrid != null)
				TitleBarShadowGrid.Background = TitleBarBackground;
		}

		private void SetDeactivated()
		{
			IsAboutActive = false;

			this.Background = ChromeDeactivatedBackground;
			this.Foreground = ChromeDeactivatedForeground;

			if (TitleBarShadowGrid != null)
				TitleBarShadowGrid.Background = ChromeDeactivatedBackground;
		}

		private void CheckBackground()
		{
			if (!isDueCheckBackground && !isDueCheckDefaultChromeBackground)
				return;

			if (isDueCheckDefaultChromeBackground)
			{
				var color = WindowChromeColor.GetChromeColor();
				if (color.HasValue)
					DefaultChromeBackground = new SolidColorBrush(color.Value);
			}

			chromeBackgroundActual = (UsesDefaultChromeBackground || ChromeBackground.IsTransparent())
				? DefaultChromeBackground
				: ChromeBackground;

			CaptionButtonBackground = TitleBarBackground.IsTransparent()
				? chromeBackgroundActual
				: TitleBarBackground;

			AddDragHandler();

			isDueCheckBackground = false;
			isDueCheckDefaultChromeBackground = false;
		}

		private void AddDragHandler()
		{
			RemoveDragHandler();

			if (!TitleBarBackground.IsTransparent() && (TitleBarGrid != null))
				TitleBarGrid.MouseLeftButtonDown += OnDrag;
			else
				this.MouseLeftButtonDown += OnDrag;
		}

		private void RemoveDragHandler()
		{
			if (TitleBarGrid != null)
				TitleBarGrid.MouseLeftButtonDown -= OnDrag;

			this.MouseLeftButtonDown -= OnDrag;
		}

		#endregion
	}
}