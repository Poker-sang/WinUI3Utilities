using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for backdrop
/// </summary>
public static class BackdropHelper
{
    /// <summary>
    /// Set <see cref="Window.SystemBackdrop"/> if supported.
    /// Otherwise, set to <see langword="null"/>.
    /// </summary>
    /// <param name="window"></param>
    /// <param name="backdropType"></param>
    public static void SetBackdrop(this Window window, BackdropType backdropType)
    {
        window.SystemBackdrop = backdropType switch
        {
            BackdropType.Acrylic when DesktopAcrylicController.IsSupported() => new DesktopAcrylicBackdrop(),
            BackdropType.Mica when MicaController.IsSupported() => new MicaBackdrop(),
            BackdropType.MicaAlt when MicaController.IsSupported() => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Maintain => window.SystemBackdrop,
            BackdropType.None
                or BackdropType.Acrylic
                or BackdropType.Mica
                or BackdropType.MicaAlt => null,
            _ => ThrowHelper.ArgumentOutOfRange<BackdropType, SystemBackdrop>(backdropType)
        };
    }
}
