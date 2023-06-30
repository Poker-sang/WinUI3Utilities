using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Possible value of property
/// </summary>
public enum DependencyPropertyDefaultValue
{
    /// <summary>
    /// <see cref="DependencyProperty.UnsetValue"/>
    /// </summary>
    UnsetValue,
    /// <summary>
    /// <see langword="default"/>(T)
    /// </summary>
    Default,
    /// <summary>
    /// <see langword="new"/> T()
    /// </summary>
    /// <remarks>
    /// Call non-parameter construction
    /// </remarks>
    New
}
