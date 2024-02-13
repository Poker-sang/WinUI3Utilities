using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate property according to the properties of the settings class <typeparamref name="T"/> for the specified viewmodel class
/// (the attributes on the property will also be copied)<br/>
/// <strong>Use <see cref="AttributeIgnoreAttribute"/> to indicate which properties are ignored</strong><br/>
/// Example:
/// <code>
/// <see langword="public record"/> <typeparamref name="T"/><br/>
/// {
///     [Attribute1]
///     <see langword="public"/> Type1 Property1 { <see langword="get"/>; <see langword="set"/>; }
///     [<see cref="AttributeIgnoreAttribute"/>(<see langword="typeof"/>(<see cref="SettingsViewModelAttribute{T}"/>))]
///     <see langword="public"/> Type2 Property2 { <see langword="get"/>; <see langword="set"/>; }
///     ...
/// }
/// </code>
/// Generate:
/// <code>
/// <see langword="partial class"/> SpecifiedClass
/// {
///     [Attribute1]
///     <see langword="public"/> Type1 Property1
///     {
///         <see langword="get"/> => <see cref="SettingName"/>.Property1;
///         <see langword="set"/> => SetProperty(<see cref="SettingName"/>.Property1, <see langword="value"/>, <see cref="SettingName"/>, (@setting, @value) => @setting.Property1 = @value);
///     }
///     ...
/// }
/// </code>
/// </summary>
/// <param name="settingName">The name of the <typeparamref name="T"/> field</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SettingsViewModelAttribute<T>(string settingName) : Attribute
{
    /// <summary>
    /// The name of the <typeparamref name="T"/> field
    /// </summary>
    public string SettingName { get; } = settingName;
}
