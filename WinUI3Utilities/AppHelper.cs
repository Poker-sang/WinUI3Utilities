using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

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
}
