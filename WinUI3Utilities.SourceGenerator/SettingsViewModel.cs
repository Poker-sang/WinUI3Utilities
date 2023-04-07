using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using static WinUI3Utilities.SourceGenerator.Utilities;

namespace WinUI3Utilities.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? SettingsViewModel(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var attribute = attributeList[0];

        if (attribute.AttributeClass is not ({ IsGenericType: true } and { TypeArguments.IsDefaultOrEmpty: false }))
            return null;
        var type = attribute.AttributeClass.TypeArguments[0];
        if (attribute.ConstructorArguments[0].Value is not string settingName)
            return null;

        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string>();
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var classBegin = @$"namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {name}
{{";
        var propertySentences = new List<string>();
        const string classEnd = "}";

        foreach (var property in type.GetMembers().Where(property =>
                         property is { Kind: SymbolKind.Property }
                             and not { Name: "EqualityContract" })
                     .Cast<IPropertySymbol>())
        {
            if (IgnoreAttribute(property, attribute.AttributeClass))
                continue;

            namespaces.UseNamespace(usedTypes, typeSymbol, property.Type);
            foreach (var propertyAttribute in property.GetAttributes())
            {
                namespaces.UseNamespace(usedTypes, typeSymbol, propertyAttribute.AttributeClass!);

                foreach (var arg in propertyAttribute.ConstructorArguments)
                {
                    if (arg.Kind is TypedConstantKind.Array)
                        foreach (var value in arg.Values)
                            namespaces.UseNamespace(usedTypes, typeSymbol, (ITypeSymbol)value.Value!);
                    else
                        namespaces.UseNamespace(usedTypes, typeSymbol, (ITypeSymbol)arg.Value!);
                }
            }

            propertySentences.Add(Spacing(1) + Regex.Replace(
                property.DeclaringSyntaxReferences[0].GetSyntax().ToString(), @"\s*{[\s\S]+}[\s\S]*",
                $@"
    {{
        get => {settingName}.{property.Name};
        set => SetProperty({settingName}.{property.Name}, value, {settingName}, (@setting, @value) => @setting.{property.Name} = @value);
    }}"));
        }

        var allPropertySentences = propertySentences.Aggregate("", (current, ps) => current + $"{ps}\n\n");
        allPropertySentences = allPropertySentences[..^2];
        return namespaces.GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(allPropertySentences)
            .AppendLine(classEnd)
            .ToString();
    }
}
