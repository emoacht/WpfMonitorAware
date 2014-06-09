WPF Per-Monitor DPI Aware Window
================================

A library for WPF Per-Monitor DPI Aware Window

##Requirements

 * .NET Framework 4.5
 * Windows 8.1 or newer to take advantage of Per-Monitor DPI

##Properties (Dependency Properties)

 - SystemDpi: System DPI. This value is used by default to render a Window and other FrameworkElements and so will be the base to adjust scaling of FrameworkElements that are once rendered.

 - MonitorDpi: Per-Monitor DPI of the monitor to which a Window belongs. This value will be updated when the Window receives WM_DPICHANGED message.

 - WindowDpi: Per-Monitor DPI which is used to render a Window. This value will be conformed to Per-Monitor DPI of the monitor when the Window moves to a location where the resized Window will belong to the destination monitor but not the source monitor. There will be a time lag between the time when this conformation happens and when WM_DPICHANGED message comes depending on the location of the Window.

The default DPI of WPF rendering system is 96. To adjust scaling of FrameworkElements by code, you have to carefully select the source DPI and the destination DPI.

##Event

 - DpiChanged: This event will be fired when the WindowDpi is conformed to the MonitorDpi.

##Other

 - License: MIT License