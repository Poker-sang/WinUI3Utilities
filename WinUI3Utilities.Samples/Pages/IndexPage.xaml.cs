using System;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities.Samples.Pages;

public sealed partial class IndexPage : Page, ITypeGetter
{
    public static Type TypeGetter => typeof(IndexPage);

    public IndexPage() => InitializeComponent();
}
