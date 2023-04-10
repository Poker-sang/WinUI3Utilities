using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using WinUI3Utilities.SourceGenerator.Utilities;
using static WinUI3Utilities.SourceGenerator.Utilities.Helper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
internal class SettingsViewModelGenerator : TypeWithAttributeGenerator
{
    internal override string AttributeName => "SettingsViewModelAttribute`1";

    internal override string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var attribute = attributeList[0];

        if (attribute.AttributeClass is not ({ IsGenericType: true } and { TypeArguments.IsDefaultOrEmpty: false }))
            return null;
        var type = attribute.AttributeClass.TypeArguments[0];
        if (attribute.ConstructorArguments[0].Value is not string settingName)
            return null;

        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string>();
        var collector = new UsingCollector(namespaces);
        foreach (var typeDeclaringSyntaxReference in type.DeclaringSyntaxReferences)
            collector.Visit(typeDeclaringSyntaxReference.SyntaxTree.GetRoot());

        var classBegin = @$"namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {name}
{{";
        const string classEnd = "}";

        var allPropertySentences = type.GetMembers()
            .Where(property => property is { Kind: SymbolKind.Property } and not { Name: "EqualityContract" })
            .Cast<IPropertySymbol>()
            .Where(property => !IgnoreAttribute(property, attribute.AttributeClass))
            .Select(property => Spacing(1) + Regex.Replace(property.DeclaringSyntaxReferences[0].GetSyntax().ToString(),
                @"\s*{[\s\S]+}[\s\S]*", $@"
    {{
        get => {settingName}.{property.Name};
        set => SetProperty({settingName}.{property.Name}, value, {settingName}, (@setting, @value) => @setting.{property.Name} = @value);
    }}"))
            .Aggregate("", (current, ps) => current + $"{ps}\n\n")[..^2];

        return namespaces.GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(allPropertySentences)
            .AppendLine(classEnd)
            .ToString();
    }
}