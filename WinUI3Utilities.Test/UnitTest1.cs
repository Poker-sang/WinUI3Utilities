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
        Debug.Assert(TestProperty2 == default);
    }

    public AppConfig AppConfig { get; } = new(default);
}
