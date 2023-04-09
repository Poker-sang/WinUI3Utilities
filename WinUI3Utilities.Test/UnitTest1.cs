using System.Diagnostics;
using ABI.Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[TestClass]
[SettingsViewModel<AppConfig>(nameof(AppConfig))]
[DependencyProperty<int>("Fuck2")]
[INotifyPropertyChanged]
public partial class UnitTest1 : UserControl
{
    public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(int), typeof(UnitTest1), new PropertyMetadata(null));

    public int A { get => (int)GetValue(AProperty); set => SetValue(AProperty, value); }


    [TestMethod]
    public void TestMethod1()
    {
        Debug.WriteLine(TestProperty2);
    }
    public AppConfig AppConfig { get; } = new(default);
}
