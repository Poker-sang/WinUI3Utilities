using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using WinRT;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Window"/>
/// </summary>
public static class BackdropHelper
{
    /// <summary>
    /// BackdropType
    /// </summary>
    public enum BackdropType
    {
        /// <summary>
        /// none
        /// </summary>
        None,
        /// <summary>
        /// acrylic
        /// </summary>
        Acrylic,
        /// <summary>
        /// mica
        /// </summary>
        Mica,
        /// <summary>
        /// mica alt
        /// </summary>
        MicaAlt
    }

    /// <summary>
    /// mica controller
    /// </summary>
    public static MicaController? MicaController { get; private set; }

    /// <summary>
    /// acrylic controller
    /// </summary>
    public static DesktopAcrylicController? AcrylicController { get; private set; }

    /// <summary>
    /// System backdrop configuration
    /// </summary>
    public static SystemBackdropConfiguration? SystemBackdropConfiguration { get; private set; }

    private static WindowsSystemDispatcherQueueHelper? _dispatcherQueueHelper;

    /// <summary>
    /// Apply mica when supported
    /// </summary>
    /// <param name="useMicaAlt"></param>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <returns>Whether mica is supported</returns>
    public static bool TryApplyMica(bool useMicaAlt = true)
    {
        if (!MicaController.IsSupported())
            return false;

        Init(MicaController = new MicaController { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base });

        return true;
    }

    /// <summary>
    /// Apply acrylic when supported
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <returns>Whether acrylic is supported</returns>
    public static bool TryApplyAcrylic()
    {
        if (!DesktopAcrylicController.IsSupported())
            return false;

        Init(AcrylicController = new DesktopAcrylicController());

        return true;
    }

    private static void Init(ISystemBackdropControllerWithTargets controller)
    {
        _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
        _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();


        // Hooking up the policy object
        SystemBackdropConfiguration = new SystemBackdropConfiguration();
        CurrentContext.Window.Activated += WindowOnActivated;
        CurrentContext.Window.Closed += WindowOnClosed;
        ((FrameworkElement)CurrentContext.Window.Content).ActualThemeChanged += OnActualThemeChanged;

        // Initial configuration state
        SystemBackdropConfiguration.IsInputActive = true;
        SetConfigurationSourceTheme();

        // Enable the system backdrop
        _ = controller.AddSystemBackdropTarget(CurrentContext.Window.As<ICompositionSupportsSystemBackdrop>());
        controller.SetSystemBackdropConfiguration(SystemBackdropConfiguration);
    }

    private static void WindowOnActivated(object sender, WindowActivatedEventArgs args)
        => SystemBackdropConfiguration!.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;

    private static void WindowOnClosed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use CurrentContext.Window closed window.
        if (MicaController is not null)
        {
            MicaController.Dispose();
            MicaController = null;
        }

        if (AcrylicController is not null)
        {
            AcrylicController.Dispose();
            AcrylicController = null;
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
