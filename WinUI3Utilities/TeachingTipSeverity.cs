using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Snack bar severity on <see cref="TeachingTip.IconSource"/> (Segoe Fluent Icons font)
/// </summary>
public enum TeachingTipSeverity
{
    /// <summary>
    /// Accept (E10B)
    /// </summary>
    Ok,

    /// <summary>
    /// Info (E946)
    /// </summary>
    Information,

    /// <summary>
    /// Important (E171)
    /// </summary>
    Important,

    /// <summary>
    /// Warning (E7BA)
    /// </summary>
    Warning,

    /// <summary>
    /// ErrorBadge (EA39)
    /// </summary>
    Error,

    /// <summary>
    /// Processing (E9F5)
    /// </summary>
    Processing,

    /// <summary>
    /// <see langword="null"/>
    /// </summary>
    None
}
