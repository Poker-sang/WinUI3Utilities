using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WinUI3Utilities.SourceGenerator.Utilities;

internal class UsingCollector(HashSet<string> namespaces) : CSharpSyntaxWalker
{
    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        if (node.Name?.ToString() is { } name)
            _ = namespaces.Add(name);
    }
}
