# WPF Monitor Aware Window

A library for WPF Per-Monitor DPI aware and color profile aware window

## Overview

The introduction of Per-Monitor DPI for WPF is roughly divided into two phases:

1. Per-Monitor DPI was first brought in Windows 8.1. It works with WPF but there is no built-in support for WPF.

2. Built-in Per-Monitor DPI support for WPF was added in .NET Framework 4.6.2 on Windows 10 Anniversary Update (Redstone 1). It includes [DpiScale](https://docs.microsoft.com/en-us/dotnet/api/system.windows.dpiscale) struct which represents DPI scale information and relevant methods and events.

Reference:

- [Microsoft/WPF-Samples/PerMonitorDPI](https://github.com/Microsoft/WPF-Samples/tree/master/PerMonitorDPI)
- [Developing a Per-Monitor DPI-Aware WPF Application](https://docs.microsoft.com/en-us/windows/win32/hidpi/declaring-managed-apps-dpi-aware)

## New Phase

### MonitorAware

Implementation __after__ built-in support for WPF was added. It is designed to provide additional flexibilities for composing views under Per-Monitor DPI.

In the application manifest, DPI awareness must be specified:

```xml
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <!-- Per Monitor V1 [OS >= Windows 8.1] 
         Values: False, True, Per-monitor, True/PM -->
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">
      true/PM</dpiAware>
    <!-- Per Monitor V1 [OS >= Windows 10 Anniversary Update (1607, 10.0.14393, Redstone 1)]
         Values: Unaware, System, PerMonitor -->
    <!-- Per Monitor V2 [OS >= Windows 10 Creators Update (1703, 10.0.15063, Redstone 2)]
         Value: PerMonitorV2 -->
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">
      PerMonitorV2, PerMonitor</dpiAwareness>
  </windowsSettings>
</application>
```

### WpfMonitorAwareBehavior

Sample for implementation by Behavior.

### WpfMonitorAwareProperty

Sample for implementation by attached property.

### SlateElement

Components for DPI-aware window designed after Windows 10 style. It is required for MonitorAware to avoid inconsistent DPI scaling in non-client area but it works independently from MonitorAware.

### WpfSlateWindow

Sample for SlateElement.

### WpfExtendedWindow

Experimental DPI-aware window designed after Windows 8.1 style.

## Old Phase

### MonitorAware.Old

Original implementation __before__ built-in support for WPF was added. It includes following 3 types:

| Type                 | Description                                                                                                                                                              |
|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MonitorAwareWindow   | Per-Monitor DPI aware window.                                                                                                                                            |
| MonitorAwareBehavior | Behavior to make a window Per-Monitor DPI aware. This behavior is inherited from System.Windows.Interactivity.Behavior and so requires System.Windows.Interactivity.dll. |
| MonitorAwareProperty | Attached property to make a window Per-Monitor DPI aware.                                                                                                                |

Properties:

| Property                           | Description                                                                                                                                                                                                                                                                                                                                                                      |
|------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| WindowHandler.IsPerMonitorDpiAware | Whether current process is Per-Monitor DPI aware.                                                                                                                                                                                                                                                                                                                                |
| WindowHandler.SystemDpi            | System DPI. This value is used by default to render a Window and other FrameworkElements and so will be the starting point to adjust scaling of FrameworkElements that are once rendered.                                                                                                                                                                                        |
| WindowHandler.MonitorDpi           | Per-Monitor DPI of the monitor to which a Window belongs. This value will be updated when the Window receives WM_DPICHANGED message.                                                                                                                                                                                                                                             |
| WindowHandler.WindowDpi            | Per-Monitor DPI to be used to render a Window. This value will be conformed to Per-Monitor DPI of the monitor when the Window moves to a location where the resized Window belongs to the destination monitor but not the source monitor. There will be a time lag between when WM_DPICHANGED message comes and when this value changes depending on the location of the Window. |
| WindowHandler.ColorProfilePath     | Color profile path used by the monitor to which a Window belongs.                                                                                                                                                                                                                                                                                                                |
| WindowHandler.ForbearScaling       | Whether to forbear scaling.                                                                                                                                                                                                                                                                                                                                                      |
| WillForbearScalingIfUnnecessary    | Whether to forbear scaling if it is unnecessary because built-in scaling is enabled.                                                                                                                                                                                                                                                                                             |

Events:

| Event                             | Description                                               |
|-----------------------------------|-----------------------------------------------------------|
| WindowHandler.DpiChanged          | Occurs when the WindowDpi is conformed to the MonitorDpi. |
| WindowHandler.ColorProfileChanged | Occurs when the ColorProfilePath is changed.              |

## License

 - MIT License
