using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using WinRT;
using WinUI3Utilities.Internal;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="Window"/>
/// </summary>
public static class BackdropHelper
{
    /// <summary>
    /// Backdrop type
    /// </summary>
    public enum BackdropType
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Acrylic
        /// </summary>
        Acrylic,
        /// <summary>
        /// Mica
        /// </summary>
        Mica,
        /// <summary>
        /// Mica alt
        /// </summary>
        MicaAlt
    }

    /// <summary>
    /// Mica controller
    /// </summary>
    public static MicaController? MicaController { get; private set; }

    /// <summary>
    /// Acrylic controller
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
    /// <param name="window"></param>
    /// <returns>Whether mica is supported</returns>
    public static bool TryApplyMica(Window window, bool useMicaAlt = true)
    {
        if (!MicaController.IsSupported())
            return false;

        Init(window, MicaController = new MicaController { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base });

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
    /// <param name="window"></param>
    /// <returns>Whether acrylic is supported</returns>
    public static bool TryApplyAcrylic(Window window)
    {
        if (!DesktopAcrylicController.IsSupported())
            return false;

        Init(window, AcrylicController = new DesktopAcrylicController());

        return true;
    }

    private static void Init(Window window, ISystemBackdropControllerWithTargets controller)
    {
        _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
        _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();


        // Hooking up the policy object
        SystemBackdropConfiguration = new SystemBackdropConfiguration();
        window.Activated += WindowOnActivated;
        window.Closed += WindowOnClosed;
        window.Content.To<FrameworkElement>().ActualThemeChanged += OnActualThemeChanged;

        // Initial configuration state
        SystemBackdropConfiguration.IsInputActive = true;
        SetConfigurationSourceTheme(window);

        // Enable the system backdrop
        _ = controller.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
        controller.SetSystemBackdropConfiguration(SystemBackdropConfiguration);

        void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (SystemBackdropConfiguration is not null)
                SetConfigurationSourceTheme(window);
        }

        void WindowOnClosed(object sender, WindowEventArgs args)
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

            window.Activated -= WindowOnActivated;
            SystemBackdropConfiguration = null;
        }

        void WindowOnActivated(object sender, WindowActivatedEventArgs args)
            => SystemBackdropConfiguration!.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
    }


    private static void SetConfigurationSourceTheme(Window window) =>
        SystemBackdropConfiguration!.Theme = window.Content switch
        {
            FrameworkElement { ActualTheme: ElementTheme.Dark } => SystemBackdropTheme.Dark,
            FrameworkElement { ActualTheme: ElementTheme.Light } => SystemBackdropTheme.Light,
            FrameworkElement { ActualTheme: ElementTheme.Default } => SystemBackdropTheme.Default,
            _ => SystemBackdropConfiguration.Theme
        };
}
