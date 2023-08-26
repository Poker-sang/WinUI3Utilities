using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinUI3Utilities.Internal.PlatformInvoke;

namespace WinUI3Utilities;

public class MouseHookHelper
{
    private static int _mouseHookHandle;
    private static User32.HookProc? _mouseDelegate;
    private static MouseEventHandler? _mouseUp;
    private static MouseEventHandler? _mouseDown;
    private static MouseEventHandler? _mouseMove;
    private static int _isSubscribed;

    public static event MouseEventHandler? OnMouseUp
    {
        add
        {
            TrySubscribe();
            _mouseUp += value;
        }
        remove
        {
            _mouseUp -= value;
            TryUnsubscribe();
        }
    }

    public static event MouseEventHandler? OnMouseDown
    {
        add
        {
            TrySubscribe();
            _mouseDown += value;
        }
        remove
        {
            _mouseDown -= value;
            TryUnsubscribe();
        }
    }

    public static event MouseEventHandler? OnMouseMove
    {
        add
        {
            TrySubscribe();
            _mouseMove += value;
        }
        remove
        {
            _mouseMove -= value;
            TryUnsubscribe();
        }
    }

    private static void TryUnsubscribe()
    {
        --_isSubscribed;
        if (_isSubscribed is not 0)
            return;
        if (_mouseHookHandle != 0)
        {
            var result = User32.UnhookWindowsHookEx(_mouseHookHandle);
            _mouseHookHandle = 0;
            _mouseDelegate = null;
            if (result is 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }
    }

    private static void TrySubscribe()
    {
        ++_isSubscribed;
        if (_isSubscribed is not 1)
            return;
        if (_mouseHookHandle is 0)
        {
            _mouseDelegate = MouseHookProc;
            _mouseHookHandle = User32.SetWindowsHookEx(
                (int)NativeConstant.WH_MOUSE_LL,
                _mouseDelegate,
                Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule!.ModuleName),
                0);
            if (_mouseHookHandle is 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }
    }

    private static nint MouseHookProc(NativeConstant nCode, WindowMessage wParam, MSLLHOOKSTRUCT lParam)
    {
        if (nCode >= 0)
        {
            switch (wParam)
            {
                case WindowMessage.WM_LBUTTONUP:
                    _mouseUp?.Invoke(null, lParam.pt);
                    break;
                case WindowMessage.WM_LBUTTONDOWN:
                    _mouseDown?.Invoke(null, lParam.pt);
                    break;
                case WindowMessage.WM_MOUSEMOVE:
                    _mouseMove?.Invoke(null, lParam.pt);
                    break;
                default:
                    break;
            }
        }
        return User32.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
    }
}

public delegate void MouseEventHandler(object? sender, PointInt32 p);
