using System;
using Windows.Storage;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// <strong>Only for packaged applications</strong><br/>
/// Generate field _container<see cref="MethodName"/> and methods Initialize/Load/Save<see cref="MethodName"/> for the specified class<br/>
/// <strong>Use <see cref="AttributeIgnoreAttribute"/> to indicate which properties are ignored</strong><br/>
/// Generate:
/// <code>
/// <see langword="partial class"/> SpecifiedClass
/// {
///     <see langword="private static"/> <see cref="ApplicationDataContainer"/> _container<see cref="MethodName"/> = <see langword="null"/>!;
/// 
///     <see langword="private const string"/> <see cref="MethodName"/>Key = <see cref="ConfigKey"/>;
/// 
///     <see langword="public static void"/> Initialize<see cref="MethodName"/>()
///     {
///         <see langword="var"/> settings = <see cref="ApplicationData"/>.Current.<see cref="Type"/>;
///         <see langword="if"/> (!settings.Containers.ContainsKey(<see cref="MethodName"/>Key))
///         {
///             _ = settings.CreateContainer(<see cref="MethodName"/>Key, <see cref="CreateDisposition"/>);
///         }
/// 
///         _container<see cref="MethodName"/> = settings.Containers[<see cref="MethodName"/>Key];
///     }
/// 
///     <see langword="public static"/> <typeparamref name="T"/>? Load<see cref="MethodName"/>()
///     {
///         <see langword="try"/><br/>
///         {
///             <see langword="var"/> values = _container<see cref="MethodName"/>.Values;
///             <see langword="var"/> converter = <see langword="new"/> <typeparamref name="TConverter"/>();
///             <see langword="return new"/> <typeparamref name="T"/>(
///                 converter.ConvertBack&lt;Type1&gt;(values[<see langword="nameof"/>(<typeparamref name="T"/>.Property1)])
///                 converter.ConvertBack&lt;Type2&gt;(values[<see langword="nameof"/>(<typeparamref name="T"/>.Property2)])
///                 ...
///             );
///         }
///         <see langword="catch"/><br/>
///         {
///             <see langword="return default"/>;
///         }
///     }
/// 
///     <see langword="public static void"/> Save<see cref="MethodName"/>(<typeparamref name="T"/>? configuration)
///     {
///         <see langword="if"/> (configuration <see langword="is"/> { } appConfiguration)
///         {
///             <see langword="var"/> values = _container<see cref="MethodName"/>.Values;
///             <see langword="var"/> converter = <see langword="new"/> <typeparamref name="TConverter"/>();
///             values[<see langword="nameof"/>(<typeparamref name="T"/>.Property1)] = converter.Convert(appConfiguration.Property1);
///             values[<see langword="nameof"/>(<typeparamref name="T"/>.Property2)] = converter.Convert(appConfiguration.Property2);
///             ...
///         }
///     }
/// }
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TConverter"></typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AppContextAttribute<T, TConverter> : Attribute where TConverter : ISettingsValueConverter, new()
{
    /// <summary>
    /// Configuration container key
    /// </summary>
    /// <remarks>Default: "Configuration"</remarks>
    public string ConfigKey { get; init; } = "Configuration";

    /// <summary>
    /// Method name
    /// </summary>
    /// <remarks>Default: "Configuration"</remarks>
    public string MethodName { get; init; } = "Configuration";

    /// <inheritdoc cref="ApplicationDataContainerType"/>
    /// <remarks>Default: <see cref="ApplicationDataContainerType.Local"/></remarks>
    public ApplicationDataContainerType Type { get; init; } = ApplicationDataContainerType.Local;

    /// <inheritdoc cref="ApplicationDataCreateDisposition"/>
    /// <remarks>Default: <see cref="ApplicationDataCreateDisposition.Always"/></remarks>
    public ApplicationDataCreateDisposition CreateDisposition { get; init; } = ApplicationDataCreateDisposition.Always;
}

/// <inheritdoc />
public class AppContextAttribute<T> : AppContextAttribute<T, SettingsValueConverter>;
