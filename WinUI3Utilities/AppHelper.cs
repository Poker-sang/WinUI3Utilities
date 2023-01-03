using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
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
    /// RegisterUnhandledExceptionHandler();
    /// 
    /// CurrentContext.AppWindow.Title = CurrentContext.Title;
    /// CurrentContext.AppWindow.Resize(size);
    /// CurrentContext.AppWindow.Show();
    /// if (CurrentContext.IconPath is "")
    /// CurrentContext.AppWindow.SetIcon(CurrentContext.IconPath);
    /// 
    /// _ = TitleBarHelper.TryCustomizeTitleBar();
    /// _ = MicaHelper.TryApplyMica();
    /// </code>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// <item><term><see cref="CurrentContext.App"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="size">The size of <see cref="CurrentContext.Window"/></param>
    public static void Initialize(SizeInt32 size)
    {
        RegisterUnhandledExceptionHandler();

        CurrentContext.AppWindow.Title = CurrentContext.Title;
        CurrentContext.AppWindow.Resize(size);
        CurrentContext.AppWindow.Show();
        if (CurrentContext.IconPath is not "")
            CurrentContext.AppWindow.SetIcon(CurrentContext.IconPath);

        _ = TitleBarHelper.TryCustomizeTitleBar();
        _ = MicaHelper.TryApplyMica();
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
    public static void RegisterUnhandledExceptionHandler()
    {
        CurrentContext.App.UnhandledException += async (_, args) =>
        {
            args.Handled = true;
            await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += async (_, args) =>
        {
            args.SetObserved();
            await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += async (_, args) =>
        {
            if (args.ExceptionObject is Exception e)
            {
                await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(e));
            }
            else
            {
                ExitWithPushedNotification();
            }
        };

#if DEBUG
        static Task UncaughtExceptionHandler(Exception e)
        {
            Debugger.Break();
            return Task.CompletedTask;
        }
#elif RELEASE
        Task UncaughtExceptionHandler(Exception e)
        {
            return ShowExceptionDialogAsync(e);
        }
#endif
    }

    /// <summary>
    /// Exit the notification after pushing an <see cref="ApplicationExitingMessage" /> to the <see cref="EventChannel" />
    /// </summary>
    /// <remarks>
    /// Registered events:
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.App"/></term></item>
    /// </list>
    /// </remarks>
    public static void ExitWithPushedNotification()
    {
        _ = WeakReferenceMessenger.Default.Send(new ApplicationExitingMessage());
        CurrentContext.App.Exit();
    }

    /// <summary>
    /// Application exiting message
    /// </summary>
    public record ApplicationExitingMessage;

    #endregion
}
