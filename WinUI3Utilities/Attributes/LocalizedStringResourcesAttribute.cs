using System;
using Microsoft.Windows.ApplicationModel.Resources;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate for all the .resw files in <see langword="PRIResource"/> under the specified namespace
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
/// &lt;/<see langword="ItemGroup"/>&gt;
/// </code>
/// Then add this group for all of your .resw files:
/// <code>
/// &lt;<see langword="ItemGroup"/>&gt;
///     &lt;<see langword="PRIResource"/> Include="XXX\APage.resw" /&gt;
///     &lt;<see langword="PRIResource"/> Include="XXX\BWindow.resw" /&gt;
///     ...
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
///     <see langword="private static readonly"/> <see cref="ResourceLoader"/> _resourceLoader = <see langword="new"/>(<see cref="ResourceLoader.GetDefaultResourceFilePath"/>, "APageResource");
///     <see langword="public static readonly string"/> TextAPath = _resourceLoader.GetString("TextA/Path");
///     <see langword="public static readonly string"/> TextBPath = _resourceLoader.GetString("TextB/Path");
/// }<br/>
/// <see langword="public static class"/> BWindowResource
/// {
///     <see langword="private static readonly"/> <see cref="ResourceLoader"/> _resourceLoader = <see langword="new"/>(<see cref="ResourceLoader.GetDefaultResourceFilePath"/>, "BWindowResource");
///     <see langword="public static readonly string"/> ResourceAPath = _resourceLoader.GetString("ResourceA/Path");
///     <see langword="public static readonly string"/> ResourceBPath = _resourceLoader.GetString("ResourceB/Path");
/// }
/// ...
/// </code>
/// </remarks>
/// <remarks>
/// 
/// </remarks>
/// <param name="specifiedNamespace">In which namespace the code will be generated</param>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class LocalizedStringResourcesAttribute(string specifiedNamespace) : Attribute
{
    /// <summary>
    /// In which namespace the code will be generated
    /// </summary>
    public string SpecifiedNamespace { get; set; } = specifiedNamespace;
}

