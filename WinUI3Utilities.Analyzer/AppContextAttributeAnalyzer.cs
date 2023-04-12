using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WinUI3Utilities.Analyzer;



// [DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AppContextAttributeAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "Utilities";

    private static readonly DiagnosticDescriptor _needFullName = new(
        DiagnosticId + "004",
        "填写方法全限定名",
        "'{0}' 应该是扩展方法的全限定名{1}",
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
            if (attribute.AttributeClass?.OriginalDefinition.ToDisplayString() is not "WinUI3Utilities.Attributes.AppContextAttribute<T>")
                continue;

            if (attribute.NamedArguments.FirstOrDefault(t => t is { Key: "CastMethod", Value.Value: string and not "" }) is { Key: null } arg)
                continue;

            var castMethod = (string)arg.Value.Value!;

            if (castMethod.StartsWith("global::"))
                castMethod = castMethod[8..];

            var name = castMethod.Split('.')[^1];

            var possibleFullName = "";

            if (context.Compilation.ContainsSymbolsWithName(name, SymbolFilter.Member))
            {
                var symbols = context.Compilation.GetSymbolsWithName(name, SymbolFilter.Member);
                if (symbols.Any(s =>
                    {
                        if (s is IMethodSymbol method)
                        {
                            var symbolName = method.ToDisplayString().Split('(')[0];
                            if (symbolName == castMethod
                                && method is { TypeParameters: [{ } type], Parameters: [{ } param] }
                                && SymbolEqualityComparer.Default.Equals(param.Type, type))
                            {
                                return true;
                            }
                            else
                                possibleFullName = $"，可能是 '{symbolName}'";
                        }

                        return false;
                    }))
                    continue;
            }

            var diagnostic = Diagnostic.Create(_needFullName, attribute.AttributeConstructor!.Locations[0], castMethod, possibleFullName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
