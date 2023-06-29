using System;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate helper properties to limit the window size
/// </summary>
/// <remarks>
/// <strong>This attribute can ONLY be placed on a <see cref="Window"/> class</strong><br/>
/// Generate properties: <see langword="int"/> MinWidth, <see langword="int"/> MinHeight, <see langword="int"/> MaxWidth, <see langword="int"/> MaxHeight
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class WindowSizeHelperAttribute : Attribute
{
    /// <inheritdoc cref="WindowSizeHelperAttribute"/>
    /// <param name="propertyModifier">The modifier of properties: MinWidth, MinHeight, MaxWidth, MaxHeight</param>
    public WindowSizeHelperAttribute(string propertyModifier = "public") => PropertyModifier = propertyModifier;

    /// <summary>
    /// The modifier of properties: MinWidth, MinHeight, MaxWidth, MaxHeight
    /// </summary>
    /// <remarks>Default: "public"</remarks>
    public string PropertyModifier { get; }
}
