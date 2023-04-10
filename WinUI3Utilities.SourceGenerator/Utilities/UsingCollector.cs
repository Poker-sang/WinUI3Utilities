using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WinUI3Utilities.SourceGenerator.Utilities;

internal class UsingCollector : CSharpSyntaxWalker
{
    private readonly HashSet<string> _namespaces;

    public UsingCollector(HashSet<string> namespaces) => _namespaces = namespaces;

    public override void VisitUsingDirective(UsingDirectiveSyntax node) => _namespaces.Add(node.Name.ToString());
}