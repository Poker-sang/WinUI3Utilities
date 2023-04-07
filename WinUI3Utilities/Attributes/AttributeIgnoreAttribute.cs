using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Ignores the effect of the specified attributes on this target<br/>
/// For specific effects, see corresponding attributes
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public class AttributeIgnoreAttribute : Attribute
{
    /// <inheritdoc cref="AttributeIgnoreAttribute"/>
    /// <param name="attributes">Attributes to ignore</param>
    public AttributeIgnoreAttribute(params Type[] attributes) => Attributes = attributes;

    /// <summary>
    /// Attributes to ignore
    /// </summary>
    public Type[] Attributes { get; }
}
