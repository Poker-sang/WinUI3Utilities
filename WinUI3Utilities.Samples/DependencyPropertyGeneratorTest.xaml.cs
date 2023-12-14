using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Samples;

[DependencyProperty<AppConfig>("Prop", DependencyPropertyDefaultValue.UnsetValue, nameof(TestMethod1))]
public sealed partial class DependencyPropertyGeneratorTest : UserControl
{
    public DependencyPropertyGeneratorTest() => InitializeComponent();

    public static void TestMethod1(DependencyObject o, DependencyPropertyChangedEventArgs e) { }
}
