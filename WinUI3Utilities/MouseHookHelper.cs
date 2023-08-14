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
    private static MouseUpEventHandler? MouseUp;
    private static MouseUpEventHandler? MouseDown;
    private static MouseUpEventHandler? MouseMove;
    private static bool _isSubscribed;

    public static event MouseUpEventHandler? OnMouseUp
    {
        add
        {
            if (!_isSubscribed)
                Subscribe();
            MouseUp += value;
        }
        remove
        {
            MouseUp -= value;
            if (_isSubscribed)
                Unsubscribe();
        }
    }

    public static event MouseUpEventHandler? OnMouseDown
    {
        add
        {
            if (!_isSubscribed)
                Subscribe();
            MouseDown += value;
        }
        remove
        {
            MouseDown -= value;
            if (_isSubscribed)
                Unsubscribe();
        }
    }

    public static event MouseUpEventHandler? OnMouseMove
    {
        add
        {
            if (!_isSubscribed)
                Subscribe();
            MouseMove += value;
        }
        remove
        {
            MouseMove -= value;
            if (_isSubscribed)
                Unsubscribe();
        }
    }

    private static void Unsubscribe()
    {
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

    private static void Subscribe()
    {
        if (_mouseHookHandle is 0)
        {
            _mouseDelegate = MouseHookProc;
            _mouseHookHandle = User32.SetWindowsHookEx(WH_MOUSE_LL,
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

    private static nint MouseHookProc(int nCode, nint wParam, MSLLHOOKSTRUCT lParam)
    {
        if (nCode >= 0)
        {
            switch (wParam)
            {
                case WM_LBUTTONUP:
                    MouseUp?.Invoke(null, lParam.pt);
                    break;
                case WM_LBUTTONDOWN:
                    MouseDown?.Invoke(null, lParam.pt);
                    break;
                case WM_MOUSEMOVE:
                    MouseMove?.Invoke(null, lParam.pt);
                    break;
            }
        }
        return User32.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
    }

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_MOUSEMOVE = 0x0200;



}

public delegate void MouseUpEventHandler(object? sender, PointInt32 p);
