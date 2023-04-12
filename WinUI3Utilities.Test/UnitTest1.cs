using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[TestClass]
[SettingsViewModel<AppConfig>(nameof(Config))]
[AppContext<AppConfig>(CastMethod = "WinUI3Utilities.Misc.ToNotNull")]
public partial class UnitTest1 : ObservableObject
{
    [TestMethod]
    public void TestMethod1()
    {
        Debug.Assert(TestProperty2 == default);
    }

    public AppConfig Config { get; } = new(default);
}
