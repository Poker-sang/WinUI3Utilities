using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Interfaces;

namespace WinUI3Utilities;

/// <summary>
/// Current context instances
/// </summary>
public static class CurrentContext
{
    /// <summary>
    /// Window info
    /// </summary>
    public static IWindowInfo WindowInfo { get; set; } = null!;

    /// <summary>
    /// App instance
    /// </summary>
    public static Application App => Application.Current;

    /// <inheritdoc cref="IWindowInfo.Window"/>
    public static Window Window => WindowInfo.Window;

    /// <inheritdoc cref="IWindowInfo.HWnd"/>
    public static nint HWnd => WindowInfo.HWnd;

    /// <inheritdoc cref="IWindowInfo.WindowId"/>
    public static WindowId WindowId => WindowInfo.WindowId;

    /// <inheritdoc cref="IWindowInfo.AppWindow"/>
    public static AppWindow AppWindow => WindowInfo.AppWindow;

    /// <inheritdoc cref="IWindowInfo.AppTitleBar"/>
    public static AppWindowTitleBar AppTitleBar => WindowInfo.AppTitleBar;

    /// <inheritdoc cref="IWindowInfo.AppPresenter"/>
    public static AppWindowPresenter AppPresenter => WindowInfo.AppPresenter;

    /// <inheritdoc cref="IWindowInfo.OverlappedPresenter"/>
    public static OverlappedPresenter OverlappedPresenter => WindowInfo.OverlappedPresenter;

    /// <summary>
    /// The title of <see cref="Window"/>
    /// </summary>
    public static string Title { get; set; } = "";

    /// <summary>
    /// The title bar of <see cref="Window"/>
    /// </summary>
    public static FrameworkElement TitleBar { get; set; } = null!;

    /// <summary>
    /// The text block of <see cref="TitleBar"/>
    /// </summary>
    public static TextBlock TitleTextBlock { get; set; } = null!;

    /// <summary>
    /// The root <see cref="Microsoft.UI.Xaml.Controls.NavigationView"/> instance
    /// </summary>
    public static NavigationView NavigationView { get; set; } = null!;

    /// <summary>
    /// The root <see cref="Microsoft.UI.Xaml.Controls.Frame"/> instance, content of <see cref="NavigationView"/>
    /// </summary>
    public static Frame Frame { get; set; } = null!;
}
