using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using WinUI3Utilities.SourceGenerator.Utilities;
using static WinUI3Utilities.SourceGenerator.Utilities.SourceGeneratorHelper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class AppContextGenerator : TypeWithAttributeGenerator
{
    internal override string AttributeName => "AppContextAttribute`1";

    internal override string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var namespaces = new HashSet<string>();
        var specificType = typeSymbol.ToDisplayString();
        var classBegin =
            $$"""

              namespace {{typeSymbol.ContainingNamespace.ToDisplayString()}};

              partial class {{typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}}
              {
              """;
        const string classEnd = "}";
        var containers = new StringBuilder();
        var keys = new StringBuilder();
        var initializeMethods = new StringBuilder();
        var loadMethods = new StringBuilder();
        var saveMethods = new StringBuilder();
        foreach (var attribute in attributeList)
        {
            if (attribute.AttributeClass is not { TypeArguments: [var type, ..] })
                return null;

            var configKey = "Configuration";
            var methodName = "Configuration";
            var applicationDataContainerType = "LocalSettings";
            var createDispositionValue = "Always";
            var staticClassName = "static WinUI3Utilities.Misc";
            var castMethod = "ToNotNull";

            foreach (var namedArgument in attribute.NamedArguments)
                if (namedArgument.Value.Value is { } value)
                    switch (namedArgument.Key)
                    {
                        case "ConfigKey":
                            configKey = (string)value;
                            break;
                        case "MethodName":
                            methodName = (string)value;
                            break;
                        case "Type" when namedArgument.Value is { Kind: TypedConstantKind.Enum, Value: int containerType and (0 or 1) }:
                            applicationDataContainerType =
                                containerType switch
                                {
                                    0 => "LocalSettings",
                                    _ => "RoamingSettings"
                                };
                            break;
                        case "CreateDisposition" when namedArgument.Value is { Kind: TypedConstantKind.Enum, Value: int createDisposition and (0 or 1) }:
                            createDispositionValue =
                                createDisposition switch
                                {
                                    0 => "Always",
                                    _ => "Existing"
                                };
                            break;
                        case "CastMethod":
                            var castMethodFullName = (string)value;
                            var dotPosition = castMethodFullName.LastIndexOf('.');
                            if (dotPosition is -1)
                                throw new InvalidDataException($"{namedArgument.Key} must contain the full name.");
                            staticClassName = "static " + castMethodFullName[..dotPosition];
                            castMethod = castMethodFullName[(dotPosition + 1)..];
                            break;
                    }

            var typeName = type.ToDisplayString();
            // castMethod方法所用namespace
            _ = namespaces.Add(staticClassName);
            var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            /*-----Body Begin-----*/
            _ = containers.AppendLine($"{Spacing(1)}private static Windows.Storage.ApplicationDataContainer _container{methodName} = null!;");
            _ = keys.AppendLine($"{Spacing(1)}private const string {methodName}ContainerKey = \"{configKey}\";");
            _ = initializeMethods.AppendLine(
                $$"""
                      public static void Initialize{{methodName}}()
                      {
                          var settings = Windows.Storage.ApplicationData.Current.{{applicationDataContainerType}};
                          if (!settings.Containers.ContainsKey({{specificType}}.{{methodName}}ContainerKey))
                          {
                              _ = settings.CreateContainer({{specificType}}.{{methodName}}ContainerKey, Windows.Storage.ApplicationDataCreateDisposition.{{createDispositionValue}});
                          }
                  
                          _container{{methodName}} = settings.Containers[{{specificType}}.{{methodName}}ContainerKey];
                      }
                  """).AppendLine();
            _ = loadMethods.AppendLine(
                $$"""
                      public static {{typeName}}? Load{{methodName}}()
                      {
                          try
                          {
                              var values = _container{{methodName}}.Values;
                              return new {{typeName}}(
                  """);
            _ = saveMethods.AppendLine(
                  $$"""
                      public static void Save{{methodName}}({{typeName}}? configuration)
                      {
                          if (configuration is { } appConfiguration)
                          {
                              var values = _container{{methodName}}.Values;
                  """);
            foreach (var property in type.GetProperties(attributeList[0].AttributeClass!))
            {
                _ = loadMethods.AppendLine(LoadRecord(property.Name, property.Type.Name, typeName, castMethod));
                _ = saveMethods.AppendLine(SaveRecord(property.Name, property.Type, typeName, castMethod));
                namespaces.UseNamespace(usedTypes, typeSymbol, property.Type);
            }

            // 去除','
            _ = loadMethods.Remove(loadMethods.Length - 3, 1);
            _ = loadMethods.AppendLine(
                """
                           );
                        }
                        catch
                        {
                            return default;
                        }
                    }
                """).AppendLine();
            _ = saveMethods.AppendLine(
                 """
                        }
                    }
                """).AppendLine();

        }

        return namespaces.GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(containers.ToString())
            .AppendLine(keys.ToString())
            .AppendLine(initializeMethods.ToString())
            .AppendLine(loadMethods.ToString())
            .AppendLine(saveMethods.ToString())
            .AppendLine(classEnd)
            .ToString();
    }

    private static string LoadRecord(string propertyName, string type, string typeName, string? methodName) => methodName is null
            ? $"{Spacing(4)}({type})values[nameof({typeName}.{propertyName})],"
            : $"{Spacing(4)}values[nameof({typeName}.{propertyName})].{methodName}<{type}>(),";

    private static string SaveRecord(string name, ITypeSymbol type, string typeName, string? methodName)
    {
        var body = $"values[nameof({typeName}.{name})] = appConfiguration.{name}";
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

    private static readonly HashSet<string> _primitiveTypes =
    [
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
    ];
}
