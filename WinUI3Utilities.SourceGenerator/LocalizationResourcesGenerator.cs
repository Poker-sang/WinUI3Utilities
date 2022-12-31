using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using static WinUI3Utilities.SourceGenerator.Utilities;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class LocalizationResourcesGenerator : IIncrementalGenerator
{
    private const string AttributeName = "WinUI3Utilities.Attributes.LocalizedStringResourcesAttribute";
    private const string SourceItemGroupMetadata = "build_metadata.AdditionalFiles.SourceItemGroup";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName,
                (_, _) => true,
                (syntaxContext, _) => syntaxContext)
            .Combine(context.AdditionalTextsProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Where(t => t.Right.GetOptions(t.Left).TryGetValue(SourceItemGroupMetadata, out var sourceItemGroup)
                            && sourceItemGroup is "PRIResource"
                            && t.Left.Path.EndsWith(".resw"))
                .Select((tuple, _) => tuple.Left).Collect());

        context.RegisterSourceOutput(attributes, (spc, source) => Execute(spc, source.Left, source.Right));
    }

    public void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext asc, ImmutableArray<AdditionalText> additionalTexts)
    {
        for (var i = 0; i < asc.Attributes.Length; ++i)
        {
            var attribute = asc.Attributes[i];
            var fileName = Path.GetFileNameWithoutExtension(attribute.ConstructorArguments[0].Value as string) ?? asc.TargetSymbol.Name;
            var extension = Path.GetExtension(attribute.ConstructorArguments[0].Value as string) ?? "resw";
            var additionalText = additionalTexts.SingleOrDefault(t => Path.GetFileNameWithoutExtension(t.Path) == fileName && Path.GetExtension(t.Path) == extension);
            if (additionalText is null)
                continue;
            var doc = XDocument.Parse(additionalText.GetText()?.ToString()!);

            var source = new StringBuilder($@"#nullable enable

using Microsoft.Windows.ApplicationModel.Resources;

namespace {asc.TargetSymbol.ContainingNamespace};

partial class {asc.TargetSymbol.Name} 
{{   
    private static readonly ResourceLoader _resourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), ""{Path.GetFileNameWithoutExtension(additionalText.Path)}"");
");
            if (doc.XPathSelectElements("//data") is { } elements)
                foreach (var node in elements)
                {
                    var name = node.Attribute("name")!.Value;
                    if (name.Contains("["))
                        continue;

                    _ = source.AppendLine(@$"{Spacing(1)}public readonly string {new Regex(@"\.|\:|\[|\]").Replace(name, "")} = _resourceLoader.GetString(""{name.Replace('.', '/')}"");");
                }

            _ = source.AppendLine("}");
            spc.AddSource($"{asc.TargetSymbol.Name}{i}.StringResources.g.cs", source.ToString());
        }
    }
}
