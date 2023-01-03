using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate property according to the properties of the settings class <typeparamref name="T"/> for the specified viewmodel class
/// (the attributes on the property will also be copied)<br/>
/// <example>
/// Example:
/// <code>
/// <see langword="public record"/> <typeparamref name="T"/><br/>
/// {
///     [Attribute1]
///     <see langword="public"/> Type1 Property1 { <see langword="get"/>; <see langword="set"/>; }
///     [<see cref="SettingsViewModelExclusionAttribute"/>]
///     <see langword="public"/> Type2 Property2 { <see langword="get"/>; <see langword="set"/>; }
///     ...
/// }
/// </code>
/// Generate:
/// <code>
/// <see langword="partial class"/> specifiedClass
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
/// </example>
/// </summary>
/// <remarks>Use <see cref="SettingsViewModelExclusionAttribute"/> to specify which properties are excluded</remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SettingsViewModelAttribute<T> : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingName">The name of the <typeparamref name="T"/> field</param>
    public SettingsViewModelAttribute(string settingName) => SettingName = settingName;

    /// <summary>
    /// The name of the <typeparamref name="T"/> field
    /// </summary>
    public string SettingName { get; }
}
