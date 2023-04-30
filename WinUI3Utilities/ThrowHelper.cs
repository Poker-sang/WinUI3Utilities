using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WinUI3Utilities;

/// <summary>
/// Throw helper
/// </summary>
public static class ThrowHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="System.Exception"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Exception(string? message = null) => throw new(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="obj"/>))]</param>
    /// <exception cref="ArgumentException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Argument<T>(T? obj, string? message = null, [CallerArgumentExpression(nameof(obj))] string? paraName = null)
        => throw new ArgumentException(message, paraName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="actualValue"/>))]</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgumentOutOfRange<T>(T? actualValue, string? message = null, [CallerArgumentExpression(nameof(actualValue))] string? paraName = null)
        => throw new ArgumentOutOfRangeException(paraName, actualValue, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="obj"/>))]</param>
    /// <exception cref="ArgumentNullException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgumentNull<T>(T? obj, string? message = null, [CallerArgumentExpression(nameof(obj))] string? paraName = null)
        => throw new ArgumentNullException(paraName, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidOperationException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidOperation(string? message = null) => throw new InvalidOperationException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidCastException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidCast(string? message = null) => throw new InvalidCastException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="IndexOutOfRangeException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void IndexOutOfRange(string? message = null) => throw new IndexOutOfRangeException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotSupportedException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NotSupported(string? message = null) => throw new NotSupportedException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NullReferenceException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NullReference(string? message = null) => throw new NullReferenceException(message);

    #region With generic return

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="System.Exception"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn Exception<TReturn>(string? message = null) => throw new(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="obj"/>))]</param>
    /// <exception cref="ArgumentException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn Argument<T, TReturn>(T? obj, string? message = null, [CallerArgumentExpression(nameof(obj))] string? paraName = null)
        => throw new ArgumentException(message, paraName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="actualValue"/>))]</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ArgumentOutOfRange<T, TReturn>(T? actualValue, string? message = null, [CallerArgumentExpression(nameof(actualValue))] string? paraName = null)
        => throw new ArgumentOutOfRangeException(paraName, actualValue, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="paraName">[<see cref="CallerArgumentExpressionAttribute"/>(<see langword="nameof"/>(<paramref name="obj"/>))]</param>
    /// <exception cref="ArgumentNullException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ArgumentNull<T, TReturn>(T? obj, string? message = null, [CallerArgumentExpression(nameof(obj))] string? paraName = null)
        => throw new ArgumentNullException(paraName, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidOperationException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn InvalidOperation<TReturn>(string? message = null) => throw new InvalidOperationException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="InvalidCastException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn InvalidCast<TReturn>(string? message = null) => throw new InvalidCastException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="IndexOutOfRangeException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn IndexOutOfRange<TReturn>(string? message = null) => throw new IndexOutOfRangeException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotSupportedException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn NotSupported<TReturn>(string? message = null) => throw new NotSupportedException(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NullReferenceException"/>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn NullReference<TReturn>(string? message = null) => throw new NullReferenceException(message);

    #endregion
}
