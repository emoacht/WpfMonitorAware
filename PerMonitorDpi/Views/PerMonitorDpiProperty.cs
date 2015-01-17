using System;
using System.Windows;

using PerMonitorDpi.Models;

namespace PerMonitorDpi.Views
{
    /// <summary>
    /// Attached property to make a <see cref="Window"/> Per-Monitor DPI aware
    /// </summary>
    public class PerMonitorDpiProperty : Freezable
    {
        #region Freezable member

        /// <summary>
        /// Implement <see cref="Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>New Freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new PerMonitorDpiProperty();
        }

        #endregion


        /// <summary>
        /// Get AttachedProperty.
        /// </summary>
        /// <param name="window">Owner <see cref="Window"/></param>
        /// <returns>AttachedProperty</returns>
        public static PerMonitorDpiProperty GetAttachedProperty(Window window)
        {
            return (PerMonitorDpiProperty)window.GetValue(AttachedPropertyProperty);
        }
        /// <summary>
        /// Set AttachedProperty.
        /// </summary>
        /// <param name="window">Owner <see cref="Window"/></param>
        /// <param name="attachedProperty">AttachedProperty</param>
        public static void SetAttachedProperty(Window window, PerMonitorDpiProperty attachedProperty)
        {
            window.SetValue(AttachedPropertyProperty, attachedProperty);
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