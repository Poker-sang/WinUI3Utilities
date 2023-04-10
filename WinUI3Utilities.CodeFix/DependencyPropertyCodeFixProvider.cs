using Microsoft.CodeAnalysis.CodeFixes;

namespace WinUI3Utilities.CodeFix;

public abstract class DependencyPropertyCodeFixProvider : CodeFixProvider
{
    protected const string DiagnosticId = "Utilities";

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
}
