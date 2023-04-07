using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[TestClass]
[SettingsViewModel<AppConfig>(nameof(AppConfig))]
public partial class UnitTest1 : ObservableObject
{
    [TestMethod]
    public void TestMethod1()
    {
        Debug.WriteLine(TestProperty2);
    }

    public AppConfig AppConfig { get; } = new(true);
}

[GenerateConstructor]
public partial class AppConfig
{
    [AttributeIgnore(typeof(SettingsViewModelAttribute<>), typeof(GenerateConstructorAttribute))]
    public bool TestProperty { get; set; } = true;
    public bool TestProperty2 { get; set; } = true;
}
