using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System;

namespace WinUI3Utilities;

/// <summary>
/// Throw helper
/// </summary>
public static class ThrowHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="actualValue"/>))]</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgumentOutOfRange(object? actualValue = null, string? message = null, [CallerArgumentExpression(nameof(actualValue))] string? paraName = null)
        => throw new ArgumentOutOfRangeException(paraName, actualValue, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidOperation(string? message = null) => throw new InvalidOperationException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidCastException"></exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidCast(string? message = null) => throw new InvalidCastException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void IndexOutOfRange(string? message = null) => throw new IndexOutOfRangeException(message);
}
