using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

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
    /// Use <see cref="TitleBarHelper.TryCustomizeTitleBar(Microsoft.UI.Xaml.Window, FrameworkElement)"/>
    /// </summary>
    Window,

    /// <summary>
    /// Use <see cref="TitleBarHelper.TryCustomizeTitleBar(AppWindowTitleBar)"/>
    /// </summary>
    AppWindow,

    /// <summary>
    /// <strong>Use with Caution</strong><br/>
    /// Use both of <see cref="TitleBarHelper.TryCustomizeTitleBar(Microsoft.UI.Xaml.Window, FrameworkElement)"/> and <see cref="TitleBarHelper.TryCustomizeTitleBar(AppWindowTitleBar)"/>
    /// </summary>
    Both = Window | AppWindow
}
