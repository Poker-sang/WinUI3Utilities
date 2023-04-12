using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities.Test;

public class CodeFixTest : UserControl
{
    public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(int), typeof(CodeFixTest), new(null));

    public int A { get => (int)GetValue(AProperty); set => SetValue(AProperty, value); }
}
