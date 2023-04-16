using System;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities.Test.Pages;

public sealed partial class IndexPage : Page, ITypeGetter
{
    public static Type TypeGetter => typeof(IndexPage);

    public IndexPage() => InitializeComponent();
}
