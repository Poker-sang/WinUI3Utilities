using System;

namespace WinUI3Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LocalizedStringResourcesAttribute : Attribute
{
    public LocalizedStringResourcesAttribute(string? fileName = null) => FileName = fileName;

    public string? FileName { get; set; }
}
