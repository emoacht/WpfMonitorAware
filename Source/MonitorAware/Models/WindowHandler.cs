using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using MonitorAware.Models.Win32;

namespace MonitorAware.Models
{
	/// <summary>
	/// Handler for <see cref="System.Windows.Window"/>
	/// </summary>
	public class WindowHandler : DependencyObject
	{
		#region Type

		/// <summary>
		/// Status of a Window
		/// </summary>
		private enum WindowStatus
		{
			/// <summary>
			/// A Window stands still.
			/// </summary>
			None = 0,

			/// <summary>
			/// A Window's location is being changed.
			/// </summary>
			LocationChanged,

			/// <summary>
			/// A Window's size is being changed.
			/// </summary>
			SizeChanged,
		}

		/// <summary>
		/// DPI and other information on a Window
		/// </summary>
		private class WindowInfo
		{
			public DpiScale Dpi { get; set; }
			public double Width { get; set; }
			public double Height { get; set; }

			public Size Size
			{
				get => new Size(Width, Height);
				set => (this.Width, this.Height) = (value.Width, value.Height);
			}
		}

		#endregion

		#region Property (DPI)

		/// <summary>
		/// Whether target Window is Per-Monitor DPI aware (readonly)
		/// </summary>
		public bool IsPerMonitorDpiAware => DpiHelper.IsPerMonitorDpiAware();

		/// <summary>
		/// System DPI (readonly)
		/// </summary>
		public DpiScale SystemDpi => DpiHelper.SystemDpi;

		/// <summary>
		/// Per-Monitor DPI of current monitor (public readonly)
		/// </summary>
		/// <remarks>This property cannot become a binding target because it has no public setter.</remarks>
		public DpiScale MonitorDpi
		{
			get { return (DpiScale)GetValue(MonitorDpiProperty); }
			private set { SetValue(MonitorDpiPropertyKey, value); }
		}
		/// <summary>
		/// Dependency property key for <see cref="MonitorDpi"/>
		/// </summary>
		private static readonly DependencyPropertyKey MonitorDpiPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"MonitorDpi",
				typeof(DpiScale),
				typeof(WindowHandler),
				new PropertyMetadata(DpiHelper.Identity));
		/// <summary>
		/// Dependency property for <see cref="MonitorDpi"/>
		/// </summary>
		public static readonly DependencyProperty MonitorDpiProperty = MonitorDpiPropertyKey.DependencyProperty;

		/// <summary>
		/// Per-Monitor DPI of target Window (public readonly)
		/// </summary>
		/// <remarks>This property cannot become a binding target because it has no public setter.</remarks>
		public DpiScale WindowDpi
		{
			get { return (DpiScale)GetValue(WindowDpiProperty); }
			private set { SetValue(WindowDpiPropertyKey, value); }
		}
		/// <summary>
		/// Dependency property key for <see cref="WindowDpi"/>
		/// </summary>
		private static readonly DependencyPropertyKey WindowDpiPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"WindowDpi",
				typeof(DpiScale),
				typeof(WindowHandler),
				new PropertyMetadata(DpiHelper.Identity));
		/// <summary>
		/// Dependency property for <see cref="WindowDpi"/>
		/// </summary>
		public static readonly DependencyProperty WindowDpiProperty = WindowDpiPropertyKey.DependencyProperty;

		/// <summary>
		/// Per-Monitor DPI of target Window when it is initialized
		/// </summary>
		protected DpiScale InitialDpi { get; private set; }

		/// <summary>
		/// Scaling factor for <see cref="Transform.Identity"/>
		/// </summary>
		/// <remarks>This must locate before <see cref="ScaleFactor"/> property.</remarks>
		private static readonly Point IdentityFactor = new Point(1D, 1D);

		/// <summary>
		/// Scaling factor of target Window (public readonly)
		/// </summary>
		/// <remarks>This property cannot become a binding target because it has no public setter.</remarks>
		public Point ScaleFactor
		{
			get { return (Point)GetValue(ScaleFactorProperty); }
			private set { SetValue(ScaleFactorPropertyKey, value); }
		}
		/// <summary>
		/// Dependency property key for <see cref="ScaleFactor"/>
		/// </summary>
		private static readonly DependencyPropertyKey ScaleFactorPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"ScaleFactor",
				typeof(Point),
				typeof(WindowHandler),
				new PropertyMetadata(IdentityFactor));
		/// <summary>
		/// Dependency property for <see cref="ScaleFactor"/>
		/// </summary>
		public static readonly DependencyProperty ScaleFactorProperty = ScaleFactorPropertyKey.DependencyProperty;

		/// <summary>
		/// Scaling mode of target Window
		/// </summary>
		public ScaleMode ScaleMode
		{
			get { return (ScaleMode)GetValue(ScaleModeProperty); }
			set { SetValue(ScaleModeProperty, value); }
		}
		/// <summary>
		/// Dependency property for <see cref="ScaleMode"/>
		/// </summary>
		public static readonly DependencyProperty ScaleModeProperty =
			DependencyProperty.Register(
				"ScaleMode",
				typeof(ScaleMode),
				typeof(WindowHandler),
				new PropertyMetadata(ScaleMode.InvokeWhileMoving));

		#endregion

		#region Property (Color profile)

		/// <summary>
		/// Color profile path of target Window (public readonly)
		/// </summary>
		/// <remarks>This property cannot become a binding target because it has no public setter.</remarks>
		public string ColorProfilePath
		{
			get { return (string)GetValue(ColorProfilePathProperty); }
			private set { SetValue(ColorProfilePathPropertyKey, value); }
		}
		/// <summary>
		/// Dependency property key for <see cref="ColorProfilePath"/>
		/// </summary>
		private static readonly DependencyPropertyKey ColorProfilePathPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"ColorProfilePath",
				typeof(string),
				typeof(WindowHandler),
				new FrameworkPropertyMetadata(
					string.Empty,
					(d, e) => Debug.WriteLine($"Color Profile Path: {(string)e.NewValue}")));
		/// <summary>
		/// Dependency property for <see cref="ColorProfilePath"/>
		/// </summary>
		public static readonly DependencyProperty ColorProfilePathProperty = ColorProfilePathPropertyKey.DependencyProperty;

		#endregion

		#region Event

		/// <summary>
		/// Occurs when target Window's DPI is changed.
		/// </summary>
		/// <remarks>
		/// This event is fired when DPI of target Window is changed. It is not necessarily the same timing
		/// when DPI of the monitor to which target Window belongs is changed.
		/// </remarks>
		public event EventHandler<DpiChangedEventArgs> DpiChanged;

		/// <summary>
		/// Occurs when color profile path has changed.
		/// </summary>
		/// <remarks>
		/// This event is fired when color profile path of the monitor to which target Window belongs has changed
		/// and Window's move/resize which caused the change has exited.
		/// </remarks>
		public event EventHandler<ColorProfileChangedEventArgs> ColorProfileChanged;

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public WindowHandler()
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="window">Target Window</param>
		/// <param name="element">Target FrameworkElement</param>
		public WindowHandler(Window window, FrameworkElement element = null)
		{
			Initialize(window, element);
		}

		#endregion

		/// <summary>
		/// Target Window
		/// </summary>
		private Window _targetWindow;

		/// <summary>
		/// Target FrameworkElement which will be transformed when DPI is changed
		/// </summary>
		/// <remarks>If this FrameworkElement is null, Window.Content will be transformed.</remarks>
		private FrameworkElement _targetElement;

		private HwndSource _targetSource;

		/// <summary>
		/// Initializes.
		/// </summary>
		/// <param name="window">Target Window</param>
		/// <param name="element">Target FrameworkElement</param>
		protected internal void Initialize(Window window, FrameworkElement element = null)
		{
			if (window is null)
				throw new ArgumentNullException(nameof(window));

			if (!window.IsInitialized)
				throw new InvalidOperationException("Target Window has not been initialized.");

			_targetWindow = window;
			_targetElement = element;

			InitialDpi = WindowDpi = MonitorDpi = IsPerMonitorDpiAware
				? DpiHelper.GetDpiFromVisual(_targetWindow)
				: SystemDpi;

			if (ColorProfileHelper.TryGetColorProfilePath(_targetWindow, out string profilePath))
				ColorProfilePath = profilePath;

			_targetSource = PresentationSource.FromVisual(_targetWindow) as HwndSource;
			_targetSource?.AddHook(WndProc);
		}

		/// <summary>
		/// Closes.
		/// </summary>
		protected internal void Close()
		{
			_targetSource?.RemoveHook(WndProc);
		}

		/// <summary>
		/// Information that target Window due to be
		/// </summary>
		private WindowInfo _dueInfo;

		/// <summary>
		/// Current status of target Window
		/// </summary>
		private WindowStatus _currentStatus = WindowStatus.None;

		/// <summary>
		/// Whether target Window's location or size has started to be changed
		/// </summary>
		private bool _isEnteredSizeMove;

		/// <summary>
		/// Whether DPI has changed after target Window's location or size has started to be changed
		/// </summary>
		private bool _isDpiChanged;

		/// <summary>
		/// Target Window's size to be the base for calculating due size when DPI changed
		/// </summary>
		private Size _baseSize = Size.Empty;

		/// <summary>
		/// Count of WM_MOVE messages
		/// </summary>
		private int _countLocationChanged;

		/// <summary>
		/// Count of WM_SIZE messages
		/// </summary>
		private int _countSizeChanged;

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
		protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WindowMessage.WM_DPICHANGED:
					var newDpi = DpiChangedEventHelper.ConvertPointerToDpi(wParam);

					Debug.WriteLine($"DPICHANGED: {MonitorDpi.PixelsPerInchX} -> {newDpi.PixelsPerInchX}");

					if (MonitorDpi.Equals(newDpi))
						break;

					var oldDpi = MonitorDpi;
					MonitorDpi = newDpi;

					if (ScaleMode == ScaleMode.Forbear)
					{
						WindowDpi = newDpi;
						break;
					}

					_isDpiChanged = true;

					var newInfo = new WindowInfo { Dpi = newDpi };

					switch (_currentStatus)
					{
						case WindowStatus.None:
						case WindowStatus.LocationChanged:
							newInfo.Size = _baseSize = (_baseSize != Size.Empty)
								? new Size(
									_baseSize.Width * newDpi.DpiScaleX / oldDpi.DpiScaleX,
									_baseSize.Height * newDpi.DpiScaleY / oldDpi.DpiScaleY)
								: DpiChangedEventHelper.ConvertPointerToRect(lParam).Size;

							break;

						case WindowStatus.SizeChanged:
							// None.
							break;
					}

					Interlocked.Exchange(ref _dueInfo, newInfo);

					switch (_currentStatus)
					{
						case WindowStatus.None:
							ChangeDpi();
							break;

						case WindowStatus.LocationChanged:
							// None.
							break;

						case WindowStatus.SizeChanged:
							ChangeDpi(WindowStatus.SizeChanged);
							break;
					}

					handled = true;
					break;

				case WindowMessage.WM_ENTERSIZEMOVE:
					Debug.WriteLine("ENTERSIZEMOVE");

					if (!IsPerMonitorDpiAware || (ScaleMode == ScaleMode.Forbear))
						break;

					_isEnteredSizeMove = true;
					_isDpiChanged = false;

					_baseSize = WindowHelper.TryGetWindowRect(_targetSource.Handle, out Rect baseRect)
						? baseRect.Size
						: Size.Empty;

					_countLocationChanged = 0;
					_countSizeChanged = 0;
					break;

				case WindowMessage.WM_EXITSIZEMOVE:
					Debug.WriteLine("EXITSIZEMOVE");

					if (_isEnteredSizeMove)
					{
						_isEnteredSizeMove = false;

						// Last stand!!!
						if (_isDpiChanged && (_baseSize != Size.Empty) && (_currentStatus == WindowStatus.LocationChanged))
						{
							var lastInfo = new WindowInfo
							{
								Dpi = MonitorDpi,
								Size = _baseSize,
							};

							Interlocked.Exchange(ref _dueInfo, lastInfo);

							ChangeDpi(WindowStatus.LocationChanged);
						}

						_currentStatus = WindowStatus.None;
					}

					ChangeColorProfilePath();
					break;

				case WindowMessage.WM_MOVE:
					Debug.WriteLine("MOVE");

					if (_isEnteredSizeMove)
					{
						_countLocationChanged++;
						if (_countLocationChanged > _countSizeChanged)
							_currentStatus = WindowStatus.LocationChanged;

						if (ScaleMode == ScaleMode.InvokeWhileMoving)
							ChangeDpi(WindowStatus.LocationChanged);
					}
					break;

				case WindowMessage.WM_SIZE when ((uint)wParam == WindowMessage.SIZE_RESTORED):
					Debug.WriteLine("SIZE");

					if (_isEnteredSizeMove)
					{
						_countSizeChanged++;
						if (_countLocationChanged < _countSizeChanged)
							_currentStatus = WindowStatus.SizeChanged;

						// DPI change by resize will be managed when WM_DPICHANGED comes.
					}
					break;
			}
			return IntPtr.Zero;
		}

		/// <summary>
		/// Object to block entering into change DPI process
		/// </summary>
		/// <remarks>
		/// Null:   Don't block.
		/// Object: Block.
		/// </remarks>
		private object _blocker = null;

		private void ChangeDpi(WindowStatus status = WindowStatus.None)
		{
			if (Interlocked.CompareExchange(ref _blocker, new object(), null) != null)
				return;

			try
			{
				// Take information which is to be tested from _dueInfo and set null in return.
				var testInfo = Interlocked.Exchange(ref _dueInfo, null);

				while (testInfo != null)
				{
					if (!WindowHelper.TryGetWindowRect(_targetSource.Handle, out Rect windowRect))
						continue;

					var testRect = new Rect(windowRect.X, windowRect.Y, testInfo.Width, testInfo.Height);

					bool changesNow = true;

					switch (status)
					{
						case WindowStatus.None:
						case WindowStatus.SizeChanged:
							// None.
							break;

						case WindowStatus.LocationChanged:
							// Determine whether to reflect information now.
							var testDpi = DpiHelper.GetDpiFromRect(testRect);

							changesNow = testInfo.Dpi.Equals(testDpi);
							break;
					}

					if (changesNow)
					{
						// Update WindowDpi first so that it can provide new DPI during succeeding changes in target
						// Window.
						var oldDpi = WindowDpi;
						WindowDpi = testInfo.Dpi;

						switch (status)
						{
							case WindowStatus.None:
							case WindowStatus.LocationChanged:
								// Change location and size of target Window. Calling virtually identical functions
								// in a row is to ensure that the change of both width and height are duly reflected.
								if (!WindowHelper.SetWindowLocation(_targetSource.Handle, testRect.X, testRect.Y) ||
									!WindowHelper.SetWindowSize(_targetSource.Handle, testRect.Size))
									continue;

								break;

							case WindowStatus.SizeChanged:
								// None.
								break;
						}

						// Scale contents of target Window.
						var content = (_targetElement ??= VisualTreeHelperAddition.GetDescendant<FrameworkElement>(_targetWindow))
							?? (_targetWindow.Content as FrameworkElement);

						if (content != null)
						{
							if (WindowDpi.Equals(InitialDpi))
							{
								content.LayoutTransform = Transform.Identity;
								ScaleFactor = IdentityFactor;
							}
							else
							{
								var factor = new Point(
									WindowDpi.DpiScaleX / InitialDpi.DpiScaleX,
									WindowDpi.DpiScaleY / InitialDpi.DpiScaleY);
								content.LayoutTransform = new ScaleTransform(factor.X, factor.Y);
								ScaleFactor = factor;
							}
						}

						// Fire DpiChanged event last so that it can be utilized to supplement preceding changes
						// in target Window.
						DpiChanged?.Invoke(this, DpiChangedEventHelper.Create(_targetWindow, oldDpi, WindowDpi));

						// Set root DPI so as to fire Window.DpiChanged event.
						VisualTreeHelper.SetRootDpi(_targetWindow, WindowDpi);

						// Take new information which is to be tested from _dueInfo again for the case where new
						// information has been stored during this operation. If there is new information, repeat
						// the operation.
						testInfo = Interlocked.Exchange(ref _dueInfo, null);
					}
					else
					{
						// Store old information which has been tested but determined not to be reflected now to
						// _dueInfo and take new information in return. If there is new information, repeat
						// the operation. If not, old information stored back may be overwritten by new information
						// later but has a chance to be tested again in the case where it is the last information
						// at this move/resize. In such case, the information may be tested at next move/resize.
						testInfo = Interlocked.Exchange(ref _dueInfo, testInfo);
					}
				}
			}
			finally
			{
				Interlocked.Exchange(ref _blocker, null);
			}
		}

		private void ChangeColorProfilePath()
		{
			var newPath = ColorProfileHelper.GetColorProfilePath(_targetWindow);
			if (ColorProfilePath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
				return;

			var oldPath = ColorProfilePath;
			ColorProfilePath = newPath;

			ColorProfileChanged?.Invoke(this, new ColorProfileChangedEventArgs(oldPath, ColorProfilePath));
		}
	}
}