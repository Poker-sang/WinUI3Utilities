using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace WinUI3Utilities;

/// <summary>
/// Helper class for creating composition-brush based backdrops.
/// </summary>
public partial class DefaultBrushBackdrop(bool isDarkMode) : SystemBackdrop
{
    /// <summary>
    /// Is dark mode.
    /// </summary>
    public bool IsDarkMode
    {
        get;
        set
        {
            field = value;
            if (_brush is not null)
                _brush.Color = value ? Colors.Black : Colors.White;
        }
    } = isDarkMode;

    /// <summary>
    /// Called when the brush needs to be created for the provided compositor.
    /// </summary>
    /// <param name="compositor">Compositor context</param>
    /// <returns>Brush</returns>
    protected Windows.UI.Composition.CompositionBrush CreateBrush(Windows.UI.Composition.Compositor compositor) =>
        _brush = compositor.CreateColorBrush(IsDarkMode ? Colors.Black : Colors.White);

    private Windows.UI.Composition.CompositionColorBrush? _brush;

    /// <inheritdoc />
    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop? target, XamlRoot xamlRoot)
    {
        if (target is not null)
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
    }

    /// <inheritdoc />
    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        connectedTarget.SystemBackdrop = CreateBrush(WindowHelper.Compositor);
        base.OnTargetConnected(connectedTarget, xamlRoot);
    }

    /// <inheritdoc />
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        var backdrop = disconnectedTarget.SystemBackdrop;
        disconnectedTarget.SystemBackdrop = null;
        backdrop?.Dispose();
        base.OnTargetDisconnected(disconnectedTarget);
    }
}
