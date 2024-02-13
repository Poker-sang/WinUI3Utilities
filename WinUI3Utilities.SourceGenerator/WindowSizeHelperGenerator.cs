using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using WinUI3Utilities.SourceGenerator.Utilities;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class WindowSizeHelperGenerator() : TypeWithAttributeGenerator("WindowSizeHelperAttribute")
{
    internal override string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var attribute = attributeList[0];

        if (attribute.ConstructorArguments is not [{ Value: string propertyModifier }, ..])
            return null;

        const string className = "global::WinUI3Utilities.WindowSizeHelper";
        const string propertyName = "_WinUI3Utilities_WindowSizeHelper_WindowSizeParameter";
        const string fieldName = "_" + propertyName;

        return $$"""
                 #nullable enable

                 namespace {{typeSymbol.ContainingNamespace.ToDisplayString()}};

                 partial class {{typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}}
                 {
                     /// <summary>
                     /// DO NOT MODIFY OR RELEASE<br/>
                     /// Generated for <see cref="{{className}}"/>
                     /// </summary>
                     private {{className}}.WindowSizeParameter? {{fieldName}};
                 
                     /// <summary>
                     /// DO NOT MODIFY OR RELEASE<br/>
                     /// Generated for <see cref="{{className}}"/>
                     /// </summary>
                     [global::System.CodeDom.Compiler.GeneratedCode("{{SourceGeneratorHelper.AssemblyName}}{{nameof(WindowSizeHelperGenerator)}}", "{{SourceGeneratorHelper.AssemblyVersion}}")]
                     [global::System.Diagnostics.DebuggerNonUserCode]
                     [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                     private {{className}}.WindowSizeParameter {{propertyName}}
                     {
                         get
                         {
                             if (this.{{fieldName}} is null)
                             {
                                 this.{{fieldName}} = new();
                                 {{className}}.Register(this, this.{{fieldName}});
                             }
                             return this.{{fieldName}};
                         }
                     }
                 
                     /// <summary>
                     /// Minimum width of the <see cref="global::Microsoft.UI.Xaml.Window"/>
                     /// </summary>
                     /// <remarks>
                     /// Default: 0
                     /// </remarks>
                     {{propertyModifier}} int MinWidth
                     {
                         get => this.{{propertyName}}.MinWidth;
                         set => this.{{propertyName}}.MinWidth = value;
                     }
                 
                     /// <summary>
                     /// Minimum height of the <see cref="global::Microsoft.UI.Xaml.Window"/>
                     /// </summary>
                     /// <remarks>
                     /// Default: 0
                     /// </remarks>
                     {{propertyModifier}} int MinHeight
                     {
                         get => this.{{propertyName}}.MinHeight;
                         set => this.{{propertyName}}.MinHeight = value;
                     }
                 
                     /// <summary>
                     /// Maximum width of the <see cref="global::Microsoft.UI.Xaml.Window"/>
                     /// </summary>
                     /// <remarks>
                     /// Default: 0
                     /// </remarks>
                     {{propertyModifier}} int MaxWidth
                     {
                         get => this.{{propertyName}}.MaxWidth;
                         set => this.{{propertyName}}.MaxWidth = value;
                     }
                 
                     /// <summary>
                     /// Maximum height of the <see cref="global::Microsoft.UI.Xaml.Window"/>
                     /// </summary>
                     /// <remarks>
                     /// Default: 0
                     /// </remarks>
                     {{propertyModifier}} int MaxHeight
                     {
                         get => this.{{propertyName}}.MaxHeight;
                         set => this.{{propertyName}}.MaxHeight = value;
                     }
                 }
                 """;
    }
}
