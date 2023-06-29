using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Metadata;
using WinUI3Utilities.Internal.PlatformInvoke;

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
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="TitleBarType.Window"/>, set <see cref="Window.ExtendsContentIntoTitleBar"/> and <see cref="Window.SetTitleBar"/>
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static void TryCustomizeTitleBar(Window window, FrameworkElement titleBar)
    {
        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(titleBar);
    }

    /// <summary>
    /// Customize title bar when supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="TitleBarType.AppWindow"/>, set <see cref="AppWindowTitleBar.ExtendsContentIntoTitleBar"/>
    /// </remarks>
    /// <returns>Whether customization of title bar is supported</returns>
    public static void TryCustomizeTitleBar(AppWindowTitleBar appWindowTitleBar)
    {
        appWindowTitleBar.ExtendsContentIntoTitleBar = true;
    }

    /// <summary>
    /// Work when in <see cref="TitleBarType.AppWindow"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="useDark">use dark theme</param>
    public static void SetAppWindowTitleBarButtonColor(Window window, bool useDark)
    {
        window.AppWindow.TitleBar.ButtonForegroundColor = useDark ? Colors.White : Colors.Black;
        window.AppWindow.TitleBar.ButtonHoverBackgroundColor = useDark
            ? new()
            {
                A = 0x33,
                R = 0xFF,
                G = 0xFF,
                B = 0xFF
            }
            : new()
            {
                A = 0x33,
                R = 0,
                G = 0,
                B = 0
            };
    }

    /// <summary>
    /// Work when in <see cref="TitleBarType.Window"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="useDark">use dark theme</param>
    /// <param name="setAppResources">Set <see cref="Application.Current"/>.Resources["WindowCaptionForeground"]</param>
    public static void SetWindowTitleBarButtonColor(Window window, bool useDark, bool setAppResources)
    {
        if (setAppResources)
            Application.Current.Resources["WindowCaptionForeground"] = useDark ? Colors.White : Colors.Black;

        TriggerTitleBarRepaint(window);
    }

    /// <summary>
    /// To trigger repaint tracking task id 38044406
    /// </summary>
    /// <param name="window"></param>
    private static void TriggerTitleBarRepaint(Window window)
    {
        var hWnd = (nint)window.AppWindow.Id.Value;
        var activeWindow = User32.GetActiveWindow();
        if (hWnd == activeWindow)
        {
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)WindowMessage.WA_INACTIVE, 0);
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)WindowMessage.WA_ACTIVE, 0);
        }
        else
        {
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)WindowMessage.WA_ACTIVE, 0);
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)WindowMessage.WA_INACTIVE, 0);
        }
    }
}
