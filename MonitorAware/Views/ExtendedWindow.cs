using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

using MonitorAware.Helper;
using MonitorAware.Models;
using MonitorAware.Views.Controls;

namespace MonitorAware.Views
{
	/// <summary>
	/// Per-Monitor DPI aware and customizable chrome Window
	/// </summary>
	[TemplatePart(Name = "PART_ChromeOutmostBorder", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ChromeGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_ChromeBorder", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ChromeContentGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarBackGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarIcon", Type = typeof(Image))]
	[TemplatePart(Name = "PART_TitleBarOptionGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarCaptionButtonsPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_WindowContentBorder", Type = typeof(Border))]
	public class ExtendedWindow : Window
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExtendedWindow()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedWindow), new FrameworkPropertyMetadata(typeof(ExtendedWindow)));

			Application.Current.ApplyResource("/MonitorAware;component/Themes/Generic.xaml");
			this.Style = Application.Current.FindResource(typeof(ExtendedWindow)) as Style;

			RegisterCommands();
		}

		/// <summary>
		/// Handler for <see cref="ExtendedWindow"/>
		/// </summary>
		public ExtendedWindowHandler WindowHandler { get; } = new ExtendedWindowHandler();

		/// <summary>
		/// OnApplyTemplate
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ChromeOutmostBorder = this.GetTemplateChild("PART_ChromeOutmostBorder") as Border;
			ChromeGrid = this.GetTemplateChild("PART_ChromeGrid") as Grid;
			ChromeBorder = this.GetTemplateChild("PART_ChromeBorder") as Border;
			ChromeContentGrid = this.GetTemplateChild("PART_ChromeContentGrid") as Grid;
			TitleBarBackGrid = this.GetTemplateChild("PART_TitleBarBackGrid") as Grid;
			TitleBarGrid = this.GetTemplateChild("PART_TitleBarGrid") as Grid;

			var icon = this.GetTemplateChild("PART_TitleBarIcon") as Image;
			if (icon?.Visibility == Visibility.Visible)
				TitleBarIcon = icon;

			TitleBarOptionGrid = this.GetTemplateChild("PART_TitleBarOptionGrid") as Grid;
			TitleBarCaptionButtonsPanel = this.GetTemplateChild("PART_TitleBarCaptionButtonsPanel") as StackPanel;
			WindowContentBorder = this.GetTemplateChild("PART_WindowContentBorder") as Border;
		}

		/// <summary>
		/// OnSourceInitialized
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowHandler.Initialize(this, ChromeGrid);

			WindowHandler.DpiChanged += OnDpiChanged;
			WindowHandler.WindowActivatedChanged += OnWindowActivatedChanged;
			WindowHandler.DwmColorizationColorChanged += OnDwmColorizationColorChanged;

			if (TitleBarIcon != null)
				TitleBarIcon.MouseDown += OnTitleBarIconMouseDown;

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
				_isDueCheckDefaultChromeBackground = true;
		}

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.DragMove();
		}

		private void OnTitleBarIconMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			ManageTitleBarIconClick(e);
		}

		/// <summary>
		/// OnStateChanged
		/// </summary>
		/// <param name="e"></param>
		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			AdjustLayout();
			ManageCommands();
		}

		/// <summary>
		/// OnClosing
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing(CancelEventArgs e)
		{
			// Prevent default Window chrome from being shown at closing.
			this.WindowStyle = WindowStyle.None;

			base.OnClosing(e);
		}

		/// <summary>
		/// OnClosed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowHandler.Close();

			WindowHandler.DpiChanged -= OnDpiChanged;
			WindowHandler.WindowActivatedChanged -= OnWindowActivatedChanged;
			WindowHandler.DwmColorizationColorChanged -= OnDwmColorizationColorChanged;

			if (TitleBarIcon != null)
				TitleBarIcon.MouseDown -= OnTitleBarIconMouseDown;

			RemoveDragHandler();
		}

		#region Template Part

		/// <summary>
		/// Window chrome outmost border (private use only)
		/// </summary>
		private Border ChromeOutmostBorder { get; set; }

		/// <summary>
		/// Window chrome grid
		/// </summary>
		protected Grid ChromeGrid { get; private set; }

		/// <summary>
		/// Window chrome border
		/// </summary>
		protected Border ChromeBorder { get; private set; }

		/// <summary>
		/// Window chrome content grid
		/// </summary>
		protected Grid ChromeContentGrid { get; private set; }

		/// <summary>
		/// Title bar back grid
		/// </summary>
		protected Grid TitleBarBackGrid { get; private set; }

		/// <summary>
		/// Title bar grid
		/// </summary>
		protected Grid TitleBarGrid { get; private set; }

		/// <summary>
		/// Title bar icon (private use only)
		/// </summary>
		private Image TitleBarIcon { get; set; }

		/// <summary>
		/// Title bar option grid
		/// </summary>
		protected Grid TitleBarOptionGrid { get; private set; }

		/// <summary>
		/// Title bar caption buttons' panel (private use only)
		/// </summary>
		private StackPanel TitleBarCaptionButtonsPanel { get; set; }

		/// <summary>
		/// Window content border
		/// </summary>
		protected Border WindowContentBorder { get; private set; }

		#endregion

		#region Resource

		private static readonly Dictionary<ExtendedTheme, string> _themeUriMap = new Dictionary<ExtendedTheme, string>()
		{
			{ExtendedTheme.Default, string.Empty},
			{ExtendedTheme.Plain, @"/MonitorAware;component/Views/Themes/PlainTheme.xaml"},
			{ExtendedTheme.Light, @"/MonitorAware;component/Views/Themes/LightTheme.xaml"},
			{ExtendedTheme.Dark, @"/MonitorAware;component/Views/Themes/DarkTheme.xaml"},
		};

		private const string _defaultCaptionThemeUriString = @"/MonitorAware;component/Views/Themes/DefaultCaptionTheme.xaml";

		/// <summary>
		/// Window theme out of ExtendedTheme
		/// </summary>
		public ExtendedTheme Theme
		{
			get { return (ExtendedTheme)GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="Theme"/>
		/// </summary>
		public static readonly DependencyProperty ThemeProperty =
			DependencyProperty.Register(
				"Theme",
				typeof(ExtendedTheme),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					ExtendedTheme.Default,
					(d, e) => ((ExtendedWindow)d).ThemeUri = _themeUriMap[(ExtendedTheme)e.NewValue]));

		/// <summary>
		/// Window theme Uri
		/// </summary>
		public string ThemeUri
		{
			get { return (string)GetValue(ThemeUriProperty); }
			set { SetValue(ThemeUriProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="ThemeUri"/>
		/// </summary>
		public static readonly DependencyProperty ThemeUriProperty =
			DependencyProperty.Register(
				"ThemeUri",
				typeof(string),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					string.Empty,
					(d, e) =>
					{
						var window = (ExtendedWindow)d;

						window.ApplyResource((string)e.NewValue, (string)e.OldValue);
						window.AdjustLayout();

						try
						{
							window._isChangingTheme = true;
							window.CheckBackground();
						}
						finally
						{
							window._isChangingTheme = false;
						}

						if (window.IsActive)
						{
							window.IsAboutActive = false; // This is required to make caption buttons' normal color conform to window chrome color.
							window.SetActivated();
						}
					}));

		private bool _isChangingTheme;

		private void ReflectCaptionTheme(Brush background)
		{
			if (!_isChangingTheme && !BlendsCaptionButtonVisualStyle)
				return;

			ApplyCaptionTheme(Application.Current.Resources, this.Resources, background);

			if (this.IsLoaded)
				RecreateCaptionButtons();
		}

		private void ApplyCaptionTheme(ResourceDictionary targetDictionary, ResourceDictionary sourceDictionary, Brush background)
		{
			var defaultDictionary = new ResourceDictionary { Source = new Uri(_defaultCaptionThemeUriString, UriKind.Relative) };

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

				object newValue = newColor ?? defaultDictionary[key];

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
			if (TitleBarCaptionButtonsPanel != null)
			{
				TitleBarCaptionButtonsPanel.Children.Clear();

				TitleBarCaptionButtonsPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.MinimizeStyle") as Style });
				TitleBarCaptionButtonsPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.MaximizeStyle") as Style });
				TitleBarCaptionButtonsPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.RestoreStyle") as Style });
				TitleBarCaptionButtonsPanel.Children.Add(new ExtendedCaptionButton() { Style = Application.Current.FindResource("ExtendedCaptionButton.CloseStyle") as Style });
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

		/// <summary>
		/// Whether minimize button is visible
		/// </summary>
		public bool IsMinimizeVisible
		{
			get { return (bool)GetValue(IsMinimizeVisibleProperty); }
			set { SetValue(IsMinimizeVisibleProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="IsMinimizeVisible"/>
		/// </summary>
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

		/// <summary>
		/// Whether maximize button is visible
		/// </summary>
		public bool IsMaximizeVisible
		{
			get { return (bool)GetValue(IsMaximizeVisibleProperty); }
			set { SetValue(IsMaximizeVisibleProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="IsMaximizeVisible"/>
		/// </summary>
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

		/// <summary>
		/// Whether restore button is visible
		/// </summary>
		public bool IsRestoreVisible
		{
			get { return (bool)GetValue(IsRestoreVisibleProperty); }
			set { SetValue(IsRestoreVisibleProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="IsRestoreVisible"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="KeepsTitleContentMargin"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="BlendsCaptionButtonVisualStyle"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="UsesDefaultChromeBackground"/>
		/// </summary>
		public static readonly DependencyProperty UsesDefaultChromeBackgroundProperty =
			DependencyProperty.Register(
				"UsesDefaultChromeBackground",
				typeof(bool),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					false,
					(d, e) => ((ExtendedWindow)d)._isDueCheckDefaultChromeBackground = (bool)e.NewValue));

		/// <summary>
		/// OS's default chrome background Brush when a Window is activated (public readonly)
		/// </summary>
		/// <remarks>Default value (SystemColors.ActiveCaptionBrush) is merely for fall back.</remarks>
		public Brush DefaultChromeBackground
		{
			get { return (Brush)GetValue(DefaultChromeBackgroundProperty); }
			private set { SetValue(DefaultChromeBackgroundPropertyKey, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="DefaultChromeBackground"/>
		/// </summary>
		private static readonly DependencyPropertyKey DefaultChromeBackgroundPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"DefaultChromeBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(SystemColors.ActiveCaptionBrush));
		/// <summary>
		/// Dependency property for <see cref="DefaultChromeBackground"/>
		/// </summary>
		public static readonly DependencyProperty DefaultChromeBackgroundProperty = DefaultChromeBackgroundPropertyKey.DependencyProperty;

		/// <summary>
		/// Chrome background Brush when a Window is activated
		/// </summary>
		public Brush ChromeBackground
		{
			get { return (Brush)GetValue(ChromeBackgroundProperty); }
			set { SetValue(ChromeBackgroundProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="ChromeBackground"/>
		/// </summary>
		public static readonly DependencyProperty ChromeBackgroundProperty =
			DependencyProperty.Register(
				"ChromeBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((ExtendedWindow)d)._isDueCheckBackground = true));

		/// <summary>
		/// Chrome foreground Brush when a Window is activated
		/// </summary>
		/// <remarks>Black by OS's default.</remarks>
		public Brush ChromeForeground
		{
			get { return (Brush)GetValue(ChromeForegroundProperty); }
			set { SetValue(ChromeForegroundProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="ChromeForeground"/>
		/// </summary>
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
		/// <summary>
		/// Default value of <see cref="ChromeDeactivatedBackground"/>
		/// </summary>
		/// <remarks>This value seems to have to be defined before dependency property.</remarks>
		private static readonly Brush _deactivatedBackground = new SolidColorBrush(Color.FromRgb(235, 235, 235));
		/// <summary>
		/// Dependency property for <see cref="ChromeDeactivatedBackground"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="ChromeDeactivatedForeground"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="ChromeBorderThickness"/>
		/// </summary>
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
		/// <summary>
		/// Default value of <see cref="ChromeBorderBrush"/>
		/// </summary>
		/// <remarks>This value seems to have to be defined before dependency property.</remarks>
		private static readonly Brush _chromeBorderBrush = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0));
		/// <summary>
		/// Dependency property for <see cref="ChromeBorderBrush"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleBarBackground"/>
		/// </summary>
		public static readonly DependencyProperty TitleBarBackgroundProperty =
			DependencyProperty.Register(
				"TitleBarBackground",
				typeof(Brush),
				typeof(ExtendedWindow),
				new FrameworkPropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((ExtendedWindow)d)._isDueCheckBackground = true));

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
		/// <summary>
		/// Dependency property for <see cref="TitleBarNormalHeight"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleBarMaximizedHeight"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleBarPaddingLeft"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleBarPaddingRight"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="IconSize"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleAlignment"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="TitleFontSize"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="ContentMargin"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="ContentBorderThickness"/>
		/// </summary>
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
		/// <summary>
		/// Dependency property for <see cref="ContentBorderBrush"/>
		/// </summary>
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
				if (ChromeOutmostBorder != null)
				{
					ChromeOutmostBorder.BorderThickness = new Thickness(
						SystemParameters.WindowResizeBorderThickness.Left + SystemParameters.WindowNonClientFrameThickness.Left,
						SystemParameters.WindowResizeBorderThickness.Left + SystemParameters.WindowNonClientFrameThickness.Left, // Use Left values.
						SystemParameters.WindowResizeBorderThickness.Right + SystemParameters.WindowNonClientFrameThickness.Right,
						SystemParameters.WindowResizeBorderThickness.Bottom + SystemParameters.WindowNonClientFrameThickness.Bottom);

					captionHeight += ChromeOutmostBorder.BorderThickness.Top;
				}

				if (ChromeBorder != null)
				{
					ChromeBorder.BorderThickness = new Thickness(0D);
				}
			}
			else
			{
				if (ChromeOutmostBorder != null)
				{
					ChromeOutmostBorder.BorderThickness = new Thickness(0D);

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
		private bool _isDueCheckBackground = true;

		/// <summary>
		/// Whether checking default chrome background is due.
		/// </summary>
		private bool _isDueCheckDefaultChromeBackground = false;

		/// <summary>
		/// Chrome background color to be actually used
		/// </summary>
		private Brush _chromeBackgroundActual = Brushes.Transparent;

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
		/// <remarks>
		/// <para>This property will be changed when the Window is about to be activated or deactivated.</para>
		/// <para>For binding only between code behind.</para>
		/// </remarks>
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

			this.Background = _chromeBackgroundActual;
			this.Foreground = ChromeForeground;

			if (TitleBarBackGrid != null)
				TitleBarBackGrid.Background = TitleBarBackground;
		}

		private void SetDeactivated()
		{
			IsAboutActive = false;

			this.Background = ChromeDeactivatedBackground;
			this.Foreground = ChromeDeactivatedForeground;

			if (TitleBarBackGrid != null)
				TitleBarBackGrid.Background = ChromeDeactivatedBackground;
		}

		private void CheckBackground()
		{
			if (!_isDueCheckBackground && !_isDueCheckDefaultChromeBackground)
				return;

			if (_isDueCheckDefaultChromeBackground)
			{
				var color = WindowChromeColor.GetChromeColor();
				if (color.HasValue)
					DefaultChromeBackground = new SolidColorBrush(color.Value);
			}

			_chromeBackgroundActual = (UsesDefaultChromeBackground || ChromeBackground.IsTransparent())
				? DefaultChromeBackground
				: ChromeBackground;

			CaptionButtonBackground = TitleBarBackground.IsTransparent()
				? _chromeBackgroundActual
				: TitleBarBackground;

			AddDragHandler();

			_isDueCheckBackground = false;
			_isDueCheckDefaultChromeBackground = false;
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

		private void ManageTitleBarIconClick(MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1) // Single click
			{
				var clickPoint = this.PointToScreen(e.GetPosition(null));
				SystemCommands.ShowSystemMenu(this, new Point(clickPoint.X + 1, clickPoint.Y + 1));
			}
			else if ((e.ClickCount == 2) && (e.LeftButton == MouseButtonState.Pressed)) // Double Click
			{
				SystemCommands.CloseWindow(this);
			}
		}

		#endregion
	}
}