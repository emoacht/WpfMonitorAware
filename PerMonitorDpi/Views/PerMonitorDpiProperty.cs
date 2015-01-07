using System;
using System.Windows;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
    /// <summary>
    /// Attached property to make a <see cref="Window"/> Per-Monitor DPI aware
    /// </summary>
    public class PerMonitorDpiProperty : DependencyObject
    {
        /// <summary>
        /// Get AttachedProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PerMonitorDpiProperty GetAttachedProperty(DependencyObject obj)
        {
            return (PerMonitorDpiProperty)obj.GetValue(AttachedPropertyProperty);
        }
        /// <summary>
        /// Set AttachedProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAttachedProperty(DependencyObject obj, PerMonitorDpiProperty value)
        {
            obj.SetValue(AttachedPropertyProperty, value);
        }
        /// <summary>
        /// Attached property for AttachedProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyProperty =
            DependencyProperty.RegisterAttached(
                "AttachedProperty",
                typeof(PerMonitorDpiProperty),
                typeof(PerMonitorDpiProperty),
                new FrameworkPropertyMetadata(
                    null,
                    (d, e) =>
                    {
                        var window = d as Window;
                        if (window == null)
                            return;

                        ((PerMonitorDpiProperty)e.NewValue).OwnerWindow = window;
                    }));

        private Window OwnerWindow
        {
            get { return _ownerWindow; }
            set
            {
                _ownerWindow = value;

                _ownerWindow.Loaded += OnWindowLoaded;
                _ownerWindow.Closed += OnWindowClosed;
            }
        }
        private Window _ownerWindow;

        /// <summary>
        /// Handler for <see cref="Window"/>
        /// </summary>
        public WindowHandler WindowHandler
        {
            get { return _windowhandler ?? (_windowhandler = new WindowHandler()); }
        }
        private WindowHandler _windowhandler;

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            OwnerWindow.Loaded -= OnWindowLoaded;

            WindowHandler.Initialize(OwnerWindow);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            OwnerWindow.Closed -= OnWindowClosed;

            WindowHandler.Close();
        }
    }
}