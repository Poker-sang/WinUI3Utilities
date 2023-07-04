using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for easily using <see cref="AppWindowTitleBar.SetDragRectangles"/>
/// </summary>
public static class DragZoneHelper
{
    /// <summary>
    /// Calculate dragging-zones of title bar<br/>
    /// <strong>You MUST transform the rectangles with <see cref="WindowHelper.GetScaleAdjustment(Window)"/> before calling <see cref="AppWindowTitleBar.SetDragRectangles"/></strong>
    /// </summary>
    /// <param name="viewportWidth"></param>
    /// <param name="dragZoneHeight"></param>
    /// <param name="dragZoneLeftIndent"></param>
    /// <param name="nonDraggingZones"></param>
    /// <returns></returns>
    public static IEnumerable<RectInt32> GetDragZones(int viewportWidth, int dragZoneHeight, int dragZoneLeftIndent, IEnumerable<RectInt32> nonDraggingZones)
    {
        var draggingZonesX = new List<Range> { new(dragZoneLeftIndent, viewportWidth) };
        var draggingZonesY = new List<IEnumerable<Range>> { new[] { new Range(0, dragZoneHeight) } };

        foreach (var nonDraggingZone in nonDraggingZones)
        {
            for (var i = 0; i < draggingZonesX.Count; ++i)
            {
                var x = draggingZonesX[i];
                var y = draggingZonesY[i].ToArray();
                var xSubtrahend = new Range(nonDraggingZone.X, nonDraggingZone.X + nonDraggingZone.Width);
                var ySubtrahend = new Range(nonDraggingZone.Y, nonDraggingZone.Y + nonDraggingZone.Height);
                var xResult = (x - xSubtrahend).ToArray();
                if (xResult.Length is 1 && xResult[0] == x)
                    continue;
                var yResult = y - ySubtrahend;
                switch (xResult.Length)
                {
                    case 0:
                        draggingZonesY[i] = yResult;
                        break;
                    case 1:
                        draggingZonesX.RemoveAt(i);
                        draggingZonesY.RemoveAt(i);
                        if (xResult[0].Lower == x.Lower)
                        {
                            draggingZonesY.InsertRange(i, new[] { y, yResult });
                            draggingZonesX.InsertRange(i, new[]
                            {
                                x with { Upper = xResult[0].Upper },
                                x with { Lower = xSubtrahend.Lower }
                            });
                        }
                        else // xResult[0].Upper == x.Upper
                        {
                            draggingZonesY.InsertRange(i, new[] { yResult, y });
                            draggingZonesX.InsertRange(i, new[]
                            {
                                x with { Upper = xSubtrahend.Upper },
                                x with { Lower = xResult[0].Lower }
                            });
                        }
                        break;
                    case 2:
                        draggingZonesX.RemoveAt(i);
                        draggingZonesY.RemoveAt(i);
                        draggingZonesY.InsertRange(i, new[] { y, yResult, y });
                        draggingZonesX.InsertRange(i, new[]
                        {
                            x with { Upper = xResult[0].Upper } ,
                            xSubtrahend,
                            x with { Lower = xResult[1].Lower }
                        });
                        break;
                }

                i += xResult.Length;
            }
        }

        var rects = draggingZonesX
            .SelectMany((rangeX, i) => draggingZonesY[i]
                .Select(rangeY => new RectInt32(rangeX.Lower, rangeY.Lower, rangeX.Distance, rangeY.Distance)))
            .OrderBy(t => t.Y)
            .ThenBy(t => t.X).ToList();
        for (var i = 0; i < rects.Count - 1; ++i)
        {
            var now = rects[i];
            var next = rects[i + 1];
            if (now.Height == next.Height && now.X + now.Width == next.X)
            {
                rects.RemoveRange(i, 2);
                rects.Insert(i, now with { Width = now.Width + next.Width });
            }
        }

        return rects;
    }

    /// <summary>
    /// Set dragging-zones of title bar
    /// </summary>
    /// <param name="info"></param>
    /// <param name="window">Default: <see cref="CurrentContext.Window"/></param>
    public static void SetDragZones(DragZoneInfo info, Window? window = null)
    {
        window ??= CurrentContext.Window;
        var scaleAdjustment = window.GetScaleAdjustment();
        var windowWidth = (int)(window.AppWindow.Size.Width / scaleAdjustment);
        var nonDraggingZones = info.NonDraggingZones;

        if (info.ExcludeDebugToolbarArea)
            nonDraggingZones = nonDraggingZones.Concat(new RectInt32[] { new((windowWidth - DebugToolbarWidth) / 2, 0, DebugToolbarWidth, DebugToolbarHeight) });

        window.AppWindow.TitleBar.SetDragRectangles(
            GetDragZones(windowWidth, info.DragZoneHeight, info.DragZoneLeftIndent, nonDraggingZones)
                .Select(rect => new RectInt32(
                    (int)(rect.X * scaleAdjustment),
                    (int)(rect.Y * scaleAdjustment),
                    (int)(rect.Width * scaleAdjustment),
                    (int)(rect.Height * scaleAdjustment)))
                .ToArray());
    }

    private const int DebugToolbarWidth = 217;
    private const int DebugToolbarHeight = 25;
}

file record Range(int Lower, int Upper)
{
    public int Distance => Upper - Lower;

    private bool Intersects(Range other) => other.Lower <= Upper && other.Upper >= Lower;

    public static IEnumerable<Range> operator -(Range minuend, Range subtrahend)
    {
        if (!minuend.Intersects(subtrahend))
        {
            yield return minuend;
            yield break;
        }
        if (minuend.Lower < subtrahend.Lower)
            yield return minuend with { Upper = subtrahend.Lower };
        if (minuend.Upper > subtrahend.Upper)
            yield return minuend with { Lower = subtrahend.Upper };
    }

    public static IEnumerable<Range> operator -(IEnumerable<Range> minuends, Range subtrahend)
        => minuends.SelectMany(minuend => minuend - subtrahend);
}
