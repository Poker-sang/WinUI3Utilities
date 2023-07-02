using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WinUI3Utilities.CodeFix;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DependencyPropertyCodeFixProvider001)), Shared]
public class DependencyPropertyCodeFixProvider001 : DependencyPropertyCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticId + "001");

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];

        var fieldName = diagnostic.Descriptor.MessageFormat.ToString().Split('\'')[1];
        var propertyName = diagnostic.Descriptor.MessageFormat.ToString().Split('\'')[3];

        if (propertyName + "Property" != fieldName)
            return;

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var ancestor = root!.FindToken(context.Span.Start).Parent!.AncestorsAndSelf().ToImmutableArray();

        context.RegisterCodeFix(
            CodeAction.Create("删除字段和属性，改为使用DependencyPropertyAttribute",
                c => AddNewProperty(context.Document, ancestor, propertyName, c),
                diagnostic.Id),
            diagnostic);
    }

    private static async Task<Document> AddNewProperty(Document document, ImmutableArray<SyntaxNode> ancestor,
        string propertyName, CancellationToken cancellationToken)
    {
        var typeDeclaration = ancestor.OfType<TypeDeclarationSyntax>().First();
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        var fieldDeclaration = ancestor.OfType<FieldDeclarationSyntax>().First();
        var variableDeclarator = ancestor.OfType<VariableDeclaratorSyntax>().First();

        if (variableDeclarator is not
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
                        _,
                    { Expression: TypeOfExpressionSyntax { Type: { } propertyType } },
                        _,
                    {
                        Expression: BaseObjectCreationExpressionSyntax
                        {
                            ArgumentList.Arguments: [{ } defaultValue, ..] arguments
                        }
                    }
                    ]
                }
            })
            throw new();

        var propertyTypeInfo = semanticModel.GetTypeInfo(propertyType);

        var args = new List<AttributeArgumentSyntax>
        {
            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyName))),
            AttributeArgument(defaultValue.Expression switch
            {
                LiteralExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.NullLiteralExpression
                        or (int)SyntaxKind.DefaultLiteralExpression
                    }
                    or MemberAccessExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
                        Name.Identifier.ValueText: "DefaultValue",
                        Expression: MemberAccessExpressionSyntax
                        {
                            Name.Identifier.ValueText: "DependencyProperty"
                        }
                        or IdentifierNameSyntax { Identifier.ValueText: "DependencyProperty" }
                    }
                    => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("DependencyPropertyDefaultValue"), IdentifierName(
                            defaultValue.Expression switch
                            {
                                LiteralExpressionSyntax => "Default",
                                MemberAccessExpressionSyntax => "UnsetValue",
                                _ => throw new()
                            })),
                ObjectCreationExpressionSyntax { Type: { } objectCreationType }
                    when SymbolEqualityComparer.Default
                        // semanticModel.GetTypeInfo(objectCreationType).Type 得到null不知道为什么
                        .Equals(semanticModel.GetSymbolInfo(objectCreationType).Symbol, propertyTypeInfo.Type)
                    => MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression, IdentifierName("DependencyPropertyDefaultValue"),
                        IdentifierName("New")),
                //todo
                _ => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(defaultValue.ToString()))
            })
        };

        if (arguments is [_, { } propertyChangedCallback])
        {
            if (propertyChangedCallback.Expression is IdentifierNameSyntax identifier)
                args.Add(AttributeArgument(NameOfExpression(identifier)));
            else
                throw new();
        }

        var propertyDeclaration = (PropertyDeclarationSyntax)typeDeclaration.Members
            .First(t => t is PropertyDeclarationSyntax { Identifier.ValueText: { } name } && name == propertyName);

        var isSetterPrivate = propertyDeclaration.AccessorList!.Accessors
            .Any(t => t.Keyword.ValueText == "set" && t.Modifiers.Any(m => m.ValueText is "private"));

        if (isSetterPrivate)
            args.Add(AttributeArgument(LiteralExpression(SyntaxKind.TrueLiteralExpression))
                .WithNameEquals(NameEquals("IsSetterPrivate")));

        var isNullable = propertyTypeInfo.Nullability.Annotation is NullableAnnotation.Annotated;

        if (isNullable)
            args.Add(AttributeArgument(LiteralExpression(SyntaxKind.TrueLiteralExpression)));

        var attribute = Attribute(GenericName("DependencyProperty").AddTypeArgumentListArguments(propertyType))
            .AddArgumentListArguments(args.ToArray());

        var newTypeDeclaration = typeDeclaration
            .RemoveNodes(
                new SyntaxNode[] { fieldDeclaration, propertyDeclaration },
                SyntaxRemoveOptions.KeepNoTrivia)!
            .AddAttributeLists(AttributeList().AddAttributes(attribute));

        if (typeDeclaration.Modifiers.All(t => t.ValueText is not "partial"))
            newTypeDeclaration = newTypeDeclaration.AddModifiers(Token(SyntaxKind.PartialKeyword));

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(typeDeclaration, newTypeDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Generate the following code
    /// <code>
    /// nameof(<paramref name="expressionSyntax" />)
    /// </code>
    /// </summary>
    /// <returns>NameOfExpression</returns>
    internal static InvocationExpressionSyntax NameOfExpression(ExpressionSyntax expressionSyntax) =>
        InvocationExpression(IdentifierName("nameof"), ArgumentList().AddArguments(Argument(expressionSyntax)));
}
