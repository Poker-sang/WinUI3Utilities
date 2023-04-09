using Windows.Foundation;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[GenerateConstructor]
public partial class AppConfig
{
    [AttributeIgnore(typeof(SettingsViewModelAttribute<>), typeof(GenerateConstructorAttribute))]
    public bool TestProperty { get; set; } = true;
    public Rect TestProperty2 { get; set; } 
}
