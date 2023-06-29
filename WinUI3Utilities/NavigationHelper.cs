using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace WinUI3Utilities;

/// <summary>
/// A set of method for <see cref="Frame"/> and <see cref="NavigationView"/>
/// </summary>
public static class NavigationHelper
{
    /// <inheritdoc cref="GotoPage"/>
    public static void GotoPage<T>(object? parameter = null, NavigationTransitionInfo? info = null, NavigationView? navigationView = null, Frame? frame = null) where T : Page
        => GotoPage(typeof(T), parameter, info, navigationView, frame);

    /// <summary>
    /// Goto Page
    /// </summary>
    /// <param name="page"></param>
    /// <param name="parameter"></param>
    /// <param name="info"></param>
    /// <param name="navigationView">Default: <see cref="CurrentContext.NavigationView"/></param>
    /// <param name="frame">Default: <see cref="CurrentContext.Frame"/></param>
    public static void GotoPage(Type page, object? parameter = null, NavigationTransitionInfo? info = null, NavigationView? navigationView = null, Frame? frame = null)
    {
        navigationView ??= CurrentContext.NavigationView;
        frame ??= CurrentContext.Frame;
        _ = frame.Navigate(page, parameter, info);
        navigationView.IsBackEnabled = frame.CanGoBack;
    }

    /// <summary>
    /// Navigate to <typeparamref name="T"/>
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="parameter"></param>
    /// <param name="info"></param>
    /// <param name="navigationView">Set <seealso cref="NavigationView.IsBackEnabled"/></param>
    public static void Navigate<T>(this Frame frame, object? parameter = null, NavigationTransitionInfo? info = null, NavigationView? navigationView = null) where T : Page
    {
        _ = frame.Navigate(typeof(T), parameter, info);
        if (navigationView is not null)
            navigationView.IsBackEnabled = frame.CanGoBack;
    }
}
