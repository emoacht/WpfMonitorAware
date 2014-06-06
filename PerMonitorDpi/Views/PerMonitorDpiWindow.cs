using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using PerMonitorDpi.Helper;
using PerMonitorDpi.Models;
using PerMonitorDpi.Models.Win32;

namespace PerMonitorDpi.Views
{
	/// <summary>
	/// Per-Monitor DPI Aware Window
	/// </summary>
	public class PerMonitorDpiWindow : Window
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
		/// System DPI
		/// </summary>
		public Dpi SystemDpi
		{
			get { return (Dpi)GetValue(SystemDpiProperty); }
			set { SetValue(SystemDpiProperty, value); }
		}
		public static readonly DependencyProperty SystemDpiProperty =
			DependencyProperty.Register("SystemDpi", typeof(Dpi), typeof(PerMonitorDpiWindow), new FrameworkPropertyMetadata(Dpi.Default));

		/// <summary>
		/// Current Per-Monitor DPI
		/// </summary>
		public Dpi CurrentDpi
		{
			get { return (Dpi)GetValue(CurrentDpiProperty); }
			set { SetValue(CurrentDpiProperty, value); }
		}
		public static readonly DependencyProperty CurrentDpiProperty =
			DependencyProperty.Register("CurrentDpi", typeof(Dpi), typeof(PerMonitorDpiWindow), new FrameworkPropertyMetadata(Dpi.Default));

		#endregion


		#region Event

		/// <summary>
		/// DPI changed event
		/// </summary>
		/// <remarks>This event will be fired when DPI of this Window is changed. It is not necessarily 
		/// the same timing when DPI of the monitor to which this Window belongs is changed.</remarks>
		public event EventHandler DpiChanged;

		#endregion


		/// <summary>
		/// HwndSource of this Window
		/// </summary>
		private HwndSource source;

		/// <summary>
		/// Information that this Window due to be
		/// </summary>
		private WindowInfo dueInfo;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			SystemDpi = DpiChecker.GetSystemDpi(this);

			if (!OsVersion.IsEightOneOrNewer)
			{
				CurrentDpi = SystemDpi;
				return;
			}

			CurrentDpi = DpiChecker.GetDpiFromVisual(this);

			if (!SystemDpi.Equals(CurrentDpi))
			{
				var newInfo = new WindowInfo()
				{
					Dpi = CurrentDpi,
					Width = this.Width * CurrentDpi.X / SystemDpi.X,
					Height = this.Height * CurrentDpi.Y / SystemDpi.Y,
				};

				Interlocked.Exchange<WindowInfo>(ref dueInfo, newInfo);

				ChangeDpi();
			}

			this.source = PresentationSource.FromVisual(this) as HwndSource;
			if (this.source != null)
				this.source.AddHook(WndProc);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (this.source != null)
				this.source.RemoveHook(WndProc);
		}

		/// <summary>
		/// Current status of this Window
		/// </summary>
		private WindowStatus currentStatus = WindowStatus.None;

		/// <summary>
		/// Size of this Window to be the base for calculating due size when DPI changed 
		/// </summary>
		private Size baseSize = Size.Empty;

		/// <summary>
		/// Whether DPI has changed after this Window's location is started to be changed
		/// </summary>
		private bool isDpiChanged = false;

		/// <summary>
		/// Whether this Window's location or size has started to be changed.
		/// </summary>
		private bool isEnteredSizeMove = false;

		/// <summary>
		/// Number of counts of WM_MOVE message
		/// </summary>
		private int countLocationChanged = 0;

		/// <summary>
		/// Number of counts of WM_SIZE message
		/// </summary>
		private int countSizeChanged = 0;

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case (int)WindowMessage.WM_DPICHANGED:
					var oldDpi = CurrentDpi;
					CurrentDpi = new Dpi(
						NativeMacro.GetLoWord((uint)wParam),
						NativeMacro.GetHiWord((uint)wParam));

					Debug.WriteLine(String.Format("DPICHANGED {0} -> {1}", oldDpi.X, CurrentDpi.X));

					if (CurrentDpi.Equals(oldDpi))
						break;

					isDpiChanged = true;

					var newInfo = new WindowInfo() { Dpi = CurrentDpi };

					switch (currentStatus)
					{
						case WindowStatus.None:
							var rect = (NativeMethod.RECT)Marshal.PtrToStructure(lParam, typeof(NativeMethod.RECT));

							newInfo.Width = rect.right - rect.left;
							newInfo.Height = rect.bottom - rect.top;
							break;

						case WindowStatus.LocationChanged:
							if (baseSize == Size.Empty)
								baseSize = new Size(this.Width, this.Height);

							baseSize = new Size(
								baseSize.Width * (double)CurrentDpi.X / oldDpi.X,
								baseSize.Height * (double)CurrentDpi.Y / oldDpi.Y);

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

					baseSize = new Size(this.Width, this.Height);

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
							Dpi = CurrentDpi,
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

					if ((uint)wParam == 0) // SIZE_RESTORED
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
		/// Null: Don't block.
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
					var testRect = new Rect(new Point(this.Left, this.Top), testInfo.Size);

					bool willChange = true;

					switch (status)
					{
						case WindowStatus.None:
						case WindowStatus.SizeChanged:
							// None.
							break;

						case WindowStatus.LocationChanged:
							var testDpi = DpiChecker.GetDpiFromRect(new NativeMethod.RECT(testRect));

							willChange = testInfo.Dpi.Equals(testDpi);
							break;
					}

					if (willChange)
					{
						switch (status)
						{
							case WindowStatus.None:
							case WindowStatus.LocationChanged:
								Debug.WriteLine(String.Format("Old Size: {0}-{1}", this.Width, this.Height));

								this.Left = testRect.Left;
								this.Top = testRect.Top;
								this.Width = testRect.Width;
								this.Height = testRect.Height;

								Debug.WriteLine(String.Format("New Size: {0}-{1}", this.Width, this.Height));
								break;

							case WindowStatus.SizeChanged:
								// None.
								break;
						}

						var content = this.Content as FrameworkElement;
						if (content != null)
						{
							content.LayoutTransform = (testInfo.Dpi.Equals(SystemDpi))
								? Transform.Identity
								: new ScaleTransform(
									(double)testInfo.Dpi.X / SystemDpi.X,
									(double)testInfo.Dpi.Y / SystemDpi.Y);
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
