using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WinUI3Utilities.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyPropertyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "Utilities000";
    public const string DiagnosticId2 = "Utilities001";
    public const string DiagnosticId3 = "Utilities002";

    private static readonly LocalizableString _titleMissingProperty =
        new LocalizableResourceString(nameof(Resources.AnalyzerTitleMissingProperty), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _messageFormatMissingProperty =
        new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormatMissingProperty), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _descriptionMissingProperty =
        new LocalizableResourceString(nameof(Resources.AnalyzerDescriptionMissingProperty), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _titleConvertible =
        new LocalizableResourceString(nameof(Resources.AnalyzerTitleConvertible), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _messageFormatConvertible =
        new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormatConvertible), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _descriptionConvertible =
        new LocalizableResourceString(nameof(Resources.AnalyzerDescriptionConvertible), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _titleWrongNaming =
        new LocalizableResourceString(nameof(Resources.AnalyzerTitleWrongNaming), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _messageFormatWrongNaming =
        new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormatWrongNaming), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _descriptionWrongNaming =
        new LocalizableResourceString(nameof(Resources.AnalyzerDescriptionWrongNaming), Resources.ResourceManager,
            typeof(Resources));

    private static readonly DiagnosticDescriptor _missingProperty = new(DiagnosticId, _titleMissingProperty,
        _messageFormatMissingProperty, "Property", DiagnosticSeverity.Warning, true, _descriptionMissingProperty);

    private static readonly DiagnosticDescriptor _convertible = new(DiagnosticId2, _titleConvertible,
        _messageFormatConvertible, "Refactor", DiagnosticSeverity.Warning, true, _descriptionConvertible);

    private static readonly DiagnosticDescriptor _wrongNaming = new(DiagnosticId3, _titleWrongNaming,
        _messageFormatWrongNaming, "Naming", DiagnosticSeverity.Warning, true, _descriptionWrongNaming);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_missingProperty, _convertible, _wrongNaming);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var fieldSymbol = (IFieldSymbol)context.Symbol;
        var propertyName = "";
        var propertySymbol = (IPropertySymbol)null!;

        if (fieldSymbol.Name.EndsWith("Property"))
        {
            var found = false;
            foreach (var symbol in fieldSymbol.ContainingType.GetMembers()
                         .Where(t => t.Kind is SymbolKind.Property)
                         .Cast<IPropertySymbol>())
            {
                if (symbol.Name == fieldSymbol.Name[..^8])
                {
                    propertyName = symbol.Name;
                    propertySymbol = symbol;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var diagnostic = Diagnostic.Create(_missingProperty, fieldSymbol.Locations[0], fieldSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

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
                        Expression: IdentifierNameSyntax
                        {
                            Identifier.ValueText: "DependencyProperty" // dp
                        },
                        Name.Identifier.ValueText: "Register" // register
                    },
                    ArgumentList.Arguments:
                    [
                        {
                            Expression:
                            LiteralExpressionSyntax
                            {
                                Token.ValueText: not null
                            }
                            or
                            InvocationExpressionSyntax
                            {
                                Expression: IdentifierNameSyntax
                                {
                                    Identifier.ValueText: "nameof"
                                },
                                ArgumentList.Arguments: [_]
                            }
                        } first,
                        {
                            Expression: TypeOfExpressionSyntax
                            {
                                Type: { } dpType
                            }
                        },
                        {
                            Expression: TypeOfExpressionSyntax
                            {
                                Type: { } classType
                            }
                        },
                        {
                            Expression: ObjectCreationExpressionSyntax
                            {
                                Type: IdentifierNameSyntax
                                {
                                    Identifier.ValueText: "PropertyMetadata" // p meta
                                }
                            }
                        }
                    ]
                }
            })
            return;
        if (!fieldSymbol.Name.EndsWith("Property"))
        {
            var diagnostic = Diagnostic.Create(_wrongNaming, fieldSymbol.Locations[0], fieldSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (!propertySymbol!.Type.ToDisplayString().EndsWith(dpType.ToFullString()))
            return;
        if (!fieldSymbol.ContainingType!.ToDisplayString().EndsWith(classType.ToFullString()))
            return;
        switch (first.Expression)
        {
            case LiteralExpressionSyntax
            {
                Token.ValueText: var name
            }:
            {
                if (name != propertyName)
                    return;
                break;
            }
            case InvocationExpressionSyntax
            {
                Expression: IdentifierNameSyntax
                {
                    Identifier.ValueText: "nameof"
                },
                ArgumentList.Arguments:
                [
                    {
                        Expression: IdentifierNameSyntax
                        {
                            Identifier.ValueText: var name
                        }
                    }
                ]
            }:
            {
                if (name != propertyName)
                    return;
                break;
            }
        }

        if (propertySymbol.DeclaringSyntaxReferences[0].GetSyntax() is not PropertyDeclarationSyntax
            {
                AccessorList.Accessors:
                [
                    {
                        Body.Statements:
                        [
                            ReturnStatementSyntax
                            {
                                Expression: CastExpressionSyntax
                                {
                                    Expression: InvocationExpressionSyntax
                                    {
                                        Expression: IdentifierNameSyntax
                                        {
                                            Identifier.ValueText: "GetValue"
                                        },
                                        ArgumentList.Arguments: [_]
                                    } // get { return (x)GetValue(xxx); }
                                    // or IdentifierNameSyntax // get { (x)return xxx; }
                                }
                            }
                        ]
                    } or
                    {
                        ExpressionBody.Expression: CastExpressionSyntax
                        {
                            Expression: InvocationExpressionSyntax
                            {
                                Expression: IdentifierNameSyntax
                                {
                                    Identifier.ValueText: "GetValue"
                                },
                                ArgumentList.Arguments: [_]
                            } // get => (x)GetValue(xxx); 
                            // or IdentifierNameSyntax // get => (x)xxx ;
                        }
                    },
                    {
                        Body.Statements:
                        [
                            ReturnStatementSyntax
                            {
                                Expression: InvocationExpressionSyntax
                                {
                                    Expression: IdentifierNameSyntax
                                    {
                                        Identifier.ValueText: "SetValue"
                                    },
                                    ArgumentList.Arguments:
                                    [
                                        _,
                                        {
                                            Expression: IdentifierNameSyntax
                                            {
                                                Identifier.ValueText: "value"
                                            }
                                        }
                                    ]
                                }
                            }
                        ] // set { return SetValue(xxx, value); }
                    } or
                    {
                        ExpressionBody.Expression: InvocationExpressionSyntax
                        {
                            Expression: IdentifierNameSyntax
                            {
                                Identifier.ValueText: "SetValue"
                            },
                            ArgumentList.Arguments:
                            [
                                _,
                                {
                                    Expression: IdentifierNameSyntax
                                    {
                                        Identifier.ValueText: "value"
                                    }
                                }
                            ]
                        } // set => SetValue(xxx, value);
                    }
                ] accessors
            })
            return;

        switch (accessors[0])
        {
            case
            {
                Body.Statements:
                [
                    ReturnStatementSyntax
                    {
                        Expression: CastExpressionSyntax
                        {
                            Type: var type,
                            Expression: InvocationExpressionSyntax
                            {
                                ArgumentList.Arguments:
                                [
                                    {
                                        Expression: IdentifierNameSyntax
                                        {
                                            Identifier.ValueText: var name
                                        }
                                    }
                                ]
                            }
                        }
                    }
                ] // get { return GetValue(xxx); }
            }:
            {
                if (name != fieldSymbol.Name || !propertySymbol!.Type.ToDisplayString().EndsWith(type.ToFullString()))
                    return;
                break;
            }
            case
            {
                ExpressionBody.Expression: CastExpressionSyntax
                {
                    Type: var type,
                    Expression: InvocationExpressionSyntax
                    {
                        ArgumentList.Arguments:
                        [
                            {
                                Expression: IdentifierNameSyntax
                                {
                                    Identifier.ValueText: var name
                                }
                            }
                        ]
                    } // get => GetValue(xxx); 
                }
            }:
            {
                if (name != fieldSymbol.Name || !propertySymbol!.Type.ToDisplayString().EndsWith(type.ToFullString()))
                    return;
                break;
            }
        }

        switch (accessors[1])
        {
            case
            {
                Body.Statements:
                [
                    ReturnStatementSyntax
                    {
                        Expression: InvocationExpressionSyntax
                        {
                            ArgumentList.Arguments:
                            [
                                {
                                    Expression: IdentifierNameSyntax
                                    {
                                        Identifier.ValueText: var name
                                    }
                                },
                                _
                            ]
                        }
                    }
                ] // set { return SetValue(xxx, value); }
            }:
            {
                if (name != fieldSymbol.Name)
                    return;
                break;
            }
            case
            {
                ExpressionBody.Expression: InvocationExpressionSyntax
                {
                    ArgumentList.Arguments:
                    [
                        {
                            Expression: IdentifierNameSyntax
                            {
                                Identifier.ValueText: var name
                            }
                        },
                        _
                    ]
                } // set => SetValue(xxx, value);
            }:
            {
                if (name != fieldSymbol.Name)
                    return;
                break;
            }
        }

        var diagnosticEx = Diagnostic.Create(_convertible, fieldSymbol.Locations[0], fieldSymbol.Name);
        context.ReportDiagnostic(diagnosticEx);
    }
}
