using System;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

/// <summary>
/// Mode for <inheritdoc cref="DragMoveAndResizeInfo"/>
/// </summary>
[Flags]
public enum DragMoveAndResizeMode
{
    /// <summary>
    /// Enable moving window by dragging <see cref="UIElement"/>
    /// </summary>
    DragMove = 1,

    /// <summary>
    /// Enable resizing window by dragging the border of <see cref="UIElement"/>
    /// </summary>
    Resize,

    /// <summary>
    /// Enable both
    /// </summary>
    Both = DragMove | Resize
}
