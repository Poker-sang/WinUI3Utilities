using System;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

/// <summary>
/// Miscellaneous extension methods
/// </summary>
public static class Misc
{
    /// <summary>
    /// Cast <paramref name="obj"/> to type <typeparamref name="T"/> without null check<br/>
    /// <strong>This is NOT Force conversion</strong>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T To<T>(this object? obj) => (T)obj!;

    /// <summary>
    /// Try get and return the value of <paramref name="target"/>. If <paramref name="target"/> is <see langword="null"/>,
    /// set <paramref name="target"/> to <paramref name="init"/> and return.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="init"></param>
    /// <returns></returns>
    public static T GetOrInit<T>(this WeakReference<T?> target, T init) where T : class
    {
        if (!target.TryGetTarget(out var value))
            target.SetTarget(value = init);
        return value;
    }

    /// <summary>
    /// Get <see cref="FrameworkElement.DataContext"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this FrameworkElement frameworkElement)
        => frameworkElement.DataContext.To<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Tag"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetTag<T>(this FrameworkElement frameworkElement)
        => frameworkElement.Tag.To<T>();

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
    /// Get <see cref="Application.Resources"/>[<paramref name="key"/>] and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="application"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this Application application, object key)
        => application.Resources[key].To<T>();
}
