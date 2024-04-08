using Microsoft.UI.Composition.SystemBackdrops;
using System.Runtime.InteropServices;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Microsoft.UI.Windowing;
using WinUI3Utilities.Internal.PlatformInvoke;
using Microsoft.UI;

namespace WinUI3Utilities;

/// <summary>
/// Helper to get and set properties of <see cref="Window"/>
/// </summary>
public static class WindowHelper
{
    /// <summary>
    /// Get the dpi-aware screen size using win32 API, whereby "dpi-aware" means that
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
    /// <item><term>(>= 1600, >= 900)</term><description>(1280, 720)</description></item>
    /// <item><term>others</term><description>(800, 600)</description></item>
    /// </list>
    /// </remarks>
    /// </returns>
    public static SizeInt32 EstimatedWindowSize() =>
        AppHelper.GetScreenSize() switch
        {
            { Width: >= 2560, Height: >= 1440 } => new(1600, 900),
            { Width: >= 1600, Height: >= 900 } => new(1280, 720),
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
    /// // Apply maximization (depends on <see cref="InitializeInfo.IsMaximized"/>)<br/>
    /// // Apply backdrop if supported (depends on <see cref="InitializeInfo.BackdropType"/>)<br/>
    /// // Apply theme (depends on <see cref="InitializeInfo.Theme"/>)<br/>
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

        if (info.IsMaximized && window.AppWindow.Presenter is OverlappedPresenter presenter)
            presenter.Maximize();

        window.SetBackdrop(info.BackdropType, info.Theme is ElementTheme.Dark);

        window.SetTheme(info.Theme);
    }

    /// <summary>
    /// Set <see cref="Window.SystemBackdrop"/> if supported.
    /// Otherwise, set to <see langword="null"/>.
    /// </summary>
    /// <param name="window"></param>
    /// <param name="backdropType"></param>
    /// <param name="isDarkMode">Set <see cref="DefaultBrushBackdrop.IsDarkMode"/>, use <paramref name="window"/>.Content.ActualTheme when <see langword="null"/></param>
    public static void SetBackdrop(this Window window, BackdropType backdropType, bool? isDarkMode = null)
    {
        window.SystemBackdrop = backdropType switch
        {
            BackdropType.Acrylic when DesktopAcrylicController.IsSupported() => new DesktopAcrylicBackdrop(),
            BackdropType.Mica when MicaController.IsSupported() => new MicaBackdrop(),
            BackdropType.MicaAlt when MicaController.IsSupported() => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Maintain => window.SystemBackdrop,
            _ => new DefaultBrushBackdrop(isDarkMode ?? window.Content?.To<FrameworkElement>().ActualTheme is ElementTheme.Dark)
        };
    }

    /// <summary>
    /// Set theme for <paramref name="window"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="theme"></param>
    public static void SetTheme(this Window window, ElementTheme theme)
    {
        var actualTheme = theme switch
        {
            ElementTheme.Default => AppHelper.IsDarkMode ? ElementTheme.Dark : ElementTheme.Light,
            _ => theme
        };

        if (window.Content is FrameworkElement framework)
        {
            framework.RequestedTheme = actualTheme;
            if (window.SystemBackdrop is DefaultBrushBackdrop backdrop) 
                backdrop.IsDarkMode = actualTheme is ElementTheme.Dark;
        }

        window.AppWindow.TitleBar.ButtonForegroundColor = actualTheme is ElementTheme.Dark ? Colors.White : Colors.Black;
    }

    private static void EnsureDispatcherQueueController()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() is null && _dispatcherQueueController is 0)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2; // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            _ = CoreMessaging.CreateDispatcherQueueController(options, out _dispatcherQueueController);
        }
    }

    private static nint _dispatcherQueueController;

    private static Windows.UI.Composition.Compositor? _compositor;

    private static readonly object _compositorLock = new();

    internal static Windows.UI.Composition.Compositor Compositor
    {
        get
        {
            if (_compositor is null)
                lock (_compositorLock)
                    if (_compositor is null)
                    {
                        EnsureDispatcherQueueController();
                        _compositor = new();
                    }

            return _compositor;
        }
    }
}
