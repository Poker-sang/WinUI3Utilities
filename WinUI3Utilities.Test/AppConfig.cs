using Microsoft.UI.Xaml;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[GenerateConstructor]
public partial class AppConfig
{
    [AttributeIgnore(typeof(SettingsViewModelAttribute<>), typeof(GenerateConstructorAttribute), typeof(AppContextAttribute<>))]
    public bool TestProperty { get; set; } = true;

    public Visibility TestProperty2 { get; set; }
}
