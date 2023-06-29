using System;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate:
/// <code>
/// <see langword="public static readonly"/> <see cref="DependencyProperty"/> Property = <see cref="DependencyProperty"/>.Register(<see langword="nameof"/>(Field), <see langword="typeof"/>(<typeparamref name="T"/>), <see langword="typeof"/>(TClass), <see langword="new"/> <see cref="PropertyMetadata"/>(DefaultValue, OnPropertyChanged));
/// <br/>
/// <see langword="public"/> <typeparamref name="T"/> Field { <see langword="get"/> => (<typeparamref name="T"/>)GetValue(Property); <see langword="set"/> => SetValue(Property, <see langword="value"/>); }
/// </code>
/// </summary>
/// <typeparam name="T">property type (nullable value type are not allowed)</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute<T> : Attribute where T : notnull
{
    /// <inheritdoc cref="DependencyPropertyAttribute{T}"/>
    /// <param name="name">Property name</param>
    /// <param name="defaultValueType">Possible value of property</param>
    /// <param name="propertyChanged">The name of the method, which called when property changed</param>
    public DependencyPropertyAttribute(string name, DependencyPropertyDefaultValue defaultValueType = DependencyPropertyDefaultValue.UnsetValue, string propertyChanged = "")
    {
        Name = name;
        PropertyChanged = propertyChanged;
        DefaultValueType = defaultValueType;
    }

    /// <inheritdoc cref="DependencyPropertyAttribute{T}"/>
    /// <param name="name">Property name</param>
    /// <param name="defaultValue">Default value of the property</param>
    /// <param name="propertyChanged">The name of the method, which called when property changed</param>
    public DependencyPropertyAttribute(string name, string defaultValue, string propertyChanged = "")
    {
        Name = name;
        PropertyChanged = propertyChanged;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Property name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name of the method, which called when property changed
    /// </summary>
    public string PropertyChanged { get; }

    /// <summary>
    /// <inheritdoc cref="DependencyPropertyDefaultValue"/>
    /// </summary>
    /// <remarks>Default: <see cref="DependencyPropertyDefaultValue.UnsetValue"/></remarks>
    public DependencyPropertyDefaultValue DefaultValueType { get; } = DependencyPropertyDefaultValue.UnsetValue;

    /// <summary>
    /// Default value of the property
    /// </summary>
    /// <remarks>Default: <see cref="DependencyProperty.UnsetValue"/></remarks>
    public string DefaultValue { get; } = "";

    /// <summary>
    /// Whether property setter is private
    /// </summary>
    /// <remarks>Default: <see langword="false"/></remarks>
    public bool IsSetterPrivate { get; init; } = false;

    /// <summary>
    /// Whether property type is nullable (nullable value type are not allowed)
    /// </summary>
    /// <remarks>Default: <see langword="false"/></remarks>
    public bool IsNullable { get; init; }
}
