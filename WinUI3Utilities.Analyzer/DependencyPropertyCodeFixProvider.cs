using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WinUI3Utilities.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyPropertyCodeFixProvider : DiagnosticAnalyzer
{
    private const string DiagnosticId = "Utilities";

    private static readonly DiagnosticDescriptor _missingProperty = new(
        DiagnosticId + "000",
        "没有对应的属性",
        "依赖属性字段 '{0}' 没有对应的属性",
        "Completion", DiagnosticSeverity.Warning, true,
        "没有对应的属性，应该声明.");

    private static readonly DiagnosticDescriptor _convertible = new(
        DiagnosticId + "001",
        "转化为Attribute",
        "依赖属性字段 '{0}' 可以转化为Attribute",
        "Refactor", DiagnosticSeverity.Warning, true,
        "建议转化为Attribute.");

    private static readonly DiagnosticDescriptor _wrongNamingEnd = new(
        DiagnosticId + "002",
        "应为Property结尾",
        "依赖属性字段 '{0}' 应为Property结尾",
        "Naming", DiagnosticSeverity.Warning, true,
        "此字段应为Property结尾.");

    private static readonly DiagnosticDescriptor _wrongNamingConsistent = new(
        DiagnosticId + "003",
        "应与属性保持一致",
        "依赖属性字段 '{0}' 应与属性保持一致，如 '{1}Property'",
        "Naming", DiagnosticSeverity.Warning, true,
        "此字段应与属性保持一致.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_missingProperty, _convertible, _wrongNamingEnd, _wrongNamingConsistent);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var fieldSymbol = (IFieldSymbol)context.Symbol;

        if (fieldSymbol.Type.ToDisplayString() is not "Microsoft.UI.Xaml.DependencyProperty")
            return;
        if (!fieldSymbol.GetAttributes().IsEmpty)
            return;
        if (fieldSymbol is not { IsReadOnly: true, IsStatic: true, DeclaredAccessibility: Accessibility.Public })
            return;
        if (fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() is not VariableDeclaratorSyntax
            {
                Initializer.Value: InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax { Identifier.ValueText: "DependencyProperty" },
                        Name.Identifier.ValueText: "Register"
                    },
                    ArgumentList.Arguments:
                    [
                        { Expression: { } propertyNameExpression },
                        { Expression: TypeOfExpressionSyntax { Type: { } propertyType } },
                        { Expression: TypeOfExpressionSyntax { Type: { } classType } },
                        {
                            Expression: ObjectCreationExpressionSyntax
                            {
                                Type: IdentifierNameSyntax { Identifier.ValueText: "PropertyMetadata" }
                            }
                        }
                    ]
                }
            })
            return;

        if (!fieldSymbol.Name.EndsWith("Property"))
        {
            var diagnostic = Diagnostic.Create(_wrongNamingEnd, fieldSymbol.Locations[0], fieldSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (!fieldSymbol.ContainingType!.ToDisplayString().EndsWith(classType.ToFullString()))
            return;

        var propertySymbol = (IPropertySymbol)null!;

        var propertyName = propertyNameExpression switch
        {
            LiteralExpressionSyntax { Token.ValueText: var name1 } => name1,
            InvocationExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" },
                ArgumentList.Arguments: [{ Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } }]
            } => name1,
            _ => ""
        };

        if (propertyName is "")
            return;

        if (!FindProperty(propertyName))
        {
            var diagnostic = Diagnostic.Create(_missingProperty, fieldSymbol.Locations[0], fieldSymbol.Name);
            context.ReportDiagnostic(diagnostic);
            return;
        }

        if (!propertySymbol.Type.ToDisplayString().EndsWith(propertyType.ToFullString()))
            return;

        if (propertyName + "Property" != fieldSymbol.Name)
        {
            var diagnostic = Diagnostic.Create(_wrongNamingConsistent, fieldSymbol.Locations[0], fieldSymbol.Name,
                propertyName);
            context.ReportDiagnostic(diagnostic);
        }

        bool FindProperty(string name)
        {
            foreach (var symbol in fieldSymbol.ContainingType.GetMembers()
                         .Where(t => t.Kind is SymbolKind.Property)
                         .Cast<IPropertySymbol>())
            {
                if (symbol.Name == name)
                {
                    propertySymbol = symbol;
                    return true;
                }
            }

            return false;
        }

        if (propertySymbol.DeclaringSyntaxReferences[0].GetSyntax() is not PropertyDeclarationSyntax
            {
                AccessorList.Accessors: [{ } getter, { } setter]
            })
            return;
        var (fieldSymbolNameGetter, type) = getter switch
        {
            // get { return GetValue(xxx); }
            {
                Body.Statements:
                [
                    ReturnStatementSyntax
                    {
                        Expression: CastExpressionSyntax
                        {
                            Type: var type1,
                            Expression: InvocationExpressionSyntax
                            {
                                Expression: IdentifierNameSyntax { Identifier.ValueText: "GetValue" },
                                ArgumentList.Arguments:
                                [
                                    { Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } }
                                ]
                            }
                        }
                    }
                ]
            } => (name1, type1),
            // get => (x)GetValue(xxx); 
            {
                ExpressionBody.Expression: CastExpressionSyntax
                {
                    Type: var type1,
                    Expression: InvocationExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax { Identifier.ValueText: "GetValue" },
                        ArgumentList.Arguments:
                        [
                            { Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } }
                        ]
                    }
                }
            } => (name1, type1),
            _ => ("", null)
        };

        if (fieldSymbolNameGetter is "" || type is null)
            return;

        if (fieldSymbolNameGetter != fieldSymbol.Name ||
            !propertySymbol.Type.ToDisplayString().EndsWith(type.ToFullString()))
            return;

        var fieldSymbolNameSetter = setter switch
        {
            // set { return SetValue(xxx, value); }
            {
                Body.Statements:
                [
                    ReturnStatementSyntax
                    {
                        Expression: InvocationExpressionSyntax
                        {
                            Expression: IdentifierNameSyntax { Identifier.ValueText: "SetValue" },
                            ArgumentList.Arguments:
                            [
                                { Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } },
                                { Expression: IdentifierNameSyntax { Identifier.ValueText: "value" } }
                            ]
                        }
                    }
                ]
            } => name1,
            // set => SetValue(xxx, value);
            {
                ExpressionBody.Expression: InvocationExpressionSyntax
                {
                    Expression: IdentifierNameSyntax { Identifier.ValueText: "SetValue" },
                    ArgumentList.Arguments:
                    [
                        { Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } },
                        { Expression: IdentifierNameSyntax { Identifier.ValueText: "value" } }
                    ]
                }
            } => name1,
            _ => ""
        };

        if (fieldSymbolNameSetter is "")
            return;

        if (fieldSymbolNameSetter != fieldSymbol.Name)
            return;

        var diagnosticEx = Diagnostic.Create(_convertible, fieldSymbol.Locations[0], fieldSymbol.Name);
        context.ReportDiagnostic(diagnosticEx);
    }
}
