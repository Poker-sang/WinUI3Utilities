using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;
using WinUI3Utilities.Samples.Pages;

namespace WinUI3Utilities.Samples;

[WindowSizeHelper]
public sealed partial class MainWindow
{
    public MainWindow() => InitializeComponent();

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

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var isDark = Grid.RequestedTheme is ElementTheme.Dark;
        Grid.RequestedTheme = isDark ? ElementTheme.Light : ElementTheme.Dark;
        AppWindow.TitleBar.PreferredTheme = isDark ? TitleBarTheme.Light : TitleBarTheme.Dark;
    }
}
