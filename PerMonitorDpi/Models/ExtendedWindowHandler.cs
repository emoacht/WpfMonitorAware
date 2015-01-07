using System;

using PerMonitorDpi.Models.Win32;

namespace PerMonitorDpi.Models
{
    /// <summary>
    /// Handler for <see cref="PerMonitorDpi.Views.ExtendedWindow"/>
    /// </summary>
    public class ExtendedWindowHandler : WindowHandler
    {
        #region Event

        /// <summary>
        /// Window activated changed event
        /// </summary> 
        /// <remarks>
        /// Args true:  Window is being activated.
        /// Args false: Window is being deactivated.
        /// </remarks>
        internal event EventHandler<bool> WindowActivatedChanged;

        /// <summary>
        /// DWM colorization color changed event
        /// </summary>
        /// <remarks>This event will be fired when default Window chrome color is changed.</remarks>
        internal event EventHandler DwmColorizationColorChanged;

        #endregion


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
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)WindowMessage.WM_ACTIVATE:
                    {
                        var handler = WindowActivatedChanged;
                        if (handler == null)
                            break;

                        switch (wParam.ToInt32())
                        {
                            case NativeMethod.WA_ACTIVE:
                            case NativeMethod.WA_CLICKACTIVE:
                                handler(this, true); // Activated.
                                break;

                            case NativeMethod.WA_INACTIVE:
                                handler(this, false); // Deactivated.
                                break;
                        }
                    }
                    break;

                case (int)WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED:
                    {
                        var handler = DwmColorizationColorChanged;
                        if (handler != null)
                            handler(this, EventArgs.Empty);
                    }
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}