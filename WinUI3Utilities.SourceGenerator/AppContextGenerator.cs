using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using WinUI3Utilities.SourceGenerator.Utilities;
using static WinUI3Utilities.SourceGenerator.Utilities.Helper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class AppContextGenerator : TypeWithAttributeGenerator
{
    internal override string AttributeName => "AppContextAttribute`1";

    internal override string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var attribute = attributeList[0];

        if (attribute.AttributeClass is not { TypeArguments: [var type, ..] })
            return null;

        var staticClassName = "static WinUI3Utilities.Misc";
        var methodName = "ToNotNull";

        string? configKey = null;

        foreach (var namedArgument in attribute.NamedArguments)
            if (namedArgument.Value.Value is { } value)
                switch (namedArgument.Key)
                {
                    case "ConfigKey":
                        configKey = (string)value;
                        break;
                    case "CastMethod":
                        var castMethodFullName = (string)value;
                        var dotPosition = castMethodFullName.LastIndexOf('.');
                        if (dotPosition is -1)
                            throw new InvalidDataException($"{namedArgument.Key} must contain the full name.");
                        staticClassName = "static " + castMethodFullName[..dotPosition];
                        methodName = castMethodFullName[(dotPosition + 1)..];
                        break;
                }

        configKey ??= "Configuration";

        var specificType = typeSymbol.ToDisplayString();
        var typeName = type.ToDisplayString();
        var namespaces = new HashSet<string>();
        // methodName方法所用namespace
        _ = namespaces.Add(staticClassName);
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        /*-----Body Begin-----*/
        var classBegin = @$"
namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}
{{
    private static Windows.Storage.ApplicationDataContainer _configurationContainer = null!;

    private const string ConfigurationContainerKey = ""{configKey}"";

    public static void InitializeConfigurationContainer()
    {{
        var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        if (!roamingSettings.Containers.ContainsKey({specificType}.ConfigurationContainerKey))
            _ = roamingSettings.CreateContainer({specificType}.ConfigurationContainerKey, Windows.Storage.ApplicationDataCreateDisposition.Always);

        _configurationContainer = roamingSettings.Containers[{specificType}.ConfigurationContainerKey];
    }}

    public static {typeName}? LoadConfiguration()
    {{
        try
        {{
            return new {typeName}(";
        /*-----Splitter-----*/
        var loadConfigurationContent = new StringBuilder();
        /*-----Splitter-----*/
        var loadConfigurationEndAndSaveConfigurationBegin = $@"           );
        }}
        catch
        {{
            return null;
        }}
    }}

    public static void SaveConfiguration({typeName}? configuration)
    {{
        if (configuration is {{ }} appConfiguration)
        {{";
        /*-----Splitter-----*/
        var saveConfigurationContent = new StringBuilder();
        /*-----Splitter-----*/
        const string saveConfigurationEndAndClassEnd = $@"        }}
    }}
}}";
        /*-----Body End-----*/
        foreach (var property in type.GetProperties(attributeList[0].AttributeClass!))
        {
            _ = loadConfigurationContent.AppendLine(LoadRecord(specificType, property.Name, property.Type.Name, typeName, methodName));
            _ = saveConfigurationContent.AppendLine(SaveRecord(specificType, property.Name, property.Type, typeName, methodName));
            namespaces.UseNamespace(usedTypes, typeSymbol, property.Type);
        }

        // 去除" \r\n"
        _ = loadConfigurationContent.Remove(loadConfigurationContent.Length - 3, 3);

        return namespaces.GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(loadConfigurationContent.ToString())
            .AppendLine(loadConfigurationEndAndSaveConfigurationBegin)
            // saveConfigurationContent 后已有空行
            .Append(saveConfigurationContent)
            .AppendLine(saveConfigurationEndAndClassEnd)
            .ToString();
    }

    private static string LoadRecord(string specificType, string propertyName, string type, string typeName, string? methodName) => methodName is null
            ? $"{Spacing(4)}({type}){specificType}._configurationContainer.Values[nameof({typeName}.{propertyName})],"
            : $"{Spacing(4)}{specificType}._configurationContainer.Values[nameof({typeName}.{propertyName})].{methodName}<{type}>(),";

    private static string SaveRecord(string specificType, string name, ITypeSymbol type, string typeName, string? methodName)
    {
        var body = $"{specificType}._configurationContainer.Values[nameof({typeName}.{name})] = appConfiguration.{name}";
        return !_primitiveTypes.Contains(type.Name)
            ? type switch
            {
                { Name: nameof(String) } => $"{Spacing(3)}{body} ?? string.Empty;",
                { TypeKind: TypeKind.Enum } => methodName is null
                    ? $"{Spacing(3)}(int)({body});"
                    : $"{Spacing(3)}{body}.{methodName}<int>();",
                _ => throw new InvalidCastException("Only primitive and Enum types are supported.")
            }
            : $"{Spacing(3)}{body};";
    }

    private static readonly HashSet<string> _primitiveTypes = new()
    {
        nameof(SByte),
        nameof(Byte),
        nameof(Int16),
        nameof(UInt16),
        nameof(Int32),
        nameof(UInt32),
        nameof(Int64),
        nameof(UInt64),
        nameof(Single),
        nameof(Double),
        nameof(Boolean),
        nameof(Char),
        nameof(DateTime),
        nameof(TimeSpan),
        nameof(Guid),
        nameof(DateTimeOffset)
    };
}
