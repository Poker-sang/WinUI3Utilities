using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using WinUI3Utilities.Internal.PlatformInvoke;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Window"/>
/// </summary>
public static class WindowHelper
{
    /// <summary>
    /// Call <see cref="WinRT.Interop.InitializeWithWindow.Initialize"/> and return <paramref name="obj"/> itself
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="obj"></param>
    /// <param name="window">Default: <see cref="CurrentContext.Window"/></param>
    /// <returns><paramref name="obj"/></returns>
    public static T InitializeWithWindow<T>(this T obj, Window? window = null)
    {
        var hWnd = (nint)(window ?? CurrentContext.Window).AppWindow.Id.Value;
        // When running on win32, FileOpenPicker needs to know the top-level hWnd via IInitializeWithWindow::Initialize.
        WinRT.Interop.InitializeWithWindow.Initialize(obj, hWnd);
        return obj;
    }

    /// <summary>
    /// Get the dpi-aware screen size using win32 API, where by "dpi-aware" means that
    /// the result will be divided by the scale factor of the monitor that hosts the app
    /// </summary>
    /// <returns>Screen size</returns>
    public static (int, int) GetScreenSize()
        => (User32.GetSystemMetrics(SystemMetric.CxScreen), User32.GetSystemMetrics(SystemMetric.CyScreen));

    /// <summary>
    /// Calculate the window size by current screen size
    /// </summary>
    /// <returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><term>(>= 2560, >= 1440)</term><description>(1600, 900)</description></item>
    /// <item><term>(> 1600, > 900)</term><description>(1280, 720)</description></item>
    /// <item><term>others</term><description>(800, 600)</description></item>
    /// </list>
    /// </remarks>
    /// </returns>
    public static SizeInt32 EstimatedWindowSize() =>
        GetScreenSize() switch
        {
            ( >= 2560, >= 1440) => new(1600, 900),
            ( > 1600, > 900) => new(1280, 720),
            _ => new(800, 600)
        };

    /// <summary>
    /// Get DPI
    /// </summary>
    /// <param name="window"></param>
    /// <returns>DPI</returns>
    public static double GetDpi(this Window window) => User32.GetDpiForWindow((nint)window.AppWindow.Id.Value);

    /// <summary>
    /// Get DPI
    /// </summary>
    /// <param name="window"></param>
    /// <returns>DPI</returns>
    public static double GetDpi(nint window) => User32.GetDpiForWindow(window);

    /// <summary>
    /// Get Scale Adjustment
    /// </summary>
    /// <param name="window"></param>
    /// <returns>scale factor percent</returns>
    public static double GetScaleAdjustment(this Window window) => GetDpi(window) / 96d;

    /// <summary>
    /// Get Scale Adjustment
    /// </summary>
    /// <param name="window"></param>
    /// <returns>scale factor percent</returns>
    public static double GetScaleAdjustment(nint window) => User32.GetDpiForWindow(window) / 96d;

    /// <summary>
    /// </summary>
    /// <remarks>
    /// <code>
    /// <paramref name="window"/>.AppWindow.Title = <paramref name="title"/>;<br/>
    /// <br/>
    /// <see langword="if"/> (<paramref name="info"/>.Size.HasValue)
    ///     <paramref name="window"/>.AppWindow.Resize(<paramref name="info"/>.Size.Value);<br/>
    /// <see langword="if"/> (<paramref name="info"/>.IconPath <see langword="is not"/> "")
    ///     <paramref name="window"/>.AppWindow.SetIcon(<paramref name="info"/>.IconPath);<br/>
    /// <see langword="else if"/> (<paramref name="info"/>.IconId != <see langword="default"/>)
    ///     <paramref name="window"/>.AppWindow.SetIcon(<paramref name="info"/>.IconId);<br/>
    /// <br/>
    /// // try apply customize title bar (depends on <see cref="InitializeInfo.TitleBarType"/>)<br/>
    /// <see cref="TitleBarHelper"/>... 
    /// // try apply backdrop (depends on <see cref="InitializeInfo.BackdropType"/>)<br/>
    /// <paramref name="window"/>.Window.SystemBackdrop = <paramref name="info"/>.BackdropType <see langword="switch"/> { ... }
    /// // try set minimum size and maximum size of the window
    /// _ = SetWindowSubclass(...);
    /// </code>
    /// </remarks>
    /// <param name="info"></param>
    /// <param name="window"></param>
    /// <param name="title"></param>
    /// <param name="titleBar"></param>
    public static void Initialize(this Window window, InitializeInfo info, string title = "", FrameworkElement? titleBar = null)
    {
        window.AppWindow.Title = title;

        if (info.Size != default)
            window.AppWindow.Resize(info.Size);
        if (info.IconPath is not "")
            window.AppWindow.SetIcon(info.IconPath);
        else if (info.IconId != default)
            window.AppWindow.SetIcon(info.IconId);

        if (info.TitleBarType.HasFlag(TitleBarType.Window) && titleBar is not null)
            TitleBarHelper.TryCustomizeTitleBar(window, titleBar);
        if (info.TitleBarType.HasFlag(TitleBarType.AppWindow))
            TitleBarHelper.TryCustomizeTitleBar(window.AppWindow.TitleBar);

        window.SystemBackdrop = info.BackdropType switch
        {
            BackdropType.None => null,
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            BackdropType.Mica => new MicaBackdrop(),
            BackdropType.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Maintain => window.SystemBackdrop,
            _ => ThrowHelper.ArgumentOutOfRange<BackdropType, SystemBackdrop>(info.BackdropType)
        };
    }
}
