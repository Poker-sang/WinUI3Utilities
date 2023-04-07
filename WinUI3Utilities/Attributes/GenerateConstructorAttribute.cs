using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Generate constructor like <see langword="record"/> for the specified type, according to the properties of it<br/>
/// <strong>Use <see cref="AttributeIgnoreAttribute"/> to indicate which properties are ignored</strong><br/>
/// Example:
/// <code>
/// [<see cref="GenerateConstructorAttribute"/>]<br/>
/// <see langword="public partial class"/> specifiedType
/// {
///     [<see cref="AttributeIgnoreAttribute"/>(<see langword="typeof"/>(<see cref="GenerateConstructorAttribute"/>))]
///     <see langword="public"/> Type1 Property1 { <see langword="get"/>; <see langword="set"/>; }
///     <see langword="public"/> Type2 Property2 { <see langword="get"/>; <see langword="set"/>; }
///     ...
/// }
/// </code>
/// Generate:
/// <code>
/// <see langword="partial class"/> specifiedType
/// {
///     <see langword="public"/> specifiedType(Type2 property2 ...)
///     {
///         Property2 = property2;
///         ...
///     }
/// }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class GenerateConstructorAttribute : Attribute
{

}
