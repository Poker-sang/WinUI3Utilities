using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Indicates that the source generator is disabled. This is usually used for debug purpose
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class DisableSourceGeneratorAttribute : Attribute;
