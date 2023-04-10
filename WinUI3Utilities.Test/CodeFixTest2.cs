using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[DependencyProperty<AppConfig>("Fuck2")]
public partial class CodeFixTest2 : UserControl
{

    public static readonly DependencyProperty A = DependencyProperty.Register("B", typeof(int), typeof(CodeFixTest2), new(1));

    public int C { get => (int)GetValue(A); private set => SetValue(A, value); }

    public static void TestMethod1(DependencyObject o, DependencyPropertyChangedEventArgs e) { }
}
