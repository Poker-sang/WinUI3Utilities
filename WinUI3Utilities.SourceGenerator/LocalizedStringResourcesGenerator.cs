using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.CodeAnalysis;
using static WinUI3Utilities.SourceGenerator.Utilities.SourceGeneratorHelper;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class LocalizedStringResourcesGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = AttributeNamespace + "LocalizedStringResourcesAttribute";

    private const string SourceItemGroupMetadata = "build_metadata.AdditionalFiles.SourceItemGroup";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeFullName,
                (_, _) => true,
                (syntaxContext, _) => syntaxContext)
            .Select((t, _) => t.Attributes[0])
            .Combine(context.AdditionalTextsProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Where(t => t.Right.GetOptions(t.Left).TryGetValue(SourceItemGroupMetadata, out var sourceItemGroup)
                            && sourceItemGroup is "PRIResource"
                            && (t.Left.Path.EndsWith(".resw") || t.Left.Path.EndsWith(".resjson")))
                .Select((tuple, _) => tuple.Left).Collect())
            .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(attributes, (spc, triple) =>
        {
            var ((left, right), compilation) = triple;

            if (compilation.Assembly.GetAttributes().Any(attrData => attrData.AttributeClass?.ToDisplayString() == DisableSourceGeneratorAttribute))
                return;

            if (Execute(left, right) is { } s)
                spc.AddSource(AttributeFullName + ".g.cs", s);
        });
    }

    public string? Execute(AttributeData attribute, ImmutableArray<AdditionalText> additionalTexts)
    {
        if (attribute.ConstructorArguments.Length < 1 || attribute.ConstructorArguments[0].Value is not string specifiedNamespace)
            return null;

        var resources = new Dictionary<string, HashSet<string>>();

        foreach (var additionalText in additionalTexts)
        {
            var extension = Path.GetExtension(additionalText.Path);
            var fileName = Path.GetFileNameWithoutExtension(additionalText.Path);

            var resource = resources.TryGetValue(fileName, out var r) ? r : resources[fileName] = [];

            switch (extension)
            {
                case ".resw":
                {
                    var doc = XDocument.Parse(additionalText.GetText()!.ToString());

                    if (doc.XPathSelectElements("//data") is { } elements)
                        foreach (var node in elements)
                            _ = resource.Add(node.Attribute("name")!.Value);
                    break;
                }
                case ".resjson":
                {
                    var doc = JsonDocument.Parse(additionalText.GetText()!.ToString());

                    if (doc.RootElement.EnumerateObject() is var elements)
                        foreach (var node in elements)
                            _ = resource.Add(node.Name);
                    break;
                }
            }
        }

        var source = new StringBuilder(
            $"""
             #nullable enable

             using Microsoft.Windows.ApplicationModel.Resources;

             namespace {specifiedNamespace};

             """);

        foreach (var resource in resources)
        {

            _ = source.AppendLine(
                $$"""

                  public static class {{resource.Key}}Resources
                  {
                      private static readonly ResourceLoader _resourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), "{{resource.Key}}");

                  """);

            foreach (var p in resource.Value) 
                AppendSource(p, source);

            _ = source.AppendLine("}");
        }

        return source.ToString();

        static void AppendSource(string name, StringBuilder sb)
        {
            if (name.IndexOf('[') is var index and not -1)
            {
                var endIndex = name.IndexOf(']');
                name = name.Remove(index, endIndex - index + 1);
            }

            var uid = name.Replace('.', '/');

            _ = sb.AppendLine($$"""{{Spacing(1)}}public static string {{uid.Replace("/", "")}} { get; } = _resourceLoader.GetString("{{uid}}");""");
        }
    }
}
