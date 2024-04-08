using System;
using Windows.UI.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace WinUI3Utilities.Controls;

/// <summary>
/// An abstract <see cref="MarkupExtension"/> which to produce text-based icons.
/// </summary>
public abstract class GlyphIconBaseExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets if the icon in back layer mode.
    /// </summary>
    public bool IsBackLayer { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon to display.
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// Gets or sets the glyph of the icon to display.
    /// </summary>
    public IconGlyph Glyph { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon to display.
    /// </summary>
    public FontFamily? FontFamily { get; set; } = _symbolThemeFontFamily;

    /// <summary>
    /// Gets or sets the size of the icon to display. Priority is higher than <see cref="FontSize"/>.
    /// </summary>
    public FontSizeType Size { get; set; }

    [ThreadStatic]
    private static readonly FontFamily _symbolThemeFontFamily;

    static GlyphIconBaseExtension() => _symbolThemeFontFamily = new("Segoe Fluent Icons,Segoe MDL2 Assets");

    /// <summary>
    /// Gets or sets the thickness of the icon glyph.
    /// </summary>
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;

    /// <summary>
    /// Gets or sets the font style for the icon glyph.
    /// </summary>
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;

    /// <summary>
    /// Gets or sets the foreground <see cref="Brush"/> for the icon.
    /// </summary>
    public Brush? Foreground { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatic text enlargement, to reflect the system text size setting, is enabled.
    /// </summary>
    public bool IsTextScaleFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the icon is mirrored when the flow direction is right to left.
    /// </summary>
    public bool MirroredWhenRightToLeft { get; set; }
}
