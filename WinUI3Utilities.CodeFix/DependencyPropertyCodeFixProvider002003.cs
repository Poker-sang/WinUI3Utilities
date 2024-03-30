using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace WinUI3Utilities.CodeFix;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DependencyPropertyCodeFixProvider002003)), Shared]
public class DependencyPropertyCodeFixProvider002003 : DependencyPropertyCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticId + "002", DiagnosticId + "003"];

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var ancestor = root!.FindToken(context.Span.Start).Parent!.AncestorsAndSelf().ToImmutableArray();

        foreach (var diagnostic in context.Diagnostics)
        {
            var newName = diagnostic.Id switch
            {
                DiagnosticId + "002" => diagnostic.Descriptor.MessageFormat.ToString().Split('\'')[1] + "Property",
                DiagnosticId + "003" => diagnostic.Descriptor.MessageFormat.ToString().Split('\'')[3],
                _ => ""
            };
            context.RegisterCodeFix(
                CodeAction.Create("更改依赖属性的名称", c => RenameField(context.Document, ancestor, newName, c), diagnostic.Id),
                diagnostic);
        }
    }

    private static async Task<Solution> RenameField(Document document, ImmutableArray<SyntaxNode> ancestor, string newName, CancellationToken cancellationToken)
    {
        var declaration = ancestor.OfType<VariableDeclarationSyntax>().First();

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        var fieldSymbol = semanticModel!.GetDeclaredSymbol(declaration, cancellationToken)!;

        var originalSolution = document.Project.Solution;
        var newSolution = await Renamer.RenameSymbolAsync(originalSolution, fieldSymbol, new(), newName, cancellationToken).ConfigureAwait(false);

        return newSolution;
    }
}
