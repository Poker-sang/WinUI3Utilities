using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WinUI3Utilities.SourceGenerator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static WinUI3Utilities.SourceGenerator.Utilities.SourceGeneratorHelper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class GenerateConstructorGenerator() : TypeWithAttributeGenerator("GenerateConstructorAttribute")
{
    internal override string TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var attribute = attributeList[0];
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        var callParameterlessConstructor = false;
        if (attribute.NamedArguments is [{ Key: "CallParameterlessConstructor", Value.Value: bool value }])
            callParameterlessConstructor = value;

        var ctor = ConstructorDeclaration(name).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
        if (callParameterlessConstructor)
            ctor = ctor.WithInitializer(ConstructorInitializer(SyntaxKind.ThisConstructorInitializer));
        ctor = typeSymbol.GetProperties(attributeList[0].AttributeClass!).Where(t => !t.IsReadOnly && !t.IsStatic).Aggregate(ctor, (current, property) => GetDeclaration(property, current));

        var generatedType = GetDeclaration(name, typeSymbol, ctor);
        var generatedNamespace = GetFileScopedNamespaceDeclaration(typeSymbol, generatedType, true);
        var compilationUnit = GetCompilationUnit(generatedNamespace);
        return SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText().ToString();
    }
}
