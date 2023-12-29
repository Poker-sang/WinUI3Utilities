using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;
using WinUI3Utilities.Samples.Pages;

namespace WinUI3Utilities.Samples;

[WindowSizeHelper]
public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        NavigateFrame.GoBack();
        NavigationView.SelectedItem = NavigateFrame.Content switch
        {
            IndexPage => NavigationView.MenuItems[0],
            SettingsPage => NavigationView.SettingsItem,
            _ => NavigationView.SelectedItem
        };
        NavigationView.IsBackEnabled = NavigateFrame.CanGoBack;
    }

    private void NavigationView_OnPaneChanging(NavigationView sender, object args)
    {
        sender.UpdateAppTitleMargin(TitleTextBlock);
    }
}
