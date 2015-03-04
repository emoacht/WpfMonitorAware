WPF Monitor Aware Window
========================

A library for WPF Per-Monitor DPI aware and color profile aware window

##Requirements

 * .NET Framework 4.5
 * Windows 8.1 or newer to take advantage of Per-Monitor DPI

##Types

| Type                 | Description                                                                                                                                                              |
|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MonitorAwareWindow   | Per-Monitor DPI aware window.                                                                                                                                            |
| MonitorAwareBehavior | Behavior to make a window Per-Monitor DPI aware. This behavior is inherited from System.Windows.Interactivity.Behavior and so requires System.Windows.Interactivity.dll. |
| MonitorAwareProperty | Attached property to make a window Per-Monitor DPI aware.                                                                                                                |
| ExtendedWindow       | Per-Monitor DPI aware and customizable chrome Window. This window is completely Per-Monitor DPI aware including window chrome.                                           |

##Common Properties

| Property                           | Description                                                                                                                                                                                                                                                                                                                                                                                             |
|------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| WindowHandler.IsPerMonitorDpiAware | Whether current process is Per-Monitor DPI aware.                                                                                                                                                                                                                                                                                                                                                       |
| WindowHandler.SystemDpi            | System DPI. This value is used by default to render a Window and other FrameworkElements and so will be the base to adjust scaling of FrameworkElements that are once rendered.                                                                                                                                                                                                                         |
| WindowHandler.MonitorDpi           | Per-Monitor DPI of the monitor to which a Window belongs. This value will be updated when the Window receives WM_DPICHANGED message.                                                                                                                                                                                                                                                                    |
| WindowHandler.WindowDpi            | Per-Monitor DPI which is used to render a Window. This value will be conformed to Per-Monitor DPI of the monitor when the Window moves to a location where the resized Window will belong to the destination monitor but not the source monitor. There will be a time lag between the time when this conformation happens and when WM_DPICHANGED message comes depending on the location of the Window. |
| WindowHandler.ColorProfilePath     | Color profile path used by the monitor to which a Window belongs.                                                                                                                                                                                                                                                                                                                                       |

The default DPI of WPF rendering system is 96. To adjust scaling of FrameworkElements by code, you have to carefully select the source DPI and the destination DPI.

##Common Events

| Event                             | Description                                               |
|-----------------------------------|-----------------------------------------------------------|
| WindowHandler.DpiChanged          | Occurs when the WindowDpi is conformed to the MonitorDpi. |
| WindowHandler.ColorProfileChanged | Occurs when the ColorProfilePath is changed.              |

##Note

This library has no function to notify OS of DPI awareness of the app. The app using this library needs to declare itself Per-Monitor DPI aware in the application manifest.

##License

 - MIT License
