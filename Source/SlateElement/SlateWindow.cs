using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using SlateElement.Helper;
using SlateElement.Models;
using SlateElement.Models.Win32;

namespace SlateElement
{
	/// <summary>
	/// Customizable chrome Window
	/// </summary>
	[TemplatePart(Name = "PART_ChromeBorder", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ChromeGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_TitleBarContentGrid", Type = typeof(Grid))]
	public class SlateWindow : Window
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public SlateWindow()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(SlateWindow), new FrameworkPropertyMetadata(typeof(SlateWindow)));

			RegisterCommands();
		}

		/// <summary>
		/// URI string of prototype resources for SlateWindow's Style
		/// </summary>
		public const string PrototypeResourceUriString = "/SlateElement;component/Prototype.xaml";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="resourceUriString">URI string of resources for SlateWindow's Style</param>
		public SlateWindow(string resourceUriString) : this()
		{
			if (!string.IsNullOrWhiteSpace(resourceUriString))
			{
				Application.Current.ApplyResource(resourceUriString);
				this.Style = Application.Current.FindResource(typeof(SlateWindow)) as Style;
			}
		}

		/// <summary>
		/// OnApplyTemplate
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ChromeBorder = this.GetTemplateChild("PART_ChromeBorder") as Border;
			ChromeGrid = this.GetTemplateChild("PART_ChromeGrid") as Grid;
			TitleBarGrid = this.GetTemplateChild("PART_TitleBarGrid") as Grid;
			TitleBarContentGrid = this.GetTemplateChild("PART_TitleBarContentGrid") as Grid;
		}

		private HwndSource _targetSource;

		/// <summary>
		/// OnSourceInitialized
		/// </summary>
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			ManageChromeBorder();
			ManageTitleBarBackground();
			ManageTitleBarContent();
			ManageTitleBarCaptionButtons();

			_targetSource = PresentationSource.FromVisual(this) as HwndSource;
			_targetSource?.AddHook(WndProc);
		}

		/// <summary>
		/// OnClosed
		/// </summary>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			_targetSource?.RemoveHook(WndProc);
		}

		/// <summary>
		/// Handles window messages.
		/// </summary>
		/// <param name="hwnd">The window handle</param>
		/// <param name="msg">The message ID</param>
		/// <param name="wParam">The message's wParam value</param>
		/// <param name="lParam">The message's lParam value</param>
		/// <param name="handled">Whether the message was handled</param>
		/// <returns>Return value depending on the particular message</returns>
		/// <remarks>This is an implementation of System.Windows.Interop.HwndSourceHook.</remarks>
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WindowMessage.WM_GETMINMAXINFO:
					// Adjust this Window's location and size when it is maximized.
					if (WindowHelper.TryGetMonitorRect(hwnd, out Rect monitorRect, out Rect workRect, out _))
					{
						// The coordinates start in the left-top corner of current monitor.
						var maximizeLocation = new Point(workRect.X - monitorRect.X, workRect.Y - monitorRect.Y);
						var maximizeSize = new Point(workRect.Width, workRect.Height);

						var info = Marshal.PtrToStructure<NativeMethod.MINMAXINFO>(lParam);
						info.ptMaxPosition = maximizeLocation;
						info.ptMaxSize = maximizeSize;
						Marshal.StructureToPtr(info, lParam, true);
					}
					break;

				case WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED:
					// Update OS's default chrome background Brush.
					if (WindowChromeColor.TryGetChromeColor(out Color chromeColor))
						DefaultChromeBackground = new SolidColorBrush(chromeColor);

					break;
			}
			return IntPtr.Zero;
		}

		/// <summary>
		/// OnDpiChanged
		/// </summary>
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);

			ManageChromeBorder(newDpi);
		}

		/// <summary>
		/// OnStateChanged
		/// </summary>
		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			ManageChromeBorder();
			ManageTitleBarCaptionButtons();
		}

		/// <summary>
		/// Whether this Window is maximized
		/// </summary>
		protected bool IsMaximized => (this.WindowState == WindowState.Maximized);

		#region Template Part

		/// <summary>
		/// Window chrome outer border
		/// </summary>
		protected Border ChromeBorder { get; private set; }

		/// <summary>
		/// Window chrome grid
		/// </summary>
		public Grid ChromeGrid { get; private set; }

		/// <summary>
		/// Title bar grid
		/// </summary>
		protected Grid TitleBarGrid { get; private set; }

		/// <summary>
		/// Title bar content grid
		/// </summary>
		protected Grid TitleBarContentGrid { get; private set; }

		#endregion

		#region Command

		#region SystemCommands.MinimizeWindow

		private void MinimizeExecuted(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(this);

		#endregion

		#region SystemCommands.MaximizeWindow

		private void MaximizeExecuted(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MaximizeWindow(this);
		private void CanMaximizeExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanResize && !IsMaximized;

		#endregion

		#region SystemCommands.RestoreWindow

		private void RestoreExecuted(object sender, ExecutedRoutedEventArgs e) => SystemCommands.RestoreWindow(this);
		private void CanRestoreExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanResize && IsMaximized;

		#endregion

		#region SystemCommands.CloseWindow

		private void CloseExecuted(object target, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);

		#endregion

		private void RegisterCommands()
		{
			this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeExecuted));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeExecuted, CanMaximizeExecute));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreExecuted, CanRestoreExecute));
			this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseExecuted));
		}

		/// <summary>
		/// Getter for CaptionButton
		/// </summary>
		public static SlateCaptionButtonType GetCaptionButton(Button button)
		{
			return (SlateCaptionButtonType)button.GetValue(CaptionButtonProperty);
		}
		/// <summary>
		/// Setter for CaptionButton
		/// </summary>
		public static void SetCaptionButton(Button button, SlateCaptionButtonType value)
		{
			button.SetValue(CaptionButtonProperty, value);
		}
		/// <summary>
		/// Attached property for CaptionButton
		/// </summary>
		public static readonly DependencyProperty CaptionButtonProperty =
			DependencyProperty.RegisterAttached(
				"CaptionButton",
				typeof(SlateCaptionButtonType),
				typeof(SlateWindow),
				new PropertyMetadata(
					default(SlateCaptionButtonType),
					(d, e) =>
					{
						if ((d is Button button) && (Window.GetWindow(button) is SlateWindow window))
							window.InitializeCaptionButton(button, (SlateCaptionButtonType)e.NewValue);
					}));

		private Button _minimizeButton;
		private Button _maximizeButton;
		private Button _restoreButton;

		private void InitializeCaptionButton(Button button, SlateCaptionButtonType buttonType)
		{
			switch (buttonType)
			{
				case SlateCaptionButtonType.Minimize:
					button.Command = SystemCommands.MinimizeWindowCommand;
					_minimizeButton = button;
					break;

				case SlateCaptionButtonType.Maximize:
					button.Command = SystemCommands.MaximizeWindowCommand;
					_maximizeButton = button;
					break;

				case SlateCaptionButtonType.Restore:
					button.Command = SystemCommands.RestoreWindowCommand;
					_restoreButton = button;
					break;

				case SlateCaptionButtonType.Close:
					button.Command = SystemCommands.CloseWindowCommand;
					break;
			}
		}

		private bool CanResize => (this.ResizeMode == ResizeMode.CanResize) || (this.ResizeMode == ResizeMode.CanResizeWithGrip);

		private void ManageTitleBarCaptionButtons()
		{
			static void SetVisible(Button button, bool isVisible) =>
				button?.SetValue(VisibilityProperty, (isVisible ? Visibility.Visible : Visibility.Collapsed));

			switch (this.ResizeMode)
			{
				case ResizeMode.NoResize:
					SetVisible(_minimizeButton, false);
					SetVisible(_maximizeButton, false);
					SetVisible(_restoreButton, false);
					break;

				case ResizeMode.CanMinimize:
					SetVisible(_minimizeButton, true);
					SetVisible(_maximizeButton, true);
					SetVisible(_restoreButton, false);
					break;

				default: // ResizeMode.CanResize, ResizeMode.CanResizeWithGrip
					SetVisible(_minimizeButton, true);
					SetVisible(_maximizeButton, !IsMaximized);
					SetVisible(_restoreButton, IsMaximized);
					break;
			}

			CommandManager.InvalidateRequerySuggested();
		}

		#endregion

		#region Layout (Property)

		/// <summary>
		/// OS's default chrome background Brush when a Window is activated (public readonly)
		/// </summary>
		public Brush DefaultChromeBackground
		{
			get { return (Brush)GetValue(DefaultChromeBackgroundProperty); }
			private set { SetValue(DefaultChromeBackgroundPropertyKey, value); }
		}
		/// <summary>
		/// Gets OS's default chrome Brush.
		/// </summary>
		/// <returns>If successfully gets, default chrome Brush. If not, null.</returns>
		private static SolidColorBrush GetDefaultChromeBrush() =>
			WindowChromeColor.TryGetChromeColor(out Color chromeColor) ? new SolidColorBrush(chromeColor) : null;
		/// <summary>
		/// Dependency property for <see cref="DefaultChromeBackground"/>
		/// </summary>
		private static readonly DependencyPropertyKey DefaultChromeBackgroundPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"DefaultChromeBackground",
				typeof(Brush),
				typeof(SlateWindow),
				new PropertyMetadata(GetDefaultChromeBrush()));
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
				typeof(SlateWindow),
				new PropertyMetadata(
					Brushes.Transparent,
					(d, e) => ((SlateWindow)d).ManageTitleBarBackground()));

		/// <summary>
		/// Chrome foreground Brush when a Window is activated
		/// </summary>
		/// <remarks>Default value (Black) is OS's default.</remarks>
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
				typeof(SlateWindow),
				new PropertyMetadata(Brushes.Black));

		/// <summary>
		/// Chrome border thickness
		/// </summary>
		/// <remarks>Default value (1) is OS's default.</remarks>
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
				typeof(SlateWindow),
				new PropertyMetadata(new Thickness(1D)));

		/// <summary>
		/// Chrome border Brush
		/// </summary>
		/// <remarks>Default value (#3C000000) is OS's default.</remarks>
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
				typeof(SlateWindow),
				new PropertyMetadata(_chromeBorderBrush));

		/// <summary>
		/// Title bar icon
		/// </summary>
		public Image TitleBarIcon
		{
			get { return (Image)GetValue(TitleBarIconProperty); }
			set { SetValue(TitleBarIconProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="TitleBarIcon"/>
		/// </summary>
		public static readonly DependencyProperty TitleBarIconProperty =
			DependencyProperty.Register(
				"TitleBarIcon",
				typeof(Image),
				typeof(SlateWindow),
				new PropertyMetadata(
					null,
					(d, e) =>
					{
						var window = (SlateWindow)d;
						window.RemoveTitleBarIconHandler(e.OldValue as Image);
						window.AddTitleBarIconHandler(e.NewValue as Image);
						window.ManageTitleBarContent();
					}));

		/// <summary>
		/// Title bar content
		/// </summary>
		public FrameworkElement TitleBarContent
		{
			get { return (FrameworkElement)GetValue(TitleBarContentProperty); }
			set { SetValue(TitleBarContentProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="TitleBarContent"/>
		/// </summary>
		public static readonly DependencyProperty TitleBarContentProperty =
			DependencyProperty.Register(
				"TitleBarContent",
				typeof(FrameworkElement),
				typeof(SlateWindow),
				new PropertyMetadata(
					null,
					(d, e) => ((SlateWindow)d).ManageTitleBarContent()));

		#endregion

		#region Layout (Method)

		/// <summary>
		/// This window's DPI
		/// </summary>
		protected DpiScale WindowDpi { get; set; }

		/// <summary>
		/// Manages chrome outer border.
		/// </summary>
		protected virtual void ManageChromeBorder(DpiScale dpi = default)
		{
			if (!dpi.Equals(default(DpiScale)))
			{
				WindowDpi = dpi;
			}
			else if (WindowDpi.Equals(default(DpiScale)))
			{
				WindowDpi = VisualTreeHelper.GetDpi(this);
			}

			if (ChromeBorder != null)
			{
				ChromeBorder.BorderThickness = IsMaximized
					? new Thickness(0)
					: new Thickness(
						ChromeBorderThickness.Left / WindowDpi.DpiScaleX,
						ChromeBorderThickness.Top / WindowDpi.DpiScaleY,
						ChromeBorderThickness.Right / WindowDpi.DpiScaleX,
						ChromeBorderThickness.Bottom / WindowDpi.DpiScaleY);
			}
		}

		/// <summary>
		/// Manages title bar background.
		/// </summary>
		protected virtual void ManageTitleBarBackground()
		{
			if (TitleBarGrid != null)
				TitleBarGrid.Background = ChromeBackground;

			AddTitleBarHandler();
		}

		private void AddTitleBarHandler()
		{
			RemoveTitleBarHandler();

			if (TitleBarGrid?.Background != null)
			{
				TitleBarGrid.MouseLeftButtonDown += OnTitleBarMouseLeftButtonDown;
				TitleBarGrid.MouseRightButtonDown += OnTitleBarMouseRightButtonDown;
			}
			else
			{
				this.MouseLeftButtonDown += OnTitleBarMouseLeftButtonDown;
			}
		}

		private void RemoveTitleBarHandler()
		{
			if (TitleBarGrid != null)
			{
				TitleBarGrid.MouseLeftButtonDown -= OnTitleBarMouseLeftButtonDown;
				TitleBarGrid.MouseRightButtonDown -= OnTitleBarMouseRightButtonDown;
			}

			// It is always necessary to remove this event handler. Othewise, an event handler which
			// was added when TitleBarGrid was null will not be removed after TitleBarGrid is assigned.
			this.MouseLeftButtonDown -= OnTitleBarMouseLeftButtonDown;
		}

		/// <summary>
		/// Manages title bar content.
		/// </summary>
		protected virtual void ManageTitleBarContent()
		{
			if (TitleBarContentGrid is null)
				return;

			TitleBarContentGrid.ColumnDefinitions.Clear();
			TitleBarContentGrid.Children.Clear();

			if (TitleBarIcon != null)
			{
				TitleBarContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				TitleBarContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1D, GridUnitType.Star) });

				Grid.SetColumn(TitleBarIcon, 0);
				TitleBarContentGrid.Children.Add(TitleBarIcon);

				if (TitleBarContent != null)
				{
					Grid.SetColumn(TitleBarContent, 1);
					TitleBarContentGrid.Children.Add(TitleBarContent);
				}
			}
			else
			{
				if (TitleBarContent != null)
				{
					TitleBarContentGrid.Children.Add(TitleBarContent);
				}
			}
		}

		private void AddTitleBarIconHandler(Image titleBarIcon)
		{
			if (titleBarIcon != null)
				titleBarIcon.MouseDown += OnTitleBarIconMouseDown;
		}

		private void RemoveTitleBarIconHandler(Image titleBarIcon)
		{
			if (titleBarIcon != null)
				titleBarIcon.MouseDown -= OnTitleBarIconMouseDown;
		}

		private bool _isTransitionFromMaximizedToDragged;

		private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch (e.ClickCount)
			{
				case 1 when (e.ButtonState == MouseButtonState.Pressed): // Single click
					e.Handled = true;

					_isTransitionFromMaximizedToDragged = IsMaximized;
					this.DragMove();
					break;

				case 2 when CanResize: // Double Click
					e.Handled = true;

					if (IsMaximized)
						SystemCommands.RestoreWindow(this);
					else
						SystemCommands.MaximizeWindow(this);

					break;
			}
		}

		/// <summary>
		/// OnMouseMove
		/// </summary>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_isTransitionFromMaximizedToDragged)
			{
				_isTransitionFromMaximizedToDragged = false;

				if (RestoreWindow())
					this.DragMove();
				else
					SystemCommands.RestoreWindow(this); // Fallback
			}

			bool RestoreWindow()
			{
				var mouseClientLocation = e.MouseDevice.GetPosition(this);
				var mouseScreenLocation = this.PointToScreen(mouseClientLocation);

				var handle = new WindowInteropHelper(this).Handle;
				if (!WindowHelper.TryGetWindowRect(handle, out Rect currentRect))
					return false;

				if (!WindowHelper.TryGetWindowNormalRect(handle, out Rect restoreRect))
					return false;

				var left = Math.Min(Math.Max((mouseScreenLocation.X - restoreRect.Width / 2D), currentRect.Left), currentRect.Right - restoreRect.Width);
				var top = mouseScreenLocation.Y - mouseClientLocation.Y;
				return WindowHelper.SetWindowNormalRect(handle, new Rect(left, top, restoreRect.Width, restoreRect.Height));
			}
		}

		/// <summary>
		/// OnMouseLeftButtonUp
		/// </summary>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			_isTransitionFromMaximizedToDragged = false;
		}

		private void OnTitleBarMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch (e.ClickCount)
			{
				case 1:
					SystemCommands.ShowSystemMenu(this, GetLocationForSystemMenu(e));
					break;
			}
		}

		private void OnTitleBarIconMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			switch (e.ClickCount)
			{
				case 1: // Single click
					SystemCommands.ShowSystemMenu(this, GetLocationForSystemMenu(e));
					break;

				case 2 when (e.LeftButton == MouseButtonState.Pressed): // Double Click
					SystemCommands.CloseWindow(this);
					break;
			}
		}

		private Point GetLocationForSystemMenu(MouseButtonEventArgs e)
		{
			var mouseLocation = this.PointToScreen(e.GetPosition(null));

			// Move the location so that the system menu will not block double click.
			var movedLocation = new Point(mouseLocation.X + 1D, mouseLocation.Y + 1D);

			// Adjust the location to nullify scaling logic of SystemCommands.ShowSystemMenu method.
			var adjustedLocation = new Point(movedLocation.X / WindowDpi.DpiScaleX, movedLocation.Y / WindowDpi.DpiScaleY);

			return adjustedLocation;
		}

		#endregion
	}
}