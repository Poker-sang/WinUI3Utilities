using System;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate:
/// <code>
/// <see langword="public static readonly"/> <see cref="DependencyProperty"/> Property = <see cref="DependencyProperty"/>.Register("Field", <see langword="typeof"/>(Type), <see langword="typeof"/>(TClass), <see langword="new"/> <see cref="PropertyMetadata"/>(DefaultValue, OnPropertyChanged));
/// <br/>
/// <see langword="public"/> <see cref="T:Attributes.DependencyPropertyAttribute`1"/> Field { <see langword="get"/> => (<see cref="T:Attributes.DependencyPropertyAttribute`1"/>)GetValue(Property); <see langword="set"/> => SetValue(Property, <see langword="value"/>); }
/// </code>
/// </summary>
/// <typeparam name="T">property type (nullable-references are not allowed)</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute<T> : Attribute
{
    /// <summary>
    /// <inheritdoc cref="DependencyPropertyAttribute{T}"/>
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="propertyChanged">the name of the method, which called when property changed</param>
    public DependencyPropertyAttribute(string name, string propertyChanged = "")
    {
        Name = name;
        PropertyChanged = propertyChanged;
    }

    /// <summary>
    /// Property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name of the method, which called when property changed.
    /// </summary>
    public string PropertyChanged { get; }

    /// <summary>
    /// Whether property setter is public
    /// </summary>
    public bool IsSetterPublic { get; init; } = true;

    /// <summary>
    /// Whether property type is nullable (nullable-references are not allowed).
    /// </summary>
    /// <remarks>default: <see langword="true"/></remarks>
    public bool IsNullable { get; init; } = true;

    /// <summary>
    /// Default value of property.
    /// </summary>
    /// <remarks>default: <see cref="DependencyProperty.UnsetValue"/></remarks>
    public string DefaultValue { get; init; } = "DependencyProperty.UnsetValue";
}
