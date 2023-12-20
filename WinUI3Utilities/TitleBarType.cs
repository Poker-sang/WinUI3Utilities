using System;

namespace WinUI3Utilities;

/// <summary>
/// TitleBar type
/// </summary>
[Flags]
public enum TitleBarType
{
    /// <summary>
    /// Neither
    /// </summary>
    Neither,

    /// <summary>
    /// Use <see cref="TitleBarHelper.SetWindowTitleBar"/>
    /// </summary>
    Window,

    /// <summary>
    /// Use <see cref="TitleBarHelper.SetAppWindowTitleBar"/>
    /// </summary>
    AppWindow,

    /// <summary>
    /// <strong>Use with Caution</strong><br/>
    /// Use both of <see cref="TitleBarHelper.SetWindowTitleBar"/> and <see cref="TitleBarHelper.SetAppWindowTitleBar"/>
    /// </summary>
    Both = Window | AppWindow
}
