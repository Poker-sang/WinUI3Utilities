using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Microsoft.UI;
using WinUI3Utilities.Interfaces;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Application"/>
/// </summary>
public static class AppHelper
{
    /// <summary>
    /// Is it Windows 11 or not?
    /// </summary>
    /// <remarks>Windows 11 starts with 10.0.22000</remarks>
    public static bool IsWindows11 => Environment.OSVersion.Version.Build >= 22000;

    /// <summary>
    /// Info type for <see cref="Initialize"/>
    /// </summary>
    public record InitializeInfo
    {
        /// <summary>
        /// Window size
        /// </summary>
        /// <remarks>
        /// Default: <see langword="default"/>
        /// </remarks>
        public SizeInt32 Size { get; set; }

        /// <summary>
        /// TitleBar type
        /// </summary>
        /// <remarks>
        /// Default: <see cref="TitleBarHelper.TitleBarType.Window"/>
        /// </remarks>
        public TitleBarHelper.TitleBarType TitleBarType { get; set; } = TitleBarHelper.TitleBarType.Window;

        /// <summary>
        /// Backdrop type
        /// </summary>
        /// <remarks>
        /// Default: <see cref="BackdropHelper.BackdropType.MicaAlt"/>
        /// </remarks>
        public BackdropHelper.BackdropType BackdropType { get; set; } = BackdropHelper.BackdropType.MicaAlt;

        /// <summary>
        /// Unhandled exception handler
        /// </summary>
        /// <remarks>
        /// Default: <see langword="null"/>
        /// </remarks>
        public Func<Exception, Task>? UnhandledExceptionHandler { get; set; }

        /// <summary>
        /// Icon path
        /// </summary>
        /// <remarks>
        /// Default: ""
        /// </remarks>
        public string IconPath { get; set; } = "";

        /// <summary>
        /// Icon id
        /// </summary>
        /// <remarks>
        /// Default: <see langword="default"/>
        /// </remarks>
        public IconId IconId { get; set; } = default;
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// <code>
    /// <see cref="RegisterUnhandledExceptionHandler"/><br/>
    /// <br/>
    /// 
    /// <see cref="CurrentContext.App"/>.Resources["NavigationViewContentMargin"] = <see langword="new"/> <see cref="Thickness"/>(0, 48, 0, 0);<br/>
    /// <see cref="IWindowInfo.AppWindow"/>.Title = <see cref="CurrentContext.Title"/>;<br/>
    /// <br/>
    /// <see langword="if"/> (<paramref name="info"/>.Size.HasValue)
    ///     <paramref name="windowInfo"/>.AppWindow.Resize(<paramref name="info"/>.Size.Value);<br/>
    /// <see langword="if"/> (<paramref name="info"/>.IconPath <see langword="is not"/> "")
    ///     <paramref name="windowInfo"/>.AppWindow.SetIcon(<paramref name="info"/>.IconPath);<br/>
    /// <see langword="else if"/> (<paramref name="info"/>.IconId != <see langword="default"/>)
    ///     <paramref name="windowInfo"/>.AppWindow.SetIcon(<paramref name="info"/>.IconId);<br/>
    /// <br/>
    /// <paramref name="windowInfo"/>.AppWindow.Show();<br/>
    /// <br/>
    /// <see cref="TitleBarHelper"/>... // try apply customize title bar (depends on <see cref="InitializeInfo.TitleBarType"/>)<br/>
    /// <see cref="BackdropHelper"/>... // try apply backdrop (depends on <see cref="InitializeInfo.BackdropType"/>)
    /// </code>
    /// </remarks>
    /// <param name="info"></param>
    /// <param name="windowInfo">Default: <see cref="CurrentContext.WindowInfo"/></param>
    /// <param name="titleBar">Default: <see cref="CurrentContext.TitleBar"/></param>
    public static void Initialize(InitializeInfo info, IWindowInfo? windowInfo = null, FrameworkElement? titleBar = null)
    {
        titleBar ??= CurrentContext.TitleBar;
        windowInfo ??= CurrentContext.WindowInfo;
        RegisterUnhandledExceptionHandler(windowInfo.Window, info.UnhandledExceptionHandler);

        CurrentContext.App.Resources["NavigationViewContentMargin"] = new Thickness(0, 48, 0, 0);

        windowInfo.AppWindow.Title = CurrentContext.Title;

        if (info.Size != default)
            windowInfo.AppWindow.Resize(info.Size);
        if (info.IconPath is not "")
            windowInfo.AppWindow.SetIcon(info.IconPath);
        else if (info.IconId != default)
            windowInfo.AppWindow.SetIcon(info.IconId);

        windowInfo.AppWindow.Show();

        if (info.TitleBarType.HasFlag(TitleBarHelper.TitleBarType.Window))
            _ = TitleBarHelper.TryCustomizeTitleBar(windowInfo.Window, titleBar);
        if (info.TitleBarType.HasFlag(TitleBarHelper.TitleBarType.AppWindow))
            _ = TitleBarHelper.TryCustomizeTitleBar(windowInfo.AppTitleBar);

        _ = info.BackdropType switch
        {
            BackdropHelper.BackdropType.None => false,
            BackdropHelper.BackdropType.Acrylic => BackdropHelper.TryApplyAcrylic(windowInfo.Window),
            BackdropHelper.BackdropType.Mica => BackdropHelper.TryApplyMica(windowInfo.Window, false),
            BackdropHelper.BackdropType.MicaAlt => BackdropHelper.TryApplyMica(windowInfo.Window),
            _ => ThrowHelper.ArgumentOutOfRange<BackdropHelper.BackdropType, bool>(info.BackdropType)
        };
    }

    #region DebugHelper

    /// <summary>
    /// Method to register exception handler
    /// </summary>
    /// <remarks>
    /// Registered events:
    /// <list type="bullet">
    /// <item><term><see cref="Application.UnhandledException"/></term></item>
    /// <item><term><see cref="TaskScheduler.UnobservedTaskException"/></term></item>
    /// <item><term><see cref="AppDomain.UnhandledException"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="window"></param>
    /// <param name="func"></param>
    public static void RegisterUnhandledExceptionHandler(Window window, Func<Exception, Task>? func = null)
    {
        func ??= UncaughtExceptionHandler;

        CurrentContext.App.UnhandledException += (o, args) =>
        {
            args.Handled = true;
            _ = window.DispatcherQueue.TryEnqueue(() => func(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += (o, args) =>
        {
            args.SetObserved();
            _ = window.DispatcherQueue.TryEnqueue(() => func(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += (o, args) =>
        {
            if (args.ExceptionObject is Exception e)
                _ = window.DispatcherQueue.TryEnqueue(() => func(e));
            else
                CurrentContext.App.Exit();
        };

        static Task UncaughtExceptionHandler(Exception e)
        {
            Debugger.Break();
            return Task.CompletedTask;
        }
    }

    #endregion
}
