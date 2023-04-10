using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WinUI3Utilities.SourceGenerator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static WinUI3Utilities.SourceGenerator.Utilities.Helper;

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
            if (attribute.AttributeClass is not ({ IsGenericType: true } and { TypeArguments.IsDefaultOrEmpty: false }))
                return null;
            var type = attribute.AttributeClass.TypeArguments[0];

            if (attribute.ConstructorArguments.Length < 2 || attribute.ConstructorArguments[0].Value is not string propertyName || attribute.ConstructorArguments[1].Value is not string propertyChanged)
                continue;

            var isSetterPrivate = false;
            var defaultValue = "global::Microsoft.UI.Xaml.DependencyProperty.UnsetValue";
            var isNullable = false;

            foreach (var namedArgument in attribute.NamedArguments)
                if (namedArgument.Value.Value is { } value)
                    switch (namedArgument.Key)
                    {
                        case "IsSetterPrivate":
                            isSetterPrivate = (bool)value;
                            break;
                        case "DefaultValue":
                            defaultValue = (string)value;
                            break;
                        case "IsNullable":
                            isNullable = (bool)value;
                            break;
                    }

            var fieldName = propertyName + "Property";

            var defaultValueExpression = ParseExpression(defaultValue);
            var metadataCreation = GetObjectCreationExpression(defaultValueExpression);
            if (propertyChanged is not "")
                metadataCreation = GetMetadataCreation(metadataCreation, propertyChanged);

            var registration = GetRegistration(propertyName, type, typeSymbol, metadataCreation);
            var staticFieldDeclaration = GetStaticFieldDeclaration(fieldName, registration)
                .AddAttributeLists(GetAttributeForField(nameof(DependencyPropertyGenerator)));
            var getter = GetGetter(fieldName, isNullable, type, typeSymbol);
            var setter = GetSetter(fieldName, isSetterPrivate, typeSymbol);
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
