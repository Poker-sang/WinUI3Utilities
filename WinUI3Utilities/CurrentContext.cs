using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;

namespace WinUI3Utilities;

/// <summary>
/// Current context instances
/// </summary>
public static class CurrentContext
{
    private static Window _window = null!;

    /// <summary>
    /// App instance
    /// </summary>
    public static Application App => Application.Current;

    /// <summary>
    /// Main window instance
    /// </summary>
    public static Window Window
    {
        get => _window;
        set
        {
            _window = value;
            HWnd = WindowNative.GetWindowHandle(_window);
            WindowId = Win32Interop.GetWindowIdFromWindow(HWnd);
            AppWindow = AppWindow.GetFromWindowId(WindowId);
        }
    }

    /// <summary>
    /// The handle of <see cref="Window"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static nint HWnd { get; private set; }

    /// <summary>
    /// The id of <see cref="Window"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static WindowId WindowId { get; private set; }

    /// <summary>
    /// The app window of <see cref="Window"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static AppWindow AppWindow { get; private set; } = null!;

    /// <summary>
    /// The title bar of <see cref="AppWindow"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static AppWindowTitleBar AppTitleBar => AppWindow.TitleBar;

    /// <summary>
    /// The presenter of <see cref="AppWindow"/>
    /// </summary>
    /// <remarks>
    /// Tips:
    /// <list type="bullet">
    /// <item><term><see cref="AppWindowPresenter.Kind"/></term><description>default value is <see cref="AppWindowPresenterKind.Overlapped"/></description></item>
    /// <item><term><see cref="System.Type"/></term><description>default value is <see cref="Microsoft.UI.Windowing.OverlappedPresenter"/></description></item>
    /// </list>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static AppWindowPresenter AppPresenter => AppWindow.Presenter;

    /// <summary>
    /// A <strong>FORCED CAST</strong> of <see cref="AppPresenter"/>, equals to (<see cref="Microsoft.UI.Windowing.OverlappedPresenter"/>)<see cref="AppPresenter"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    public static OverlappedPresenter OverlappedPresenter => AppWindow.Presenter.To<OverlappedPresenter>();

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
