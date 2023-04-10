using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WinUI3Utilities.SourceGenerator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static WinUI3Utilities.SourceGenerator.Utilities.Helper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class GenerateConstructorGenerator : TypeWithAttributeGenerator
{
    internal override string AttributeName => "GenerateConstructorAttribute";

    internal override string TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string>();
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var ctor = ConstructorDeclaration(name).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
        foreach (var property in typeSymbol.GetMembers().Where(member =>
                         member is { Kind: SymbolKind.Property } and not { Name: "EqualityContract" })
                     .Cast<IPropertySymbol>())
        {
            if (IgnoreAttribute(property, attributeList[0].AttributeClass!))
                continue;

            ctor = GetDeclaration(property, ctor);
            namespaces.UseNamespace(usedTypes, typeSymbol, property.Type);
        }

        var generatedType = GetDeclaration(name, typeSymbol, ctor);
        var generatedNamespace = GetFileScopedNamespaceDeclaration(typeSymbol, generatedType);
        var compilationUnit = GetCompilationUnit(generatedNamespace, namespaces);
        return SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText().ToString();
    }
}