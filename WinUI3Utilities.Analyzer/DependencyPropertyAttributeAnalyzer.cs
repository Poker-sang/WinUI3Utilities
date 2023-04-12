using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace WinUI3Utilities.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyPropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "Utilities";

    private static readonly DiagnosticDescriptor _needFullName = new(
        DiagnosticId + "004",
        "填写方法全限定名",
        "'{0}' 应该是方法的全限定名{1}",
        "Fix", DiagnosticSeverity.Warning, true,
        "应该填写方法全限定名.");

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_needFullName);

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        foreach (var attribute in typeSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.OriginalDefinition.ToDisplayString() is not "WinUI3Utilities.Attributes.DependencyPropertyAttribute<T>")
                continue;

            if (attribute.ConstructorArguments is not [_, { Value: string propertyChanged and not "" }, ..])
                continue;

            if (propertyChanged.StartsWith("global::"))
                propertyChanged = propertyChanged[8..];

            var name = propertyChanged.Split('.')[^1];

            var possibleFullName = "";

            if (context.Compilation.ContainsSymbolsWithName(name, SymbolFilter.Member))
            {
                var symbols = context.Compilation.GetSymbolsWithName(name, SymbolFilter.Member);
                if (symbols.Any(symbol =>
                    {
                        if (symbol.Kind is SymbolKind.Method)
                        {
                            var symbolName = symbol.ToDisplayString().Split('(')[0];
                            if (symbolName == propertyChanged)
                                return true;
                            else
                                possibleFullName = $"，可能是 '{symbolName}'" ;
                        }

                        return false;
                    }))
                    continue;
            }

            var diagnostic = Diagnostic.Create(_needFullName, attribute.AttributeConstructor!.Locations[0], propertyChanged, possibleFullName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
