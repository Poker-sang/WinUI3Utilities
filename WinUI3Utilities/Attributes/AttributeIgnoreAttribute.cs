using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Ignores the effect of the specified attributes on this target<br/>
/// For specific effects, see corresponding attributes
/// </summary>
/// <inheritdoc cref="AttributeIgnoreAttribute"/>
/// <param name="attributes">Attributes to ignore</param>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public class AttributeIgnoreAttribute(params Type[] attributes) : Attribute
{

    /// <summary>
    /// Attributes to ignore
    /// </summary>
    public Type[] Attributes { get; } = attributes;
}
