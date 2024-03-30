using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Metadata;

namespace WinUI3Utilities;

/// <summary>
/// Helper to set title bar
/// </summary>
public static class TitleBarHelper
{
    /// <summary>
    /// Should be invoked by <see cref="NavigationView.PaneOpening"/> and <see cref="NavigationView.PaneClosing"/><br/>
    /// Update app <paramref name="titleTextBlock"/>'s margin
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="titleTextBlock"></param>
    public static void UpdateAppTitleMargin(this NavigationView navigationView, TextBlock titleTextBlock)
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
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="Window"/>, set <see cref="Window.ExtendsContentIntoTitleBar"/> when <see cref="AppWindowTitleBar.IsCustomizationSupported"/>.
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static void SetWindowTitleBar(this Window window)
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
            return;
        window.ExtendsContentIntoTitleBar = true;
    }

    #region For theme

    /// <summary>
    /// If given <see cref="ElementTheme.Default"/>, returns <see cref="GetDefaultTheme"/>
    /// </summary>
    /// <remarks>
    /// <see cref="ElementTheme.Default"/> will be obtained correctly only if <see cref="Application.RequestedTheme"/> has not been changed 
    /// </remarks>
    /// <param name="theme"></param>
    /// <returns><see cref="ElementTheme.Light"/> or <see cref="ElementTheme.Dark"/></returns>
    public static ElementTheme GetElementTheme(ElementTheme theme)
    {
        return theme switch
        {
            ElementTheme.Dark => ElementTheme.Dark,
            ElementTheme.Light => ElementTheme.Light,
            ElementTheme.Default => GetDefaultTheme(),
            _ => ThrowHelper.ArgumentOutOfRange<ElementTheme, ElementTheme>(theme)
        };
    }

    /// <summary>
    /// Returns <see cref="ElementTheme.Light"/> or <see cref="ElementTheme.Dark"/> based on <see cref="Application.RequestedTheme"/>
    /// </summary>
    /// <remarks>
    /// <see cref="ElementTheme.Default"/> will be obtained correctly only if <see cref="Application.RequestedTheme"/> has not been changed 
    /// </remarks>
    /// <returns><see cref="ElementTheme.Light"/> or <see cref="ElementTheme.Dark"/></returns>
    public static ElementTheme GetDefaultTheme()
    {
        return Application.Current.RequestedTheme switch
        {
            ApplicationTheme.Light => ElementTheme.Light,
            ApplicationTheme.Dark => ElementTheme.Dark,
            _ => ThrowHelper.ArgumentOutOfRange<ApplicationTheme, ElementTheme>(Application.Current.RequestedTheme)
        };
    }

    #endregion
}
