using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics;
using WinUI3Utilities.Internal.PlatformInvoke.User32;

namespace WinUI3Utilities;

/// <summary>
/// Set drag-move function to <see cref="RootPanel"/>
/// </summary>
public static class DragMoveHelper
{
    /// <summary>
    /// Once set, 3 events that are subscribed to by <see cref="RootPanel"/> (call <see cref="SetDragMove"/>)<br/>
    /// When set to a new <see cref="UIElement"/> or <see langword="null"/>, the old <see cref="RootPanel"/>'s events will be unsubscribed (call <see cref="UnsetDragMove"/>)
    /// </summary>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// <item><term><see cref="AppWindowPresenter.Kind"/></term><description>value is <see cref="AppWindowPresenterKind.Overlapped"/></description></item>
    /// </list>
    /// </remarks>
    public static UIElement? RootPanel
    {
        get => _rootPanel;
        set
        {
            if (_rootPanel is not null)
                UnsetDragMove(_rootPanel);
            _rootPanel = value;
            if (_rootPanel is not null)
                SetDragMove(_rootPanel);
        }
    }

    /// <summary>
    /// Subscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="element"></param>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// <item><term><see cref="AppWindowPresenter.Kind"/></term><description>value is <see cref="AppWindowPresenterKind.Overlapped"/></description></item>
    /// </list>
    /// </remarks>
    public static void SetDragMove(UIElement element)
    {
        element.PointerPressed += RootPointerPressed;
        element.PointerMoved += RootPointerMoved;
        element.PointerReleased += RootPointerReleased;
    }

    /// <summary>
    /// Unsubscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="element"></param>
    public static void UnsetDragMove(UIElement element)
    {
        element.PointerPressed -= RootPointerPressed;
        element.PointerMoved -= RootPointerMoved;
        element.PointerReleased -= RootPointerReleased;
    }

    /// <summary>
    /// The L2 norm of minimum mouse offset
    /// </summary>
    /// <remarks>default: 5</remarks>
    public static int Offset { get; set; } = 5;

    private static PointInt32 _mousePoint;
    private static PointInt32 _windowPosition;
    private static bool _moving;
    private static UIElement? _rootPanel;

    private static void RootPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var element = sender.To<UIElement>();
        var properties = e.GetCurrentPoint(element).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _ = element.CapturePointer(e.Pointer);
            _windowPosition = CurrentContext.AppWindow.Position;
            _ = User32.GetCursorPos(out _mousePoint);
            _moving = true;
        }
    }

    private static void RootPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(sender.To<UIElement>()).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _ = User32.GetCursorPos(out var pt);
            if (_moving && Square(pt.X - _mousePoint.X) + Square(pt.Y - _mousePoint.Y) > Offset)
            {
                if (CurrentContext.OverlappedPresenter.State is OverlappedPresenterState.Maximized)
                {
                    var originalSize = CurrentContext.AppWindow.Size;
                    CurrentContext.OverlappedPresenter.Restore();
                    var size = CurrentContext.AppWindow.Size;
                    var rate = 1 - (double)size.Width / originalSize.Width;
                    _windowPosition = new((int)(pt.X * rate), (int)(pt.Y * rate));
                }
                CurrentContext.AppWindow.Move(new(_windowPosition.X + (pt.X - _mousePoint.X),
                    _windowPosition.Y + (pt.Y - _mousePoint.Y)));
            }
        }
    }

    private static void RootPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        sender.To<UIElement>().ReleasePointerCaptures();
        _moving = false;
    }

    private static int Square(int x) => x * x;
}
