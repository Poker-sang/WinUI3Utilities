using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Graphics;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for easily using <see cref="AppWindowTitleBar.SetDragRectangles"/>
/// </summary>
public static class DragZoneHelper
{
    /// <summary>
    /// Calculate dragging-zones of <see cref="CurrentContext.Window"/> (title bar)<br/>
    /// <strong>You MUST transform the rectangles with <see cref="WindowHelper.GetScaleAdjustment"/> before calling <see cref="AppWindowTitleBar.SetDragRectangles"/></strong>
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="dragZoneHeight"></param>
    /// <param name="dragZoneLeftIntent"></param>
    /// <param name="nonDraggingZones"></param>
    /// <returns></returns>
    public static IEnumerable<RectInt32> GetDragZones(int dragZoneHeight, int dragZoneLeftIntent, IEnumerable<RectInt32> nonDraggingZones)
    {
        var windowWidth = CurrentContext.AppWindow.Size.Width;
        var draggingZonesX = new List<Range> { new(dragZoneLeftIntent, windowWidth) };
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
                var yResult = (y - ySubtrahend).ToArray();
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

                        ++i;
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
                        ++i;
                        ++i;
                        break;
                }
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
    /// Set dragging-zones of <see cref="CurrentContext.Window"/> (title bar)
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites
    /// <list type="bullet">
    /// <item><term><see cref="Window"/></term></item>
    /// </list>
    /// </remarks>
    /// <param name="dragZoneHeight">1</param>
    /// <param name="dragZoneLeftIntent">2</param>
    /// <param name="nonDraggingZones">3</param>
    public static void SetDragZones(int dragZoneHeight = 48, int dragZoneLeftIntent = 0, IEnumerable<RectInt32>? nonDraggingZones = null)
    {
        var scaleAdjustment = WindowHelper.GetScaleAdjustment();
        CurrentContext.AppTitleBar.SetDragRectangles(
            GetDragZones(dragZoneHeight, dragZoneLeftIntent, nonDraggingZones ?? Array.Empty<RectInt32>())
                .Select(rect => new RectInt32(
                        (int)(rect.X * scaleAdjustment),
                        (int)(rect.Y * scaleAdjustment),
                        (int)(rect.Width * scaleAdjustment),
                        (int)(rect.Height * scaleAdjustment)))
                .ToArray());
    }

    /// <inheritdoc cref="SetDragZones(int, int, IEnumerable{RectInt32}?)"/>
    public static void SetDragZones(int dragZoneHeight, int dragZoneLeftIntent, params RectInt32[] nonDraggingZonesArray)
        => SetDragZones(dragZoneHeight, dragZoneLeftIntent, nonDraggingZones: nonDraggingZonesArray);

    /// <inheritdoc cref="SetDragZones(int, int, IEnumerable{RectInt32}?)"/>
    public static void SetDragZones(params RectInt32[] nonDraggingZonesSingle)
        => SetDragZones(nonDraggingZones: nonDraggingZonesSingle);

    /// <inheritdoc cref="SetDragZones(int, int, IEnumerable{RectInt32}?)"/>
    /// <param name="frameworkElements">The zones of <see cref="FrameworkElement"/>s that we need to exclude<br/>
    /// Implementation:<br/>
    /// <code>
    /// <paramref name="frameworkElements"/>.Select(t => <see langword="new"/> <see cref="RectInt32"/>(
    ///     (<see langword="int"/>) t.Margin.Left,
    ///     (<see langword="int"/>) t.Margin.Top,
    ///     (<see langword="int"/>) t.ActualWidth,
    ///     (<see langword="int"/>) t.ActualHeight)
    /// </code>
    /// </param>
#pragma warning disable CS1573
    public static void SetDragZones(int dragZoneHeight = 48, int dragZoneLeftIntent = 0, IEnumerable<RectInt32>? nonDraggingZones = null, params FrameworkElement[] frameworkElements)
#pragma warning restore CS1573
        => SetDragZones(dragZoneHeight, dragZoneLeftIntent, (nonDraggingZones ?? Array.Empty<RectInt32>())
            .Concat(frameworkElements
                .Select(t => new RectInt32(
                    (int)t.Margin.Left,
                    (int)t.Margin.Top,
                    (int)t.ActualWidth,
                    (int)t.ActualHeight))));

    /// <inheritdoc cref="SetDragZones(int, int, IEnumerable{RectInt32}?, FrameworkElement[])"/>
    /// <param name="frameworkElementsArray">The zones of <see cref="FrameworkElement"/>s that we need to exclude<br/>
    /// Implementation:<br/>
    /// <code>
    /// <paramref name="frameworkElementsArray"/>.Select(t => <see langword="new"/> <see cref="RectInt32"/>(
    ///     (<see langword="int"/>) t.Margin.Left,
    ///     (<see langword="int"/>) t.Margin.Top,
    ///     (<see langword="int"/>) t.ActualWidth,
    ///     (<see langword="int"/>) t.ActualHeight)
    /// </code>
    /// </param>
    public static void SetDragZones(params FrameworkElement[] frameworkElementsArray)
        => SetDragZones(frameworkElements: frameworkElementsArray);

    private record Range(int Lower, int Upper)
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
                yield return minuend with { Upper = subtrahend.Lower - 1 };
            if (minuend.Upper > subtrahend.Upper)
                yield return subtrahend with { Lower = minuend.Upper + 1 };
        }

        public static IEnumerable<Range> operator -(IEnumerable<Range> minuends, Range subtrahend)
            => minuends.SelectMany(minuend => minuend - subtrahend);
    }
}

