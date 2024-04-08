using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace WinUI3Utilities.Controls;

/// <summary>
/// Glyph icon markup extension
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(GlyphIcon))]
public class GlyphIconExtension : GlyphIconBaseExtension
{
    /// <inheritdoc cref="FrameworkElement.FlowDirection"/>
    public FlowDirection FlowDirection { get; set; }

    /// <inheritdoc />
    protected override object ProvideValue()
    {
        var icon = new GlyphIcon
        {
            IsBackLayer = IsBackLayer,
            IconGlyph = Glyph,
            FontFamily = FontFamily,
            FontWeight = FontWeight,
            FontStyle = FontStyle,
            IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
            MirroredWhenRightToLeft = MirroredWhenRightToLeft,
            FlowDirection = FlowDirection
        };

        if (Size is not FontSizeType.None)
            icon.FontSize = (int)Size;
        else if (FontSize > 0)
            icon.FontSize = FontSize;

        if (Foreground is not null)
            icon.Foreground = Foreground;

        return icon;
    }
}
