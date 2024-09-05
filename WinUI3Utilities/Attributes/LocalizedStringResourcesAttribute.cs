using System;
using Microsoft.Windows.ApplicationModel.Resources;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate for all the .resw and .resjson files in <see langword="PRIResource"/> under the specified namespace
/// </summary>
/// <remarks>
/// <seealso href="https://platform.uno/blog/using-msbuild-items-and-properties-in-c-9-source-generators/"/><br/>
/// To use this Attribute, at first, you should add this region to your .csproj file:
/// <code>
/// &lt;<see langword="PropertyGroup"/>&gt;
///     &lt;<see langword="EnableDefaultPriItems"/>&gt;false&lt;/<see langword="EnableDefaultPriItems"/>&gt;
/// &lt;/<see langword="PropertyGroup"/>&gt;
/// &lt;<see langword="Target"/> Name="InjectAdditionalFiles" BeforeTargets="GenerateMSBuildEditorConfigFileShouldRun"&gt;
///     &lt;<see langword="ItemGroup"/>&gt;
///         &lt;<see langword="AdditionalFiles"/> Include="@(PRIResource)" SourceItemGroup="PRIResource" /&gt;
///     &lt;/<see langword="ItemGroup"/>&gt;
/// &lt;/<see langword="Target"/>&gt;
/// &lt;<see langword="ItemGroup"/>&gt;
///     &lt;<see langword="CompilerVisibleItemMetadata"/> Include="AdditionalFiles" MetadataName="SourceItemGroup" /&gt;
///     &lt;<see langword="PRIResource"/> Include="**\*.resw" /&gt;
///     &lt;<see langword="PRIResource"/> Include="**\*.resjson" /&gt;
/// &lt;/<see langword="ItemGroup"/>&gt;
/// </code>
/// And declare an attribute with a namespace provided:
/// <code>
/// [<see langword="assembly"/>: <see cref="LocalizedStringResourcesAttribute"/>(<see langword="nameof"/>(YourNamespace))]<br/>
/// </code>
/// The code will be generated:
/// <code>
/// <see langword="namespace"/> <see cref="SpecifiedNamespace"/>;<br/>
/// <see langword="public static class"/> APageResource
/// {
///     <see langword="private static readonly"/> <see cref="ResourceLoader"/> _resourceLoader = <see langword="new"/>(<see cref="ResourceLoader.GetDefaultResourceFilePath"/>, "APage");
///     <see langword="public static string"/> GetResourceFromId(<see langword="string"/> id) => _resourceLoader.GetString(id);
///     <see langword="public static string"/> GetResource(<see langword="string"/> name) => _resourceLoader.GetString(MetadataTable[name]);
///     <see langword="public static"/> FrozenDictionary&lt;<see langword="string"/>, <see langword="string"/>&gt; MetadataTable { <see langword="get"/>; } = <see langword="new"/> Dictionary&lt;<see langword="string"/>, <see langword="string"/>&gt; 
///          { [<see langword="nameof"/>(TextAPath)] = "TextA/Path", ... }.ToFrozenDictionary();
///     <see langword="public static string"/> TextAPath { <see langword="get"/>; } = _resourceLoader.GetString("TextA/Path");
///     ...
/// }<br/>
/// ...
/// </code>
/// </remarks>
/// <remarks>
/// 
/// </remarks>
/// <param name="specifiedNamespace">In which namespace the code will be generated</param>
/// <param name="isSubProject">Is this a subproject</param>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class LocalizedStringResourcesAttribute(string specifiedNamespace, bool isSubProject = false) : Attribute
{
    /// <summary>
    /// In which namespace the code will be generated
    /// </summary>
    public string SpecifiedNamespace { get; set; } = specifiedNamespace;

    /// <summary>
    /// Is this a subproject
    /// </summary>
    public bool IsSubProject { get; set; } = isSubProject;
}

