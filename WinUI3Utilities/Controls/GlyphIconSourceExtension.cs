using Microsoft.UI.Xaml.Markup;

namespace WinUI3Utilities.Controls;

/// <summary>
/// Glyph icon source markup extension
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(GlyphIconSource))]
public class GlyphIconSourceExtension : GlyphIconBaseExtension
{
    /// <inheritdoc />
    protected override object ProvideValue()
    {
        var icon = new GlyphIconSource
        {
            IsBackLayer = IsBackLayer,
            IconGlyph = Glyph,
            FontFamily = FontFamily,
            FontWeight = FontWeight,
            FontStyle = FontStyle,
            IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
            MirroredWhenRightToLeft = MirroredWhenRightToLeft
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
