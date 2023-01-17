using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.Graphics;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Application"/>
/// </summary>
public static class AppHelper
{
    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <returns></returns>
    public static async Task BeforeLaunch()
    {
        // AppInstance.GetCurrent().Activated += (_, arguments) => ActivationRegistrar.Dispatch(arguments);
        // InitializeComponent();
        if (AppInstance.GetCurrent().GetActivatedEventArgs().Kind is ExtendedActivationKind.ToastNotification)
            return;

        var isProtocolActivated = AppInstance.GetCurrent().GetActivatedEventArgs() is { Kind: ExtendedActivationKind.Protocol };
        if (isProtocolActivated && AppInstance.GetInstances().Count > 1)
        {
            var notCurrent = AppInstance.GetInstances().First(ins => !ins.IsCurrent);
            await notCurrent.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
        }
    }

    /// <summary>
    /// Is it Windows 11 or not?
    /// </summary>
    /// <remarks>Windows 11 starts with 10.0.22000</remarks>
    public static bool IsWindows11 => Environment.OSVersion.Version.Build >= 22000;

    /// <summary>
    /// <code>
    /// <see cref="RegisterUnhandledExceptionHandler"/><br/>
    /// <br/>
    /// <see cref="CurrentContext.AppWindow"/>.Title = <see cref="CurrentContext.Title"/>;<br/>
    /// <see cref="CurrentContext.AppWindow"/>.Resize(<paramref name="size"/>);<br/>
    /// <see cref="CurrentContext.AppWindow"/>.Show();
    /// if (<see cref="CurrentContext.IconPath"/> <see langword="is"/> "")
    ///     <see cref="CurrentContext.AppWindow"/>.SetIcon(<see cref="CurrentContext.IconPath"/>);
    /// 
    /// _ = <see cref="TitleBarHelper.TryCustomizeTitleBar"/>;<br/>
    /// <see cref="BackdropHelper"/>... // try apply backdrop (depends on <paramref name="backdropType"/>)
    /// </code>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.App"/></term></item>
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="size">The size of <see cref="CurrentContext.Window"/></param>
    /// <param name="backdropType">Backdrop of <see cref="CurrentContext.Window"/></param>
    /// <param name="handler">Unhandled exception handler</param>
    public static void Initialize(SizeInt32 size, BackdropHelper.BackdropType backdropType = BackdropHelper.BackdropType.MicaAlt, Func<Exception, Task>? handler = null)
    {
        RegisterUnhandledExceptionHandler(handler);

        CurrentContext.AppWindow.Title = CurrentContext.Title;
        CurrentContext.AppWindow.Resize(size);
        CurrentContext.AppWindow.Show();
        if (CurrentContext.IconPath is not "")
            CurrentContext.AppWindow.SetIcon(CurrentContext.IconPath);

        _ = TitleBarHelper.TryCustomizeTitleBar();
        switch (backdropType)
        {
            case BackdropHelper.BackdropType.None:
                break;
            case BackdropHelper.BackdropType.Acrylic:
                _ = BackdropHelper.TryApplyAcrylic();
                break;
            case BackdropHelper.BackdropType.Mica:
                _ = BackdropHelper.TryApplyMica(false);
                break;
            case BackdropHelper.BackdropType.MicaAlt:
                _ = BackdropHelper.TryApplyMica();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backdropType), backdropType, null);
        }
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
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.App"/></term></item>
    /// </list>
    /// </remarks>
    public static void RegisterUnhandledExceptionHandler(Func<Exception, Task>? func = null)
    {
        func ??= UncaughtExceptionHandler;

        CurrentContext.App.UnhandledException += (o, args) =>
        {
            args.Handled = true;
            _ = CurrentContext.Window.DispatcherQueue.TryEnqueue(() => func(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += (o, args) =>
        {
            args.SetObserved();
            _ = CurrentContext.Window.DispatcherQueue.TryEnqueue(() => func(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += (o, args) =>
        {
            if (args.ExceptionObject is Exception e)
                _ = CurrentContext.Window.DispatcherQueue.TryEnqueue(() => func(e));
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
