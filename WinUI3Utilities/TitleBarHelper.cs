using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Metadata;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for title bar
/// </summary>
public static class TitleBarHelper
{
    /// <summary>
    /// Simply call <see cref="UpdateAppTitleMargin"/>, should be invoked by <see cref="NavigationView.PaneClosing"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.TitleTextBlock"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs e) => UpdateAppTitleMargin(sender);

    /// <summary>
    /// Simply call <see cref="UpdateAppTitleMargin"/>, should be invoked by <see cref="NavigationView.PaneOpening"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.TitleTextBlock"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void PaneOpening(NavigationView sender, object e) => UpdateAppTitleMargin(sender);

    /// <summary>
    /// Should be invoked by <see cref="NavigationView.DisplayModeChanged"/><br/>
    /// Update app title's and its text block's margin when <see cref="NavigationView.DisplayMode"/> changed
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.TitleBar"/></term></item>
    /// <item><term><see cref="CurrentContext.TitleTextBlock"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs e)
    {
        var currentMargin = CurrentContext.TitleBar.Margin;
        CurrentContext.TitleBar.Margin = sender.DisplayMode is NavigationViewDisplayMode.Minimal && sender.IsBackButtonVisible is not NavigationViewBackButtonVisible.Collapsed
            ? new() { Left = sender.CompactPaneLength * 2, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
            : new Thickness { Left = sender.CompactPaneLength, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };

        UpdateAppTitleMargin(sender);
    }

    /// <summary>
    /// Update app title text block's margin when <see cref="NavigationView.DisplayMode"/> changed
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.TitleTextBlock"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="navigationView"></param>
    public static void UpdateAppTitleMargin(NavigationView navigationView)
    {
        const int smallLeftIndent = 4, largeLeftIndent = 24;

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            CurrentContext.TitleTextBlock.TranslationTransition = new();

            CurrentContext.TitleTextBlock.Translation = (navigationView.DisplayMode is NavigationViewDisplayMode.Expanded && navigationView.IsPaneOpen) ||
                     navigationView.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new(smallLeftIndent, 0, 0)
                : new System.Numerics.Vector3(largeLeftIndent, 0, 0);
        }
        else
        {
            var currentMargin = CurrentContext.TitleTextBlock.Margin;

            CurrentContext.TitleTextBlock.Margin = (navigationView.DisplayMode is NavigationViewDisplayMode.Expanded && navigationView.IsPaneOpen) ||
                     navigationView.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new() { Left = smallLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
                : new Thickness { Left = largeLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };
        }
    }

    /// <summary>
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.App"/></term></item>
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static bool TryCustomizeTitleBar()
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
            return false;

        CurrentContext.AppTitleBar.ExtendsContentIntoTitleBar = true;
        CurrentContext.AppTitleBar.ButtonBackgroundColor = Colors.Transparent;
        CurrentContext.AppTitleBar.ButtonHoverBackgroundColor = CurrentContext.App.Resources["SystemControlBackgroundBaseLowBrush"].To<SolidColorBrush>().Color;
        CurrentContext.AppTitleBar.ButtonForegroundColor = CurrentContext.App.Resources["SystemControlForegroundBaseHighBrush"].To<SolidColorBrush>().Color;
        return true;
    }
}
