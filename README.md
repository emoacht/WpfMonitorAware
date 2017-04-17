# WPF Monitor Aware Window

A library for WPF Per-Monitor DPI aware and color profile aware window

## Requirements

 * .NET Framework 4.5.2
 * Windows 8.1 or newer to take advantage of Per-Monitor DPI

## Types

| Type                 | Description                                                                                                                                                              |
|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MonitorAwareWindow   | Per-Monitor DPI aware window.                                                                                                                                            |
| MonitorAwareBehavior | Behavior to make a window Per-Monitor DPI aware. This behavior is inherited from System.Windows.Interactivity.Behavior and so requires System.Windows.Interactivity.dll. |
| MonitorAwareProperty | Attached property to make a window Per-Monitor DPI aware.                                                                                                                |
| ExtendedWindow       | Per-Monitor DPI aware and customizable chrome Window. This window is completely Per-Monitor DPI aware including window chrome.                                           |

## Common Properties

| Property                           | Description                                                                                                                                                                                                                                                                                                                                                                      |
|------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| WindowHandler.IsPerMonitorDpiAware | Whether current process is Per-Monitor DPI aware.                                                                                                                                                                                                                                                                                                                                |
| WindowHandler.SystemDpi            | System DPI. This value is used by default to render a Window and other FrameworkElements and so will be the starting point to adjust scaling of FrameworkElements that are once rendered.                                                                                                                                                                                        |
| WindowHandler.MonitorDpi           | Per-Monitor DPI of the monitor to which a Window belongs. This value will be updated when the Window receives WM_DPICHANGED message.                                                                                                                                                                                                                                             |
| WindowHandler.WindowDpi            | Per-Monitor DPI to be used to render a Window. This value will be conformed to Per-Monitor DPI of the monitor when the Window moves to a location where the resized Window belongs to the destination monitor but not the source monitor. There will be a time lag between when WM_DPICHANGED message comes and when this value changes depending on the location of the Window. |
| WindowHandler.ColorProfilePath     | Color profile path used by the monitor to which a Window belongs.                                                                                                                                                                                                                                                                                                                |
| WindowHandler.ForbearScaling       | Whether to forbear scaling.                                                                                                                                                                                                                                                                                                                                                      |
| WillForbearScalingIfUnnecessary    | Whether to forbear scaling if it is unnecessary because built-in scaling is enabled.                                                                                                                                                                                                                                                                                             |

The default DPI of WPF rendering system is 96. To adjust scaling of FrameworkElements by code, you have to carefully select the source DPI and the destination DPI.

## Common Events

| Event                             | Description                                               |
|-----------------------------------|-----------------------------------------------------------|
| WindowHandler.DpiChanged          | Occurs when the WindowDpi is conformed to the MonitorDpi. |
| WindowHandler.ColorProfileChanged | Occurs when the ColorProfilePath is changed.              |

## DPI Awareness

This library has no function to notify OS of DPI awareness of an app. The app using this library needs to declare itself Per-Monitor DPI aware in the application manifest.

## Built-in Scaling

From Windows 10 Anniversary Update (Redstone 1), built-in scaling for WPF is supported.

The prerequisites for built-in scaling are the following:

 - OS is Windows 10 Anniversary Update (Redstone 1) or newer.
 - Target framework of assembly is .NET Framework 4.6.2 or newer.
 - `dpiAwareness` in the application manifest is set to `PerMonitor`.

In addition, if `Switch.System.Windows.DoNotScaleForDpiChanges` is specified in the application configuration, it will have the following effects:

 * True - DISABLE built-in scaling even if the above conditions are met.
 * False - ENABLE built-in scaling even if target framework is older than 4.6.2.

See the Developer Guide in [Microsoft/WPF-Samples/PerMonitorDPI/](https://github.com/Microsoft/WPF-Samples/tree/master/PerMonitorDPI).

To use built-in scaling instead of this library's scaling in an environment where built-in scaling is enabled, set `WillForbearScalingIfUnnecessary` property to True.

## License

 - MIT License
