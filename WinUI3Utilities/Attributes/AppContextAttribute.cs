using System;
using Windows.Storage;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// <strong>Only for packaged applications</strong><br/>
/// Generate field _configurationContainer and methods Load/SaveConfiguration for the specified class<br/>
/// <strong>Use <see cref="AttributeIgnoreAttribute"/> to indicate which properties are ignored</strong><br/>
/// Generate:
/// <code>
/// <see langword="partial class"/> SpecifiedClass
/// {
///     <see langword="private static"/> <see cref="ApplicationDataContainer"/> _configurationContainer = <see langword="null"/>!;
/// 
///     <see langword="private const string"/> ConfigurationContainerKey = <see cref="ConfigKey"/>;
/// 
///     <see langword="public static void"/> InitializeConfigurationContainer()
///     {
///         <see langword="var"/> roamingSettings = <see cref="ApplicationData"/>.Current.RoamingSettings.Current.RoamingSettings;
///         <see langword="if"/> (!roamingSettings.Containers.ContainsKey(ConfigurationContainerKey))
///             _ = roamingSettings.CreateContainer(ConfigurationContainerKey, <see cref="ApplicationDataCreateDisposition.Always"/>);
/// 
///         _configurationContainer = roamingSettings.Containers[ConfigurationContainerKey];
///     }
/// 
///     <see langword="public static"/> <typeparamref name="T"/>? LoadConfiguration()
///     {
///         <see langword="try"/><br/>
///         {
///             <see langword="return new"/> <typeparamref name="T"/>(
///                 _configurationContainer.Values[<see langword="nameof"/>(<typeparamref name="T"/>.Property1)].<see cref="CastMethod"/>&lt;Type1&gt;()
///                 _configurationContainer.Values[<see langword="nameof"/>(<typeparamref name="T"/>.Property2)].<see cref="CastMethod"/>&lt;Type2&gt;()
///                 ...
///             );
///         }
///         <see langword="catch"/><br/>
///         {
///             <see langword="return default"/>;
///         }
///     }
/// 
///     <see langword="public static void"/> SaveConfiguration(<typeparamref name="T"/>? configuration)
///     {
///         <see langword="if"/> (configuration <see langword="is"/> { } appConfiguration)
///         {
///             _configurationContainer.Values[<see langword="nameof"/>(<typeparamref name="T"/>.Property1)] = appConfiguration.Property1;
///             _configurationContainer.Values[<see langword="nameof"/>(<typeparamref name="T"/>.Property2)] = appConfiguration.Property2;
///             ...
///         }
///     }
/// }
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AppContextAttribute<T> : Attribute
{
    /// <summary>
    /// Configuration container key
    /// </summary>
    /// <remarks>default: "Configuration"</remarks>
    public string ConfigKey { get; init; } = "Configuration";

    /// <summary>
    /// The fullname of method to cast settings properties
    /// </summary>
    /// <remarks>default: (the fullname of) <see cref="Misc.ToNotNull{T}"/></remarks>
    public string CastMethod { get; init; } = $"{nameof(WinUI3Utilities)}.{nameof(Misc)}.{nameof(Misc.ToNotNull)}";
}
