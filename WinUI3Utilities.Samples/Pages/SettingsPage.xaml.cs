using System;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities.Samples.Pages;

public sealed partial class SettingsPage : Page, ITypeGetter
{
    public static Type TypeGetter => typeof(SettingsPage);

    public SettingsPage()
    {
        InitializeComponent();
        DragMoveAndResizeHelper.RootPanel = Grid;
    }

    private readonly SettingsViewModel _vm = new();
}
