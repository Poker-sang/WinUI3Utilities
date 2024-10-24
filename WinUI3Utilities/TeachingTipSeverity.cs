using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Snack bar severity on <see cref="TeachingTip.IconSource"/> (Segoe Fluent Icons font)
/// </summary>
public enum TeachingTipSeverity : ushort
{
    /// <summary>
    /// Accept (E10B)
    /// </summary>
    Ok = 0xE10B,

    /// <summary>
    /// Info (E946)
    /// </summary>
    Information = 0xE946,

    /// <summary>
    /// Important (E171)
    /// </summary>
    Important = 0xE171,

    /// <summary>
    /// Warning (E7BA)
    /// </summary>
    Warning = 0xE7BA,

    /// <summary>
    /// ErrorBadge (EA39)
    /// </summary>
    Error = 0xEA39,

    /// <summary>
    /// Processing (E9F5)
    /// </summary>
    Processing = 0xE9F5,

    /// <summary>
    /// <see langword="null"/>
    /// </summary>
    None = 0
}
