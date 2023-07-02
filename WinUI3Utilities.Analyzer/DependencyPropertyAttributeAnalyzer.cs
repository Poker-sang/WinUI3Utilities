using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WinUI3Utilities.Analyzer;

//[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyPropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "Utilities";

    private static readonly DiagnosticDescriptor _convertible = new(
        DiagnosticId + "001R",
        "转化为属性和字段",
        "DependencyPropertyAttribute可以转化为依赖属性字段 '{0}Property' 和属性 '{0}'",
        "Refactor", DiagnosticSeverity.Info, true,
        "没有对应的属性，应该声明.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_convertible);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        foreach (var attribute in typeSymbol.GetAttributes())
        {
            var attributeName = attribute.AttributeClass!.OriginalDefinition.ToDisplayString();
            if (attributeName is not $"{nameof(WinUI3Utilities)}.Attributes.DependencyPropertyAttribute<T>")
                continue;

            if (attribute.ConstructorArguments is not [{ Value: string propertyName }, ..])
                continue;

            context.ReportDiagnostic(Diagnostic.Create(_convertible, attribute.AttributeClass.Locations[0], propertyName));
        }
    }
}
