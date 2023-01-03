using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using WinRT;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Window"/>
/// </summary>
public class MicaHelper
{
    /// <summary>
    /// Backdrop controller
    /// </summary>
    public static MicaController? BackdropController { get; private set; }

    /// <summary>
    /// System backdrop configuration
    /// </summary>
    public static SystemBackdropConfiguration? SystemBackdropConfiguration { get; private set; }

    private static WindowsSystemDispatcherQueueHelper? _dispatcherQueueHelper;

    /// <summary>
    /// Apply mica when supported
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <returns>Whether mica is supported</returns>
    public static bool TryApplyMica()
    {
        if (!MicaController.IsSupported())
            return false;

        _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
        _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        SystemBackdropConfiguration = new SystemBackdropConfiguration();
        CurrentContext.Window.Activated += WindowOnActivated;
        CurrentContext.Window.Closed += WindowOnClosed;
        ((FrameworkElement)CurrentContext.Window.Content).ActualThemeChanged += OnActualThemeChanged;

        SystemBackdropConfiguration.IsInputActive = true;
        SetConfigurationSourceTheme();

        BackdropController = new MicaController();

        _ = BackdropController.AddSystemBackdropTarget(CurrentContext.Window.As<ICompositionSupportsSystemBackdrop>());
        BackdropController.SetSystemBackdropConfiguration(SystemBackdropConfiguration);
        return true;
    }

    private static void WindowOnActivated(object sender, WindowActivatedEventArgs args)
        => SystemBackdropConfiguration!.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;

    private static void WindowOnClosed(object sender, WindowEventArgs args)
    {
        if (BackdropController is not null)
        {
            BackdropController.Dispose();
            BackdropController = null;
        }

        CurrentContext.Window.Activated -= WindowOnActivated;
        SystemBackdropConfiguration = null;
    }

    private static void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (SystemBackdropConfiguration is not null)
            SetConfigurationSourceTheme();
    }

    private static void SetConfigurationSourceTheme() =>
        SystemBackdropConfiguration!.Theme = CurrentContext.Window.Content switch
        {
            FrameworkElement { ActualTheme: ElementTheme.Dark } => SystemBackdropTheme.Dark,
            FrameworkElement { ActualTheme: ElementTheme.Light } => SystemBackdropTheme.Light,
            FrameworkElement { ActualTheme: ElementTheme.Default } => SystemBackdropTheme.Default,
            _ => SystemBackdropConfiguration!.Theme
        };
}
