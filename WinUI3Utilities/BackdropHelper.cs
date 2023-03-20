using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
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
        /// Use <see cref="TryApplyAcrylic"/>
        /// </summary>
        Acrylic,
        /// <summary>
        /// Use <see cref="TryApplyMica"/>
        /// </summary>
        Mica,
        /// <summary>
        /// Use <see cref="TryApplyMica"/>
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
    /// <remarks>
    /// Related to <see cref="BackdropType.Mica"/> and <see cref="BackdropType.MicaAlt"/>
    /// </remarks>
    /// <param name="useMicaAlt"></param>
    /// <param name="window"></param>
    /// <returns>Whether mica is supported</returns>
    public static bool TryApplyMica(Window window, bool useMicaAlt = true)
    {
        if (!MicaController.IsSupported())
            return false;

        Init(window, MicaController = new() { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base });

        return true;
    }

    /// <summary>
    /// Apply acrylic when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="BackdropType.Acrylic"/>
    /// </remarks>
    /// <param name="window"></param>
    /// <returns>Whether acrylic is supported</returns>
    public static bool TryApplyAcrylic(Window window)
    {
        if (!DesktopAcrylicController.IsSupported())
            return false;

        Init(window, AcrylicController = new());

        return true;
    }

    private static void Init(Window window, ISystemBackdropControllerWithTargets controller)
    {
        _dispatcherQueueHelper = new();
        _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        // Hooking up the policy object
        SystemBackdropConfiguration = new();
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
            // use this closed window.
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
