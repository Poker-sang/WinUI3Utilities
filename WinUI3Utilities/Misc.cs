using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace WinUI3Utilities;

/// <summary>
/// Miscellaneous extension methods
/// </summary>
public static class Misc
{
    /// <summary>
    /// Cast <paramref name="obj"/> to type <typeparamref name="T"/> without null check<br/>
    /// <strong>This is not Force conversion</strong>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T To<T>(this object? obj) => (T)obj!;

    /// <summary>
    /// Cast of <paramref name="obj"/> to type <typeparamref name="T"/>, throw when <paramref name="obj"/> is <see langword="null"/><br/>
    /// <strong>This is not Force conversion</strong>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ToNotNull<T>(this object? obj) where T : notnull => obj is null ? ThrowHelper.ArgumentNull<object, T>(obj) : (T)obj;

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
    /// To <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        var observableCollection = new ObservableCollection<T>();
        foreach (var item in enumerable)
            observableCollection.Add(item);
        return observableCollection;
    }
}
