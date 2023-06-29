using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static WinUI3Utilities.SourceGenerator.Utilities.SourceGeneratorHelper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class DependencyPropertyGenerator : TypeWithAttributeGenerator
{
    internal override string AttributeName => "DependencyPropertyAttribute`1";

    internal override string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var members = new List<MemberDeclarationSyntax>();

        foreach (var attribute in attributeList)
        {
            if (attribute.AttributeClass is not { TypeArguments: [var type, ..] })
                return null;

            if (attribute.ConstructorArguments is not
                [
                    { Value: string propertyName },
                    var defaultValueArgument,
                    { Value: string propertyChanged },
                    ..
                ])
                continue;

            var isSetterPrivate = false;
            var isNullable = false;

            foreach (var namedArgument in attribute.NamedArguments)
                if (namedArgument.Value.Value is { } value)
                    switch (namedArgument.Key)
                    {
                        case "IsSetterPrivate":
                            isSetterPrivate = (bool)value;
                            break;
                        case "IsNullable":
                            isNullable = (bool)value;
                            break;
                    }

            var defaultValueExpression = (ExpressionSyntax)null!;

            if (defaultValueArgument is { Kind: TypedConstantKind.Enum, Value: int defaultValueType })
            {
                var typeSyntax = type.GetTypeSyntax(isNullable);
                defaultValueExpression = defaultValueType switch
                {
                    0 => ParseExpression("global::Microsoft.UI.Xaml.DependencyProperty.UnsetValue"),
                    1 => DefaultExpression(typeSyntax),
                    2 => ObjectCreationExpression(typeSyntax).AddArgumentListArguments(),
                    _ => throw new ArgumentOutOfRangeException(nameof(defaultValueType), defaultValueType,
                        "\"DefaultValueType\" should be within 0-2.")
                };
            }
            else if (defaultValueArgument is { Value: string defaultValue })
            {
                defaultValueExpression = ParseExpression(defaultValue);
            }
            else
            {
                throw new ArgumentException("\"DefaultValue\" should be enum or string.", nameof(defaultValue));
            }

            var fieldName = propertyName + "Property";

            var metadataCreation = GetObjectCreationExpression(defaultValueExpression);
            if (propertyChanged is not "")
                metadataCreation = GetMetadataCreation(metadataCreation, propertyChanged);

            var registration = GetRegistration(propertyName, type, typeSymbol, metadataCreation);
            var staticFieldDeclaration = GetStaticFieldDeclaration(fieldName, registration)
                .AddAttributeLists(GetAttributeForField(nameof(DependencyPropertyGenerator)));
            var getter = GetGetter(fieldName, isNullable, type);
            var setter = GetSetter(fieldName, isSetterPrivate);
            var propertyDeclaration = GetPropertyDeclaration(propertyName, isNullable, type, getter, setter)
                .AddAttributeLists(GetAttributeForMethod(nameof(DependencyPropertyGenerator)));

            members.Add(staticFieldDeclaration);
            members.Add(propertyDeclaration);
        }

        if (members.Count > 0)
        {
            var generatedClass = GetClassDeclaration(typeSymbol, members);
            var generatedNamespace = GetFileScopedNamespaceDeclaration(typeSymbol, generatedClass);
            var compilationUnit = GetCompilationUnit(generatedNamespace);
            return SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText().ToString();
        }

        return null;
    }
}
