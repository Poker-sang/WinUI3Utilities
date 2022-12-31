using System;

namespace WinUI3Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LocalizedStringResourcesAttribute : Attribute
{
    public LocalizedStringResourcesAttribute() { }
}
