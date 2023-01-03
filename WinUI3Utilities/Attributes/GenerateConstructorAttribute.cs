using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate constructor like <see langword="record"/> for the specified type, according to the properties of it<br/>
/// <example>
/// Example:
/// <code>
/// [<see cref="GenerateConstructorAttribute"/>]<br/>
/// <see langword="public partial class"/> specifiedType
/// {
///     <see langword="public"/> Type1 Property1 { <see langword="get"/>; <see langword="set"/>; }
///     <see langword="public"/> Type2 Property2 { <see langword="get"/>; <see langword="set"/>; }
///     ...
/// }
/// </code>
/// Generate:
/// <code>
/// <see langword="partial class"/> specifiedType
/// {
///     <see langword="public"/> specifiedType(Type1 property1, Type2 property2 ...)
///     {
///         Property1 = property1;
///         Property2 = property2;
///         ...
///     }
/// }
/// </code>
/// </example>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class GenerateConstructorAttribute : Attribute
{

}
