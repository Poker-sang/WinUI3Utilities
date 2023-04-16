using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Test.Pages;

namespace WinUI3Utilities.Test;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        CurrentContext.Window = this;
        InitializeComponent();
        CurrentContext.TitleBar = TitleBar;
        CurrentContext.TitleTextBlock = TitleTextBlock;
        // TODO: Microsoft.WindowsAppSDK 1.2后，最小化的NavigationView没有高度
        CurrentContext.NavigationView = NavigationView;
        CurrentContext.Frame = NavigateFrame.To<Frame>();
    }

    private void Loaded(object sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.To<NavigationViewItem>().Tag = typeof(SettingsPage);

        NavigationHelper.GotoPage<IndexPage>();
        NavigationView.SelectedItem = NavigationView.MenuItems[0];
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

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != NavigateFrame.Content.GetType())
            NavigationHelper.GotoPage(item);
    }

    private void TeachingTipOnLoaded(object sender, RoutedEventArgs e) => SnackBarHelper.RootSnackBar = sender.To<TeachingTip>();
}
