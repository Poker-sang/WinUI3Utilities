using Microsoft.UI.Xaml;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Samples;

[GenerateConstructor]
public partial class AppConfig
{
    public AppConfig()
    {

    }

    [AttributeIgnore(typeof(SettingsViewModelAttribute<>), typeof(GenerateConstructorAttribute), typeof(AppContextAttribute<>), typeof(AppContextAttribute<,>))]
    public bool TestProperty { get; set; } = true;

    public object TestProperty2 { get; set; } = true;

    public Visibility TestProperty3 { get; set; }
}
