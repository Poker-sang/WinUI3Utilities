using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using WinRT;

namespace WinUI3Utilities;

public class MicaHelper
{
    private static MicaController? _backdropController;

    private static WindowsSystemDispatcherQueueHelper? _dispatcherQueueHelper;

    private static SystemBackdropConfiguration? _systemBackdropConfiguration;

    public static void TryApplyMica()
    {
        if (MicaController.IsSupported())
        {
            _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
            _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

            _systemBackdropConfiguration = new SystemBackdropConfiguration();
            CurrentContext.Window.Activated += WindowOnActivated;
            CurrentContext.Window.Closed += WindowOnClosed;
            ((FrameworkElement)CurrentContext.Window.Content).ActualThemeChanged += OnActualThemeChanged;

            _systemBackdropConfiguration.IsInputActive = true;
            SetConfigurationSourceTheme();

            _backdropController = new MicaController();

            _ = _backdropController.AddSystemBackdropTarget(CurrentContext.Window.As<ICompositionSupportsSystemBackdrop>());
            _backdropController.SetSystemBackdropConfiguration(_systemBackdropConfiguration);
        }
    }

    private static void WindowOnActivated(object sender, WindowActivatedEventArgs args)
        => _systemBackdropConfiguration!.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;

    private static void WindowOnClosed(object sender, WindowEventArgs args)
    {
        if (_backdropController is not null)
        {
            _backdropController.Dispose();
            _backdropController = null;
        }

        CurrentContext.Window.Activated -= WindowOnActivated;
        _systemBackdropConfiguration = null;
    }

    private static void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_systemBackdropConfiguration is not null)
            SetConfigurationSourceTheme();
    }

    private static void SetConfigurationSourceTheme() =>
        _systemBackdropConfiguration!.Theme = CurrentContext.Window.Content switch
        {
            FrameworkElement { ActualTheme: ElementTheme.Dark } => SystemBackdropTheme.Dark,
            FrameworkElement { ActualTheme: ElementTheme.Light } => SystemBackdropTheme.Light,
            FrameworkElement { ActualTheme: ElementTheme.Default } => SystemBackdropTheme.Default,
            _ => _systemBackdropConfiguration!.Theme
        };
}
