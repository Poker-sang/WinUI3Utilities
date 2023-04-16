using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[DependencyProperty<AppConfig>("Prop", nameof(TestMethod1))]
public sealed partial class DependencyPropertyGeneratorTest : UserControl
{
    public DependencyPropertyGeneratorTest() => InitializeComponent();

    public static void TestMethod1(DependencyObject o, DependencyPropertyChangedEventArgs e) { }
}
