using System;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// A set of method for <see cref="NavigationView"/>
/// </summary>
public static class NavigationHelper
{
    /// <summary>
    /// Goto Page <typeparamref name="T"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.NavigationView"/></term></item>
    /// <item><term><see cref="CurrentContext.Frame"/></term></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter">Parameter passed to <typeparamref name="T"/></param>
    public static void GotoPage<T>(object? parameter = null) where T : Page => GotoPage(typeof(T), parameter);

    /// <summary>
    /// Goto Page <paramref name="page"/>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.NavigationView"/></term></item>
    /// <item><term><see cref="CurrentContext.Frame"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="page"></param>
    /// <param name="parameter">Parameter passed to <paramref name="page"/></param>
    public static void GotoPage(Type page, object? parameter = null)
    {
        _ = CurrentContext.Frame.Navigate(page, parameter);
        CurrentContext.NavigationView.IsBackEnabled = CurrentContext.Frame.CanGoBack;
        GC.Collect();
    }
}
