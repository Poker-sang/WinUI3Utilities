using Microsoft.UI;
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

#if false

    #region For Window

    /// <summary>
    /// Set title bar button color for window with <see cref="Window"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="useDark">use dark theme</param>
    /// <param name="setAppResources">Set <see cref="Application.Current"/>.Resources["WindowCaptionForeground"]</param>
    public static void SetWindowTitleBarButtonColor(this Window window, bool useDark, bool setAppResources = false)
    {
        if (setAppResources)
            Application.Current.Resources["WindowCaptionForeground"] = useDark ? Colors.White : Colors.Black;

        TriggerTitleBarRepaint(window);
    }

    /// <summary>
    /// Set theme for window with <see cref="Window"/>
    /// </summary>
    /// <remarks>
    /// <strong>Make sure <see cref="Window.Content"/> is <see cref="FrameworkElement"/> and loaded</strong><br/>
    /// <see cref="ElementTheme.Default"/> will be obtained correctly only if <see cref="Application.RequestedTheme"/> has not been changed 
    /// </remarks>
    /// <param name="window"></param>
    /// <param name="theme">SHOULD NOT be <see cref="ElementTheme.Default"/></param>
    public static void SetWindowTheme(this Window window, ElementTheme theme)
    {
        var t = GetElementTheme(theme);
        window.Content.To<FrameworkElement>().RequestedTheme = t;
        window.SetWindowTitleBarButtonColor(t is ElementTheme.Dark);
    }

    /// <summary>
    /// To trigger repaint tracking task id 38044406
    /// </summary>
    /// <param name="window"></param>
    public static void TriggerTitleBarRepaint(this Window window)
    {
        var hWnd = (nint)window.AppWindow.Id.Value;
        var activeWindow = User32.GetActiveWindow();
        if (hWnd == activeWindow)
        {
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)NativeConstant.WA_INACTIVE, 0);
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)NativeConstant.WA_ACTIVE, 0);
        }
        else
        {
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)NativeConstant.WA_ACTIVE, 0);
            _ = User32.SendMessage(hWnd, WindowMessage.WM_ACTIVATE, (nint)NativeConstant.WA_INACTIVE, 0);
        }
    }

    #endregion

    #region For AppWindow

    /// <summary>
    /// Customize title bar if supported
    /// </summary>
    /// <remarks>
    /// Related to <see cref="AppWindow"/>, set <see cref="AppWindowTitleBar.ExtendsContentIntoTitleBar"/> and <see cref="AppWindowTitleBar.IconShowOptions"/>
    /// May need to call <see cref="SetAppWindowTitleBarButtonColor"/>
    /// </remarks>
    public static void SetAppWindowTitleBar(AppWindowTitleBar appWindowTitleBar)
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
            return;
        appWindowTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        appWindowTitleBar.ExtendsContentIntoTitleBar = true;
    }

    /// <summary>
    /// Set title bar button color for window with <see cref="AppWindow"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="useDark">use dark theme</param>
    public static void SetAppWindowTitleBarButtonColor(this Window window, bool useDark)
    {
        var titleBar = window.AppWindow.TitleBar;
        var foreground = useDark ? Colors.White : Colors.Black;
        titleBar.ButtonBackgroundColor = Colors.Transparent;
        titleBar.ButtonForegroundColor = foreground;
        titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        titleBar.ButtonInactiveForegroundColor = useDark ? Color.FromArgb(0xFF, 0x73, 0x73, 0x73) : Color.FromArgb(0xFF, 0x9B, 0x9B, 0x9B);
        titleBar.ButtonHoverBackgroundColor = useDark ? Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D) : Color.FromArgb(0xFF, 0xE9, 0xE9, 0xE9);
        titleBar.ButtonHoverForegroundColor = foreground;
        titleBar.ButtonPressedBackgroundColor = useDark ? Color.FromArgb(0xFF, 0x29, 0x29, 0x29) : Color.FromArgb(0xFF, 0xED, 0xED, 0xED);
        titleBar.ButtonPressedForegroundColor = useDark ? Color.FromArgb(0xFF, 0xA7, 0xA7, 0xA7) : Color.FromArgb(0xFF, 0x5F, 0x5F, 0x5F);// 1 bw
    }

    /// <summary>
    /// Set theme for window with <see cref="AppWindow"/>
    /// </summary>
    /// <remarks>
    /// <strong>Make sure <see cref="Window.Content"/> is <see cref="FrameworkElement"/> and loaded</strong><br/>
    /// <see cref="ElementTheme.Default"/> will be obtained correctly only if <see cref="Application.RequestedTheme"/> has not been changed 
    /// </remarks>
    /// <param name="window"></param>
    /// <param name="theme"></param>
    public static void SetAppWindowTheme(this Window window, ElementTheme theme)
    {
        var t = GetElementTheme(theme);
        window.Content.To<FrameworkElement>().RequestedTheme = t;
        window.SetAppWindowTitleBarButtonColor(t is ElementTheme.Dark);
    }

    #endregion

#endif

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
