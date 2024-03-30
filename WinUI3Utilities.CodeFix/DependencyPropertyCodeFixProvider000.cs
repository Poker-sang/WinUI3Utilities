using System.Collections.Immutable;
using System.Composition;
using System.Linq;
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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DependencyPropertyCodeFixProvider000)), Shared]
public class DependencyPropertyCodeFixProvider000 : DependencyPropertyCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticId + "000"];

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var ancestor = root!.FindToken(context.Span.Start).Parent!.AncestorsAndSelf().ToImmutableArray();

        var fieldName = diagnostic.Descriptor.MessageFormat.ToString().Split('\'')[1];
        if (!fieldName.EndsWith("Property"))
            return;

        context.RegisterCodeFix(
            CodeAction.Create("添加对应属性", c => AddNewProperty(context.Document, ancestor, fieldName, c), diagnostic.Id),
            diagnostic);
    }

    private static async Task<Document> AddNewProperty(Document document, ImmutableArray<SyntaxNode> ancestor, string fieldName, CancellationToken cancellationToken)
    {
        var fieldDeclaration = ancestor.OfType<FieldDeclarationSyntax>().First();
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        var variableDeclarator = ancestor.OfType<VariableDeclaratorSyntax>().First();

        if (variableDeclarator is not
            {
                Initializer.Value: InvocationExpressionSyntax
                {
                    ArgumentList.Arguments: [{ Expression: { } expression }, { Expression: TypeOfExpressionSyntax { Type: { } typeSyntax } }, _, _]
                }
            })
            throw new();

        var typeSymbol = (ITypeSymbol)semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol!;
        var getter = GetGetter(fieldName, typeSymbol);
        var setter = GetSetter(fieldName);

        var name = expression switch
        {
            LiteralExpressionSyntax { Token.ValueText: var name1 } => name1,
            InvocationExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" },
                ArgumentList.Arguments: [{ Expression: IdentifierNameSyntax { Identifier.ValueText: var name1 } }]
            } => name1,
            _ => ""
        };

        var propertyDeclaration = GetPropertyDeclaration(name, typeSymbol, getter, setter);

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.InsertAfter(fieldDeclaration, propertyDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Generate the following code
    /// <code>
    /// get => (<paramref name="type" />)GetValue(<paramref name="fieldName" />);
    /// </code>
    /// </summary>
    /// <returns>Getter</returns>
    internal static AccessorDeclarationSyntax GetGetter(string fieldName, ITypeSymbol type)
    {
        ExpressionSyntax getProperty = InvocationExpression(IdentifierName("GetValue"))
            .AddArgumentListArguments(Argument(IdentifierName(fieldName)));
        if (type.SpecialType != SpecialType.System_Object)
            getProperty = CastExpression(ParseTypeName(type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)), getProperty);

        return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithExpressionBody(ArrowExpressionClause(getProperty))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    /// <summary>
    /// Generate the following code
    /// <code>
    /// set => SetValue(<paramref name="fieldName" />, value);
    /// </code>
    /// </summary>
    /// <returns>Setter</returns>
    internal static AccessorDeclarationSyntax GetSetter(string fieldName)
    {
        ExpressionSyntax setProperty = InvocationExpression(IdentifierName("SetValue"))
            .AddArgumentListArguments(Argument(IdentifierName(fieldName)), Argument(IdentifierName("value")));
        return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithExpressionBody(ArrowExpressionClause(setProperty))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    /// <summary>
    /// Generate the following code
    /// <code>
    /// public <paramref name="type" /> <paramref name="propertyName" /> { <paramref name="getter" />; <paramref name="setter" />; }
    /// </code>
    /// </summary>
    /// <returns>PropertyDeclaration</returns>
    internal static PropertyDeclarationSyntax GetPropertyDeclaration(string propertyName, ITypeSymbol type, AccessorDeclarationSyntax getter, AccessorDeclarationSyntax setter)
        => PropertyDeclaration(ParseTypeName(type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)), propertyName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(getter, setter);

}
