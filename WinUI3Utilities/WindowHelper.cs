using System.ComponentModel;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using WinUI3Utilities.PlatformInvoke.User32;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinUI3Utilities.PlatformInvoke.Shcore;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Window"/>
/// </summary>
public static class WindowHelper
{
    /// <inheritdoc cref="InitializeWithWindow{T}(T, nint)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="obj"></param>
    /// <returns><paramref name="obj"/></returns>
    public static T InitializeWithWindow<T>(this T obj) => obj.InitializeWithWindow(CurrentContext.HWnd);

    /// <summary>
    /// Call <see cref="WinRT.Interop.InitializeWithWindow.Initialize"/> and return <paramref name="obj"/> itself
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="obj"></param>
    /// <param name="hWnd"></param>
    /// <returns><paramref name="obj"/></returns>
    public static T InitializeWithWindow<T>(this T obj, nint hWnd)
    {
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
    /// 
    /// <summary>
    /// Get Scale Adjustment
    /// </summary>
    /// <returns>scale factor percent</returns>
    /// <exception cref="Win32Exception"/>
    /// <param name="windowId"></param>
    /// <exception cref="Win32Exception"></exception>
    public static double GetScaleAdjustment(WindowId windowId)
    {
        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
        var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        var result = Shcore.GetDpiForMonitor(hMonitor, MonitorDpiType.MdtDefault, out var dpiX, out _);
        if (result != 0)
            throw new Win32Exception("Could not get DPI for monitor.");

        var scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
        return scaleFactorPercent / 100.0;
    }
}
