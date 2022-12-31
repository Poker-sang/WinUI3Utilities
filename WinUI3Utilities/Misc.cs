using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace WinUI3Utilities;

public static class Misc
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? To<T>(this object? obj) where T : notnull => (T?)obj;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ToNotNull<T>(this object? obj) where T : notnull
    {
        if (obj is null)
            ThrowHelper.InvalidCast();
        return (T)obj;
    }

    public static T Get<T>(this WeakReference<T?> target, Func<T> @default) where T : class
    {
        if (!target.TryGetTarget(out var value))
            target.SetTarget(value = @default());
        return value;
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> iEnumerable)
    {
        var observableCollection = new ObservableCollection<T>();
        foreach (var item in iEnumerable)
            observableCollection.Add(item);
        return observableCollection;
    }
}
