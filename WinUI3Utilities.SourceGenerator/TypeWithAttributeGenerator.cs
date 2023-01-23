using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class TypeWithAttributeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// 对拥有某attribute的type生成代码
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="attributeList">该类的某种Attribute</param>
    /// <returns>生成的代码</returns>
    private delegate string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList);

    private const string AttributeNamespace = "WinUI3Utilities.Attributes.";

    private const string DisableSourceGeneratorAttribute = "DisableSourceGeneratorAttribute";

    /// <summary>
    /// 需要生成的Attribute
    /// </summary>
    private static readonly Dictionary<string, TypeWithAttribute> _attributes = new()
    {
        { AttributeNamespace + "GenerateConstructorAttribute", TypeWithAttributeDelegates.GenerateConstructor },
        { AttributeNamespace + "AppContextAttribute`1", TypeWithAttributeDelegates.AppContext },
        { AttributeNamespace + "DependencyPropertyAttribute`1", TypeWithAttributeDelegates.DependencyProperty },
        { AttributeNamespace + "SettingsViewModelAttribute`1", TypeWithAttributeDelegates.SettingsViewModel }
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var attribute in _attributes)
        {
            var generatorAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                attribute.Key,
                (_, _) => true,
                (syntaxContext, _) => syntaxContext
            ).Combine(context.CompilationProvider);



            context.RegisterSourceOutput(generatorAttributes, (spc, tuple) =>
            {
                var (ga, compilation) = tuple;

                if (compilation.Assembly.GetAttributes().Any(attrData => attrData.AttributeClass?.ToString() == AttributeNamespace + DisableSourceGeneratorAttribute))
                {
                    return;
                }

                if (ga.TargetSymbol is not INamedTypeSymbol symbol)
                    return;
                if (attribute.Value(symbol, ga.Attributes) is { } source)
                    spc.AddSource(
                        // 不能重名
                        $"{symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))}_{attribute.Key}.g.cs",
                        source);
            });
        }
    }
}
