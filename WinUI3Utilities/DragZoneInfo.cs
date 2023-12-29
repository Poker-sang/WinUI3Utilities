#if false
using System.Collections.Generic;
using Windows.Graphics;

namespace WinUI3Utilities;

/// <summary>
/// Info type for <see cref="DragZoneHelper"/>
/// </summary>
public record DragZoneInfo
{
    /// <summary>
    /// Create instance with <see cref="NonDraggingZones"/> set
    /// </summary>
    /// <param name="nonDragZones"></param>
    public DragZoneInfo(params RectInt32[] nonDragZones) => NonDraggingZones = nonDragZones;

    /// <summary>
    /// Default: 48
    /// </summary>
    public int DragZoneHeight { get; set; } = 48;

    /// <summary>
    /// Default: 0
    /// </summary>
    public int DragZoneLeftIndent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<RectInt32> NonDraggingZones { get; set; }

    /// <summary>
    /// Default: <see langword="false"/>
    /// </summary>
    public bool ExcludeDebugToolbarArea { get; set; }
}
#endif
