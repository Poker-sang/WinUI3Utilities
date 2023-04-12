using System;
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
    /// <param name="_"></param>
    public static void PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs _) => UpdateAppTitleMargin(sender, CurrentContext.TitleTextBlock);

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
    /// <param name="_"></param>
    public static void PaneOpening(NavigationView sender, object _) => UpdateAppTitleMargin(sender, CurrentContext.TitleTextBlock);

    /// <inheritdoc cref="DisplayModeChangedTitleBar(FrameworkElement, TextBlock, NavigationView)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.TitleBar"/></term></item>
    /// <item><term><see cref="CurrentContext.TitleTextBlock"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="_"></param>
    public static void DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs _)
        => DisplayModeChangedTitleBar(CurrentContext.TitleBar, CurrentContext.TitleTextBlock, sender);

    /// <summary>
    /// Should be invoked by <see cref="NavigationView.DisplayModeChanged"/><br/>
    /// Update app title's and its text block's margin when <see cref="NavigationView.DisplayMode"/> changed
    /// </summary>
    /// <param name="titleBar"></param>
    /// <param name="textBlock"></param>
    /// <param name="navigationView"></param>
    public static void DisplayModeChangedTitleBar(FrameworkElement titleBar, TextBlock textBlock, NavigationView navigationView)
    {
        var currentMargin = titleBar.Margin;
        titleBar.Margin = navigationView.DisplayMode is NavigationViewDisplayMode.Minimal && navigationView.IsBackButtonVisible is not NavigationViewBackButtonVisible.Collapsed
            ? new() { Left = navigationView.CompactPaneLength * 2, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
            : new Thickness { Left = navigationView.CompactPaneLength, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };

        UpdateAppTitleMargin(navigationView, textBlock);
    }

    /// <summary>
    /// Update app <paramref name="titleTextBlock"/>'s margin when <see cref="NavigationView.DisplayMode"/> changed
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="titleTextBlock"></param>
    public static void UpdateAppTitleMargin(NavigationView navigationView, TextBlock titleTextBlock)
    {
        const int smallLeftIndent = 4, largeLeftIndent = 24;

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            titleTextBlock.TranslationTransition = new();

            titleTextBlock.Translation = (navigationView.DisplayMode is NavigationViewDisplayMode.Expanded && navigationView.IsPaneOpen) ||
                                         navigationView.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new(smallLeftIndent, 0, 0)
                : new System.Numerics.Vector3(largeLeftIndent, 0, 0);
        }
        else
        {
            var currentMargin = titleTextBlock.Margin;

            titleTextBlock.Margin = (navigationView.DisplayMode is NavigationViewDisplayMode.Expanded && navigationView.IsPaneOpen) ||
                     navigationView.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new() { Left = smallLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
                : new Thickness { Left = largeLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };
        }
    }

    /// <summary>
    /// TitleBar type
    /// </summary>
    [Flags]
    public enum TitleBarType
    {
        /// <summary>
        /// Neither
        /// </summary>
        Neither,

        /// <summary>
        /// Use <see cref="TryCustomizeTitleBar(Microsoft.UI.Xaml.Window, FrameworkElement)"/>
        /// </summary>
        Window,

        /// <summary>
        /// Use <see cref="TryCustomizeTitleBar(AppWindowTitleBar)"/>
        /// </summary>
        AppWindow,

        /// <summary>
        /// <strong>Use with Caution</strong><br/>
        /// Use both of <see cref="TryCustomizeTitleBar(Microsoft.UI.Xaml.Window, FrameworkElement)"/> and <see cref="TryCustomizeTitleBar(AppWindowTitleBar)"/>
        /// </summary>
        Both = Window | AppWindow
    }

    /// <summary>
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="TitleBarType.Window"/>
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static bool TryCustomizeTitleBar(Window window, FrameworkElement titleBar)
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
            return false;

        CurrentContext.App.Resources["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
        CurrentContext.App.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);
        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(titleBar);
        return true;
    }

    /// <summary>
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="TitleBarType.AppWindow"/>
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static bool TryCustomizeTitleBar(AppWindowTitleBar appWindowTitleBar)
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
            return false;

        appWindowTitleBar.ExtendsContentIntoTitleBar = true;
        appWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindowTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindowTitleBar.ButtonHoverBackgroundColor = CurrentContext.App.Resources["SystemControlBackgroundBaseLowBrush"].To<SolidColorBrush>().Color;
        appWindowTitleBar.ButtonForegroundColor = CurrentContext.App.Resources["SystemControlForegroundBaseHighBrush"].To<SolidColorBrush>().Color;
        return true;
    }
}
