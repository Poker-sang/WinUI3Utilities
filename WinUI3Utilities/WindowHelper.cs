using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinUI3Utilities.Internal.PlatformInvoke;

namespace WinUI3Utilities;

/// <summary>
/// Helper to get and set properties of <see cref="Window"/>
/// </summary>
public static class WindowHelper
{
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
        AppHelper.GetScreenSize() switch
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
    /// <paramref name="window"/>.AppWindow.Title = <paramref name="info"/>.Title;<br/>
    /// <br/>
    /// <see langword="if"/> (<paramref name="info"/>.Size.HasValue)
    ///     <paramref name="window"/>.AppWindow.Resize(<paramref name="info"/>.Size.Value);<br/>
    /// <see langword="if"/> (<paramref name="info"/>.IconPath <see langword="is not"/> "")
    ///     <paramref name="window"/>.AppWindow.SetIcon(<paramref name="info"/>.IconPath);<br/>
    /// <see langword="else if"/> (<paramref name="info"/>.IconId != <see langword="default"/>)
    ///     <paramref name="window"/>.AppWindow.SetIcon(<paramref name="info"/>.IconId);<br/>
    /// <br/>
    /// // Apply customized title bar when supported<br/>
    /// <see langword="if"/> (<paramref name="info"/>.ExtendTitleBar)<br/>
    ///     <paramref name="window"/>.SetWindowTitleBar();<br/>
    /// <br/>
    /// // Apply backdrop if supported (depends on <see cref="InitializeInfo.BackdropType"/>)<br/>
    /// <see cref="BackdropHelper"/>... 
    /// </code>
    /// </remarks>
    /// <param name="info"></param>
    /// <param name="window"></param>
    public static void Initialize(this Window window, InitializeInfo info)
    {
        window.AppWindow.Title = info.Title;

        if (info.Size != default)
            window.AppWindow.Resize(info.Size);
        if (info.IconPath is not "")
            window.AppWindow.SetIcon(info.IconPath);
        else if (info.IconId != default)
            window.AppWindow.SetIcon(info.IconId);

        if (info.ExtendTitleBar)
            window.SetWindowTitleBar();

        window.SetBackdrop(info.BackdropType);
    }
}
