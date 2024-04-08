using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Controls;

/// <summary>
/// GlyphIconSource
/// </summary>
[DependencyProperty<IconGlyph>("IconGlyph", DependencyPropertyDefaultValue.UnsetValue, nameof(IconGlyphPropertyChangedCallback))]
[DependencyProperty<FontSizeType>("Size", DependencyPropertyDefaultValue.UnsetValue, nameof(SizePropertyChangedCallback))]
[DependencyProperty<bool>("IsBackLayer", DependencyPropertyDefaultValue.UnsetValue, nameof(IsBackLayerPropertyChangedCallback))]
public partial class GlyphIconSource : FontIconSource
{
    private static void IconGlyphPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.To<GlyphIcon>().Glyph = ((char)e.NewValue.To<IconGlyph>()).ToString();
    }

    private static void SizePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is { } size and not FontSizeType.None)
            d.To<GlyphIcon>().FontSize = (int)size;
    }

    private static void IsBackLayerPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            var icon = d.To<GlyphIcon>();
            var color = icon.Foreground.To<SolidColorBrush>().Color;
            icon.Foreground = new SolidColorBrush(Color.FromArgb(0x80, (byte)~color.R, (byte)~color.G, (byte)~color.B));
        }
    }
}
