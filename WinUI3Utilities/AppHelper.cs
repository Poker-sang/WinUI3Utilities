using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinUI3Utilities.Internal.PlatformInvoke;

namespace WinUI3Utilities;

/// <summary>
/// Helper for <see cref="Application"/>
/// </summary>
public static class AppHelper
{
    /// <summary>
    /// This static constructor is needed when calling <see cref="InitializeIsDarkMode"/> to initialize <see cref="IsDarkMode"/>.
    /// </summary>
    static AppHelper()
    {
    }

    /// <summary>
    /// Initialize <see cref="IsDarkMode"/>
    /// </summary>
    public static void InitializeIsDarkMode()
    {
    }

    /// <summary>
    /// Is app dark mode by default.
    /// </summary>
    public static bool IsDarkMode { get; } = Application.Current.RequestedTheme is ApplicationTheme.Dark;

    /// <summary>
    /// Is it Windows 11 or not?
    /// </summary>
    /// <remarks>Windows 11 starts with 10.0.22000</remarks>
    public static bool IsWindows11 => Environment.OSVersion.Version.Build >= 22000;

    /// <summary>
    /// Get the dpi-aware screen size using win32 API, where by "dpi-aware" means that
    /// the result will be divided by the scale factor of the monitor that hosts the app
    /// </summary>
    /// <returns>Screen size</returns>
    public static SizeInt32 GetScreenSize()
        => new(User32.GetSystemMetrics(SystemMetric.CxScreen), User32.GetSystemMetrics(SystemMetric.CyScreen));

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
    public static void RegisterUnhandledExceptionHandler(this Window window, Action<Exception>? func = null)
    {
        func ??= UncaughtExceptionHandler;

        Application.Current.UnhandledException += (o, args) =>
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
                Debugger.Break();
        };

        return;
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        static void UncaughtExceptionHandler(Exception e)
        {
            var inner = e.InnerException;
            Debugger.Break();
        }
    }
}
