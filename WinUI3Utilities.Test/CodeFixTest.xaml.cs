using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities.Test;

public sealed partial class CodeFixTest : UserControl
{
    public CodeFixTest() => InitializeComponent();

    public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(CodeFixTest), typeof(CodeFixTest), new(new CodeFixTest()));

    public CodeFixTest A { get => (CodeFixTest)GetValue(AProperty); set => SetValue(AProperty, value); }

    public static readonly DependencyProperty B = DependencyProperty.Register("D", typeof(int), typeof(CodeFixTest), new PropertyMetadata(1, TestMethod1));

    public int C
    {
        get { return (int)GetValue(B); }
        private set { SetValue(B, value); }
    }

    public static void TestMethod1(DependencyObject o, DependencyPropertyChangedEventArgs e) { }
}
