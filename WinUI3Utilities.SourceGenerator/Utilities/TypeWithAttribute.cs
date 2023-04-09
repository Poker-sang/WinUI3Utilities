using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace WinUI3Utilities.SourceGenerator.Utilities;

/// <summary>
/// 对拥有某attribute的type生成代码
/// </summary>
/// <param name="typeSymbol"></param>
/// <param name="attributeList">该类的某种Attribute</param>
/// <returns>生成的代码</returns>
internal delegate string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList);
