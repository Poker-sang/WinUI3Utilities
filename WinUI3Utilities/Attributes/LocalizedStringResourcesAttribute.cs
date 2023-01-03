using System;
using Microsoft.Windows.ApplicationModel.Resources;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate all the .resw files under the namespace of the specified type
/// </summary>
/// <remarks>
/// <seealso href="https://platform.uno/blog/using-msbuild-items-and-properties-in-c-9-source-generators/"/><br/>
/// <strong>This Attribute Should Only Appear ONCE Per Project</strong><br/>
/// To use this Attribute, first you should add this region to your csproj file
/// <code>
/// &lt;<see langword="Target"/> Name="InjectAdditionalFiles" BeforeTargets="GenerateMSBuildEditorConfigFileShouldRun"&gt;
///     &lt;<see langword="ItemGroup"/>&gt;
///         &lt;AdditionalFiles Include="@(PRIResource)" SourceItemGroup="PRIResource" /&gt;
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
///     &lt;<see langword="PRIResource"/> Include="XXX\BResource.resw" /&gt;
///     ...
/// &lt;/<see langword="ItemGroup"/>&gt;
/// </code>
/// And when you write:
/// <code>
/// <see langword="namespace"/> YourNamespace;
/// [<see cref="LocalizedStringResourcesAttribute"/>]<br/>
/// <see langword="file record"/> AnyName;
/// </code>
/// Code will generate:
/// <code>
/// <see langword="namespace"/> YourNamespace;<br/>
/// <see langword="public static class"/> APage
/// {
///     <see langword="private static readonly"/> <see cref="ResourceLoader"/> _resourceLoader = new(<see cref="ResourceLoader.GetDefaultResourceFilePath"/>, "APage");
///     <see langword="public static readonly string"/> TextAPath = _resourceLoader.GetString("TextA/Path");
///     <see langword="public static readonly string"/> TextBPath = _resourceLoader.GetString("TextB/Path");
/// }<br/>
/// <see langword="public static class"/> BResource
/// {
///     <see langword="private static readonly"/> <see cref="ResourceLoader"/> _resourceLoader = new(<see cref="ResourceLoader.GetDefaultResourceFilePath"/>, "BResource");
///     <see langword="public static readonly string"/> ResourceAPath = _resourceLoader.GetString("ResourceA/Path");
///     <see langword="public static readonly string"/> ResourceBPath = _resourceLoader.GetString("ResourceB/Path");
/// }
/// ...
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class LocalizedStringResourcesAttribute : Attribute
{

}
