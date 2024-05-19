using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using WinUI3Utilities.SourceGenerator.Utilities;
using static WinUI3Utilities.SourceGenerator.Utilities.SourceGeneratorHelper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class AppContextGenerator1() : AppContextGenerator("AppContextAttribute`1");

[Generator]
public class AppContextGenerator2() : AppContextGenerator("AppContextAttribute`2");

public abstract class AppContextGenerator(string attributeName) : TypeWithAttributeGenerator(attributeName)
{
    internal override string TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
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
            var converterTypeName = $"{nameof(WinUI3Utilities)}.SettingsValueConverter";
            if (attribute.AttributeClass is { TypeArguments: [var type, .. var rest] })
            {
                if (rest is [var converterType, ..])
                    converterTypeName = converterType.ToDisplayString();
            }
            else
            {
                continue;
            }

            var configKey = "Configuration";
            var methodName = "Configuration";
            var applicationDataContainerType = "LocalSettings";
            var createDispositionValue = "Always";

            foreach (var namedArgument in attribute.NamedArguments)
                switch (namedArgument.Key, namedArgument.Value)
                {
                    case ("ConfigKey", { Value: string value }):
                        configKey = value;
                        break;
                    case ("MethodName", { Value: string value }):
                        methodName = value;
                        break;
                    case ("Type", { Kind: TypedConstantKind.Enum, Value: int containerType and (0 or 1) }):
                        applicationDataContainerType =
                            containerType switch
                            {
                                0 => "LocalSettings",
                                _ => "RoamingSettings"
                            };
                        break;
                    case ("CreateDisposition", { Kind: TypedConstantKind.Enum, Value: int createDisposition and (0 or 1) }):
                        createDispositionValue =
                            createDisposition switch
                            {
                                0 => "Always",
                                _ => "Existing"
                            };
                        break;
                }

            var typeName = type.ToDisplayString();
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
                              var converter = new {{converterTypeName}}();
                              return new {{typeName}}(
                  """);
            _ = saveMethods.AppendLine(
                  $$"""
                      public static void Save{{methodName}}({{typeName}}? configuration)
                      {
                          if (configuration is { } appConfiguration)
                          {
                              var values = _container{{methodName}}.Values;
                              var converter = new {{converterTypeName}}();
                  """);
            foreach (var property in type.GetProperties(attributeList[0].AttributeClass!).Where(t => !t.IsReadOnly && !t.IsStatic))
            {
                var isNullable = property.Type.NullableAnnotation is NullableAnnotation.Annotated ? "true" : "false";
                _ = loadMethods.AppendLine($"{Spacing(4)}converter.ConvertBack<{property.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString()}>(values[nameof({typeName}.{property.Name})], {isNullable})!,");
                _ = saveMethods.AppendLine($"{Spacing(3)}values[nameof({typeName}.{property.Name})] = converter.Convert(appConfiguration.{property.Name});");
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

        _ = saveMethods.Remove(saveMethods.Length - 2, 2);

        return new HashSet<string>().GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(containers.ToString())
            .AppendLine(keys.ToString())
            .Append(initializeMethods)
            .Append(loadMethods)
            .Append(saveMethods)
            .Append(classEnd)
            .ToString();
    }
}
