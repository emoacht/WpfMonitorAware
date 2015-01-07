using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using PerMonitorDpi.Models.Win32;

namespace PerMonitorDpi.Models
{
    /// <summary>
    /// Handler for <see cref="Window"/>
    /// </summary>
    public class WindowHandler : DependencyObject
    {
        #region Type

        /// <summary>
        /// Status of a <see cref="Window"/>
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
        /// DPI and other information on a <see cref="Window"/>
        /// </summary>
        private class WindowInfo
        {
            public Dpi Dpi { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }

            public Size Size
            {
                get { return new Size(this.Width, this.Height); }
                set
                {
                    this.Width = value.Width;
                    this.Height = value.Height;
                }
            }
        }

        #endregion


        #region Property

        /// <summary>
        /// Whether target Window is Per-Monitor DPI aware (readonly)
        /// </summary>
        public bool IsPerMonitorDpiAware
        {
            get { return DpiChecker.IsPerMonitorDpiAware(); }
        }

        /// <summary>
        /// System DPI (readonly)
        /// </summary>
        public Dpi SystemDpi
        {
            get { return _systemDpi; }
        }
        private readonly Dpi _systemDpi = DpiChecker.GetSystemDpi();

        /// <summary>
        /// Per-Monitor DPI of current monitor (public readonly)
        /// </summary>
        public Dpi MonitorDpi
        {
            get { return (Dpi)GetValue(MonitorDpiProperty); }
            private set { SetValue(MonitorDpiPropertyKey, value); }
        }
        /// <summary>
        /// Dependency property key for <see cref="MonitorDpi"/>
        /// </summary>
        private static readonly DependencyPropertyKey MonitorDpiPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "MonitorDpi",
                typeof(Dpi),
                typeof(WindowHandler),
                new FrameworkPropertyMetadata(
                    Dpi.Default,
                    (d, e) => Debug.WriteLine("Monitor DPI: {0}", (Dpi)e.NewValue)));
        /// <summary>
        /// Dependency property for <see cref="MonitorDpi"/>
        /// </summary>
        public static readonly DependencyProperty MonitorDpiProperty = MonitorDpiPropertyKey.DependencyProperty;

        /// <summary>
        /// Per-Monitor DPI of target Window (public readonly)
        /// </summary>
        public Dpi WindowDpi
        {
            get { return (Dpi)GetValue(WindowDpiProperty); }
            private set { SetValue(WindowDpiPropertyKey, value); }
        }
        /// <summary>
        /// Dependency property key for <see cref="WindowDpi"/>
        /// </summary>
        private static readonly DependencyPropertyKey WindowDpiPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "WindowDpi",
                typeof(Dpi),
                typeof(WindowHandler),
                new FrameworkPropertyMetadata(
                    Dpi.Default,
                    (d, e) => Debug.WriteLine("Window DPI: {0}", (Dpi)e.NewValue)));
        /// <summary>
        /// Dependency property for <see cref="WindowDpi"/>
        /// </summary>
        public static readonly DependencyProperty WindowDpiProperty = WindowDpiPropertyKey.DependencyProperty;

        #endregion


        #region Event

        /// <summary>
        /// DPI changed event
        /// </summary>
        /// <remarks>This event will be fired when DPI of target Window is changed. It is not necessarily 
        /// the same timing when DPI of the monitor to which target Window belongs is changed.</remarks>
        public event EventHandler DpiChanged;

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowHandler()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Target Window</param>
        /// <param name="element">Target FrameworkElement</param>
        public WindowHandler(Window window, FrameworkElement element = null)
            : this()
        {
            Initialize(window, element);
        }

        #endregion


        /// <summary>
        /// Target Window
        /// </summary>
        private Window targetWindow;

        /// <summary>
        /// Target FrameworkElement which will be transformed when DPI changed
        /// </summary>
        /// <remarks>If this FrameworkElement is null, Window.Content will be transformed.</remarks>
        private FrameworkElement targetElement;

        /// <summary>
        /// HwndSource of target Window
        /// </summary>
        private HwndSource targetSource;


        internal void Initialize(Window window, FrameworkElement element = null)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            if (!window.IsInitialized)
                throw new InvalidOperationException("Target Window has not been initialized.");

            targetWindow = window;
            targetElement = element;

            if (IsPerMonitorDpiAware)
            {
                MonitorDpi = DpiChecker.GetDpiFromVisual(targetWindow);

                if (MonitorDpi.Equals(SystemDpi))
                {
                    WindowDpi = SystemDpi;
                }
                else
                {
                    var newInfo = new WindowInfo()
                    {
                        Dpi = MonitorDpi,
                        Width = targetWindow.Width * (double)MonitorDpi.X / SystemDpi.X,
                        Height = targetWindow.Height * (double)MonitorDpi.Y / SystemDpi.Y,
                    };

                    Interlocked.Exchange<WindowInfo>(ref dueInfo, newInfo);

                    ChangeDpi();
                }
            }
            else
            {
                MonitorDpi = SystemDpi;
                WindowDpi = SystemDpi;
            }

            targetSource = PresentationSource.FromVisual(targetWindow) as HwndSource;
            if (targetSource != null)
                targetSource.AddHook(WndProc);
        }

        internal void Close()
        {
            if (targetSource != null)
                targetSource.RemoveHook(WndProc);
        }


        /// <summary>
        /// Information that target Window due to be
        /// </summary>
        private WindowInfo dueInfo;

        /// <summary>
        /// Current status of target Window
        /// </summary>
        private WindowStatus currentStatus = WindowStatus.None;

        /// <summary>
        /// Size of target Window to be the base for calculating due size when DPI changed 
        /// </summary>
        private Size baseSize = Size.Empty;

        /// <summary>
        /// Whether DPI has changed after target Window's location has started to be changed
        /// </summary>
        private bool isDpiChanged = false;

        /// <summary>
        /// Whether target Window's location or size has started to be changed
        /// </summary>
        private bool isEnteredSizeMove = false;

        /// <summary>
        /// Count of WM_MOVE message
        /// </summary>
        private int countLocationChanged = 0;

        /// <summary>
        /// Count of WM_SIZE message
        /// </summary>
        private int countSizeChanged = 0;


        /// <summary>
        /// Handle window messages.
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
            if (!IsPerMonitorDpiAware)
                return IntPtr.Zero;

            switch (msg)
            {
                case (int)WindowMessage.WM_DPICHANGED:
                    var oldDpi = MonitorDpi;
                    MonitorDpi = new Dpi(
                        NativeMacro.GetLoWord((uint)wParam),
                        NativeMacro.GetHiWord((uint)wParam));

                    Debug.WriteLine("DPICHANGED {0} -> {1}", oldDpi.X, MonitorDpi.X);

                    if (MonitorDpi.Equals(oldDpi))
                        break;

                    isDpiChanged = true;

                    var newInfo = new WindowInfo() { Dpi = MonitorDpi };

                    switch (currentStatus)
                    {
                        case WindowStatus.None:
                            var rect = (NativeMethod.RECT)Marshal.PtrToStructure(lParam, typeof(NativeMethod.RECT));

                            newInfo.Width = rect.right - rect.left;
                            newInfo.Height = rect.bottom - rect.top;
                            break;

                        case WindowStatus.LocationChanged:
                            if (baseSize == Size.Empty)
                                baseSize = new Size(targetWindow.Width, targetWindow.Height);

                            baseSize = new Size(
                                baseSize.Width * (double)MonitorDpi.X / oldDpi.X,
                                baseSize.Height * (double)MonitorDpi.Y / oldDpi.Y);

                            newInfo.Size = baseSize;
                            break;

                        case WindowStatus.SizeChanged:
                            // None.
                            break;
                    }

                    Interlocked.Exchange<WindowInfo>(ref dueInfo, newInfo);

                    switch (currentStatus)
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

                case (int)WindowMessage.WM_ENTERSIZEMOVE:
                    Debug.WriteLine("ENTERSIZEMOVE");

                    baseSize = new Size(targetWindow.Width, targetWindow.Height);

                    isDpiChanged = false;
                    isEnteredSizeMove = true;
                    countLocationChanged = 0;
                    countSizeChanged = 0;
                    break;

                case (int)WindowMessage.WM_EXITSIZEMOVE:
                    Debug.WriteLine("EXITSIZEMOVE");

                    isEnteredSizeMove = false;

                    // Last stand!!!
                    if (isDpiChanged && (currentStatus == WindowStatus.LocationChanged))
                    {
                        var lastInfo = new WindowInfo()
                        {
                            Dpi = MonitorDpi,
                            Size = baseSize,
                        };

                        Interlocked.Exchange<WindowInfo>(ref dueInfo, lastInfo);

                        ChangeDpi(WindowStatus.LocationChanged);
                    }

                    currentStatus = WindowStatus.None;
                    break;

                case (int)WindowMessage.WM_MOVE:
                    Debug.WriteLine("MOVE");

                    countLocationChanged++;
                    if (isEnteredSizeMove && (countLocationChanged > countSizeChanged))
                        currentStatus = WindowStatus.LocationChanged;

                    ChangeDpi(WindowStatus.LocationChanged);
                    break;

                case (int)WindowMessage.WM_SIZE:
                    Debug.WriteLine("SIZE");

                    if ((uint)wParam == NativeMethod.SIZE_RESTORED)
                    {
                        countSizeChanged++;
                        if (isEnteredSizeMove && (countLocationChanged < countSizeChanged))
                            currentStatus = WindowStatus.SizeChanged;
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
        private object blocker = null;

        private void ChangeDpi(WindowStatus status = WindowStatus.None)
        {
            if (Interlocked.CompareExchange(ref blocker, new object(), null) != null)
                return;

            try
            {
                var testInfo = Interlocked.Exchange<WindowInfo>(ref dueInfo, null);

                while (testInfo != null)
                {
                    var testRect = new Rect(new Point(targetWindow.Left, targetWindow.Top), testInfo.Size);

                    bool changesNow = true;

                    switch (status)
                    {
                        case WindowStatus.None:
                        case WindowStatus.SizeChanged:
                            // None.
                            break;

                        case WindowStatus.LocationChanged:
                            var testDpi = DpiChecker.GetDpiFromRect(new NativeMethod.RECT(testRect));

                            changesNow = testInfo.Dpi.Equals(testDpi);
                            break;
                    }

                    if (changesNow)
                    {
                        switch (status)
                        {
                            case WindowStatus.None:
                            case WindowStatus.LocationChanged:
                                Debug.WriteLine("Old Size: {0}-{1}", targetWindow.Width, targetWindow.Height);

                                targetWindow.Left = testRect.Left;
                                targetWindow.Top = testRect.Top;
                                targetWindow.Width = testRect.Width;
                                targetWindow.Height = testRect.Height;

                                Debug.WriteLine("New Size: {0}-{1}", targetWindow.Width, targetWindow.Height);
                                break;

                            case WindowStatus.SizeChanged:
                                // None.
                                break;
                        }

                        WindowDpi = testInfo.Dpi;

                        var content = targetElement ?? targetWindow.Content as FrameworkElement;
                        if (content != null)
                        {
                            content.LayoutTransform = (testInfo.Dpi.Equals(SystemDpi))
                                ? Transform.Identity
                                : new ScaleTransform(
                                    (double)WindowDpi.X / SystemDpi.X,
                                    (double)WindowDpi.Y / SystemDpi.Y);
                        }

                        var handler = DpiChanged;
                        if (handler != null)
                            handler(this, EventArgs.Empty);

                        testInfo = Interlocked.Exchange<WindowInfo>(ref dueInfo, null);
                    }
                    else
                    {
                        testInfo = Interlocked.Exchange<WindowInfo>(ref dueInfo, testInfo);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref blocker, null);
            }
        }
    }
}