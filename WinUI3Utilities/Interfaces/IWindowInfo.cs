using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Interfaces;

/// <summary>
/// Interface for window info
/// </summary>
public interface IWindowInfo
{
    /// <summary>
    /// Main window instance
    /// </summary>
    public Window Window { get; }

    /// <summary>
    /// The handle of <see cref="Window"/>
    /// </summary>
    public nint HWnd { get; }

    /// <summary>
    /// The handle of <see cref="Window"/>
    /// </summary>
    public WindowId WindowId { get; }

    /// <summary>
    /// The app window of <see cref="Window"/>
    /// </summary>
    public AppWindow AppWindow { get; }

    /// <summary>
    /// The title bar of <see cref="AppWindow"/>
    /// </summary>
    public AppWindowTitleBar AppTitleBar { get; }

    /// <summary>
    /// The presenter of <see cref="AppWindow"/>
    /// </summary>
    public AppWindowPresenter AppPresenter { get; }

    /// <summary>
    /// A <strong>FORCED CAST</strong> of <see cref="AppPresenter"/>, equals to (<see cref="Microsoft.UI.Windowing.OverlappedPresenter"/>)<see cref="AppPresenter"/>
    /// </summary>
    public OverlappedPresenter OverlappedPresenter { get; }
}
