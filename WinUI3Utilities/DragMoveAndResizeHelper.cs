using System;
using System.Numerics;
using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics;
using WinUI3Utilities.Interfaces;
using WinUI3Utilities.Internal.PlatformInvoke.User32;

namespace WinUI3Utilities;

/// <summary>
/// Apply drag-move and resize function to <see cref="RootPanel"/>
/// </summary>
public static class DragMoveAndResizeHelper
{
    /// <summary>
    /// Info type for <see cref="DragMoveAndResizeHelper"/>
    /// </summary>
    /// <remarks>
    /// The default value of <see cref="AppWindow"/> is <see cref="CurrentContext.AppWindow"/>
    /// </remarks>
    public record DragMoveAndResizeInfo(Mode Mode)
    {
        /// <summary>
        /// Default: 10
        /// </summary>
        public int MinimumOffset { get; set; } = 10;

        /// <summary>
        /// Default: 10
        /// </summary>
        public int DraggableBorderThickness { get; set; } = 10;

        /// <summary>
        /// Default: (1280, 720)
        /// </summary>
        public (int X, int Y) MinimumSize { get; set; } = (1280, 720);

        /// <summary>
        /// Default: <see cref="WindowHelper.GetScreenSize"/>
        /// </summary>
        public (int X, int Y) MaximumSize { get; set; } = WindowHelper.GetScreenSize();
    }

    /// <summary>
    /// Mode for <inheritdoc cref="DragMoveAndResizeInfo"/>
    /// </summary>
    [Flags]
    public enum Mode
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

    /// <summary>
    /// Once set, 3 events that are subscribed to by <see cref="RootPanel"/> (call <see cref="SetDragMove"/>, and use <see cref="Mode.Both"/>)<br/>
    /// When set to a new <see cref="UIElement"/> or <see langword="null"/>, the old <see cref="RootPanel"/>'s events will be unsubscribed (call <see cref="UnsetDragMove"/>)
    /// </summary>
    public static UIElement? RootPanel
    {
        get => _rootPanel;
        set
        {
            if (_rootPanel is not null)
                UnsetDragMove(_rootPanel);
            _rootPanel = value;
            if (_rootPanel is not null)
                SetDragMove(_rootPanel, new(Mode.Both));
        }
    }

    /// <summary>
    /// Subscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="info"></param>
    /// <param name="windowInfo">Default: <see cref="CurrentContext.WindowInfo"/></param>
    public static void SetDragMove(UIElement element, DragMoveAndResizeInfo info, IWindowInfo? windowInfo = null)
    {
        _pointerPressed = (o, e) => RootPointerPressed(o, e, info);
        _pointerMoved = (o, e) => RootPointerMoved(o, e, info, windowInfo ?? CurrentContext.WindowInfo);
        element.PointerPressed += _pointerPressed;
        element.PointerMoved += _pointerMoved;
        element.PointerReleased += RootPointerReleased;
    }

    /// <summary>
    /// Unsubscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="element"></param>
    public static void UnsetDragMove(UIElement element)
    {
        element.PointerPressed -= _pointerPressed;
        element.PointerMoved -= _pointerMoved;
        element.PointerReleased -= RootPointerReleased;
    }

    private static UIElement? _rootPanel;
    private static PointerEventHandler _pointerPressed = null!;
    private static PointerEventHandler _pointerMoved = null!;

    private static PointInt32 _lastPoint;
    private static PointerOperationType _type;
    private static InputSystemCursorShape _lastShape = InputSystemCursorShape.Arrow;

    private static readonly PropertyInfo _property = typeof(UIElement).GetProperty("ProtectedCursor", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.SetProperty)!;

    [Flags]
    private enum PointerOperationType
    {
        /// <summary>
        /// Used to distinguish whether a button is pressed in the root control
        /// </summary>
        None = 0,

        /// <summary>
        /// Only available when <see cref="Mode.Resize"/> is set
        /// </summary>
        Top = 1 << 0,

        /// <inheritdoc cref="Top"/>
        Bottom = 1 << 1,

        /// <inheritdoc cref="Top"/>
        Left = 1 << 2,

        /// <inheritdoc cref="Top"/>
        Right = 1 << 3,

        /// <inheritdoc cref="Top"/>
        LeftTop = Top | Left,

        /// <inheritdoc cref="Top"/>
        RightTop = Right | Top,

        /// <inheritdoc cref="Top"/>
        LeftBottom = Left | Bottom,

        /// <inheritdoc cref="Top"/>
        RightBottom = Right | Bottom,

        /// <summary>
        /// Only available when <see cref="Mode.DragMove"/> is set
        /// </summary>
        Move = Top | Left | Right | Bottom
    }

    private static void RootPointerPressed(object sender, PointerRoutedEventArgs e, DragMoveAndResizeInfo info)
    {
        var frameworkElement = sender.To<FrameworkElement>();
        var point = e.GetCurrentPoint(frameworkElement);
        var properties = point.Properties;

        if (!properties.IsLeftButtonPressed)
            return;

        var width = frameworkElement.ActualWidth;
        var height = frameworkElement.ActualHeight;
        var position = point.Position;

        if (info.Mode is Mode.DragMove)
            _type = PointerOperationType.Move;
        else
        {
            _type = PointerOperationType.None;
            if (position.X < info.DraggableBorderThickness)
                _type |= PointerOperationType.Left;
            if (position.Y < info.DraggableBorderThickness)
                _type |= PointerOperationType.Top;
            if (width - position.X < info.DraggableBorderThickness)
                _type |= PointerOperationType.Right;
            if (height - position.Y < info.DraggableBorderThickness)
                _type |= PointerOperationType.Bottom;
            if (_type is PointerOperationType.None && info.Mode is not Mode.Resize)
                _type = PointerOperationType.Move;
        }

        _ = frameworkElement.CapturePointer(e.Pointer);
        _ = User32.GetCursorPos(out _lastPoint);
    }

    private static void RootPointerMoved(object sender, PointerRoutedEventArgs e, DragMoveAndResizeInfo info, IWindowInfo windowInfo)
    {
        var frameworkElement = sender.To<FrameworkElement>();
        var pointer = e.GetCurrentPoint(frameworkElement);

        #region Cursor shape

        var position = pointer.Position;
        var width = frameworkElement.ActualWidth;
        var height = frameworkElement.ActualHeight;

        var pointerShape = InputSystemCursorShape.Arrow;

        if (windowInfo.OverlappedPresenter.State is not OverlappedPresenterState.Maximized && info.Mode.HasFlag(Mode.Resize))
        {
            var left = position._x < info.MinimumOffset;
            var top = position._y < info.MinimumOffset;
            var right = width - position._x < info.MinimumOffset;
            var bottom = height - position._y < info.MinimumOffset;
            pointerShape = 0 switch
            {
                0 when (left && top) || (right && bottom) => InputSystemCursorShape.SizeNorthwestSoutheast,
                0 when (left && bottom) || (right && top) => InputSystemCursorShape.SizeNortheastSouthwest,
                0 when left || right => InputSystemCursorShape.SizeWestEast,
                0 when top || bottom => InputSystemCursorShape.SizeNorthSouth,
                _ => pointerShape
            };
        }

        if (pointerShape != _lastShape)
        {
            _property.GetValue(sender).To<InputCursor?>()?.Dispose();
            _property.SetValue(sender, InputSystemCursor.Create(pointerShape));
            _lastShape = pointerShape;
        }

        #endregion

        var properties = pointer.Properties;
        if (!properties.IsLeftButtonPressed || _type is PointerOperationType.None)
            return;

        _ = User32.GetCursorPos(out var point);

        var xOffset = point.X - _lastPoint.X;
        var yOffset = point.Y - _lastPoint.Y;
        var offset = Vector2.DistanceSquared(Vector2.Zero, new(xOffset, yOffset));

        if (offset < info.MinimumOffset)
            return;

        if (windowInfo.OverlappedPresenter.State is OverlappedPresenterState.Maximized)
        {
            if (_type is PointerOperationType.Move)
            {
                var originalSize = windowInfo.AppWindow.Size;
                windowInfo.OverlappedPresenter.Restore();
                var size = windowInfo.AppWindow.Size;
                var rate = 1 - (double)size.Width / originalSize.Width;
                windowInfo.AppWindow.Move(new((int)(point.X * rate), (int)(point.Y * rate)));
            }
        }
        else
        {
            var xPos = windowInfo.AppWindow.Position.X;
            var yPos = windowInfo.AppWindow.Position.Y;
            var xSize = windowInfo.AppWindow.Size.Width;
            var ySize = windowInfo.AppWindow.Size.Height;

            if (_type.HasFlag(PointerOperationType.Top))
            {
                yPos += yOffset;
                var newYSize = ySize - yOffset;
                if (info.MinimumSize.Y < newYSize && newYSize < info.MaximumSize.Y)
                    ySize = newYSize;
            }
            if (_type.HasFlag(PointerOperationType.Bottom))
            {
                var newYSize = ySize + yOffset;
                if (info.MinimumSize.Y < newYSize && newYSize < info.MaximumSize.Y)
                    ySize = newYSize;
                else
                    yPos += yOffset;
            }
            if (_type.HasFlag(PointerOperationType.Left))
            {
                xPos += xOffset;
                var newXSize = xSize - xOffset;
                if (info.MinimumSize.X < newXSize && newXSize < info.MaximumSize.X)
                    xSize = newXSize;
            }
            if (_type.HasFlag(PointerOperationType.Right))
            {
                var newXSize = xSize + xOffset;
                if (info.MinimumSize.X < newXSize && newXSize < info.MaximumSize.X)
                    xSize = newXSize;
                else
                    xPos += xOffset;
            }

            windowInfo.AppWindow.MoveAndResize(new(xPos, yPos, xSize, ySize));
        }

        _lastPoint.X = point.X;
        _lastPoint.Y = point.Y;
    }

    private static void RootPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        sender.To<UIElement>().ReleasePointerCaptures();
        _type = PointerOperationType.None;
    }
}
