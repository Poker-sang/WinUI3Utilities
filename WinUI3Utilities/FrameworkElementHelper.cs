using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

/// <summary>
/// Extension methods for <see cref="FrameworkElement"/>
/// </summary>
public static class FrameworkElementHelper
{
    /// <summary>
    /// Get <see cref="FrameworkElement.DataContext"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this FrameworkElement frameworkElement)
        => frameworkElement.DataContext.To<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.DataContext"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this object frameworkElement)
        => frameworkElement.To<FrameworkElement>().GetDataContext<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Tag"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetTag<T>(this FrameworkElement frameworkElement)
        => frameworkElement.Tag.To<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Tag"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetTag<T>(this object frameworkElement)
        => frameworkElement.To<FrameworkElement>().GetTag<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Resources"/>[<paramref name="key"/>] and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T GetResource<T>(this FrameworkElement frameworkElement, object key)
        => frameworkElement.Resources[key].To<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Resources"/>[<paramref name="key"/>] and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T GetResource<T>(this object frameworkElement, object key)
        => frameworkElement.To<FrameworkElement>().GetResource<T>(key);
}
