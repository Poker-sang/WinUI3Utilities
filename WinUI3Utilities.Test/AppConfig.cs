using Microsoft.UI.Xaml;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[GenerateConstructor]
public partial class AppConfig
{
    public AppConfig()
    {

    }

    [AttributeIgnore(typeof(SettingsViewModelAttribute<>), typeof(GenerateConstructorAttribute), typeof(AppContextAttribute<>))]
    public bool TestProperty { get; set; } = true;

    public bool TestProperty2 { get; set; } = true;

    public Visibility TestProperty3 { get; set; }
}
