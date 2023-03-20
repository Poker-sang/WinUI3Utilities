using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using WinUI3Utilities.Interfaces;

namespace WinUI3Utilities.Models;

/// <summary>
/// Default implementation of <see cref="IWindowInfo"/> 
/// </summary>
public record WindowInfo : IWindowInfo
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="window"></param>
    public WindowInfo(Window window)
    {
        Window = window;
        HWnd = WindowNative.GetWindowHandle(Window);
        WindowId = Win32Interop.GetWindowIdFromWindow(HWnd);
        AppWindow = AppWindow.GetFromWindowId(WindowId);
    }

    /// <inheritdoc cref="IWindowInfo.Window"/>
    public Window Window { get; }

    /// <inheritdoc cref="IWindowInfo.HWnd"/>
    public nint HWnd { get; }

    /// <inheritdoc cref="IWindowInfo.WindowId"/>
    public WindowId WindowId { get; }

    /// <inheritdoc cref="IWindowInfo.AppWindow"/>
    public AppWindow AppWindow { get; }

    /// <inheritdoc cref="IWindowInfo.AppTitleBar"/>
    public AppWindowTitleBar AppTitleBar => AppWindow.TitleBar;

    /// <inheritdoc cref="IWindowInfo.AppPresenter"/>
    /// <remarks>
    /// Tips:
    /// <list type="bullet">
    /// <item><term><see cref="AppWindowPresenter.Kind"/></term><description>default value is <see cref="AppWindowPresenterKind.Overlapped"/></description></item>
    /// <item><term><see cref="System.Type"/></term><description>default value is <see cref="Microsoft.UI.Windowing.OverlappedPresenter"/></description></item>
    /// </list>
    /// </remarks>
    public AppWindowPresenter AppPresenter => AppWindow.Presenter;

    /// <inheritdoc cref="IWindowInfo.OverlappedPresenter"/>
    public OverlappedPresenter OverlappedPresenter => AppWindow.Presenter.To<OverlappedPresenter>();
}
