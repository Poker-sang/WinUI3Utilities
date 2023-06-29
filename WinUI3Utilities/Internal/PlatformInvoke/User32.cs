using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3Utilities.Internal.PlatformInvoke;

internal static partial class User32
{
    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(SystemMetric systemMetric);

    [LibraryImport("user32.dll")]
    internal static partial int GetDpiForWindow(nint hWnd);

#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out PointInt32 point);

    [DllImport("user32.dll")]
    internal static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern nint SetWindowLongPtr64(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码

    internal delegate nint WinProc(nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    internal static nint SetWindowLongPtr(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
    {
        return nint.Size is 8 ? SetWindowLongPtr64(hWnd, nIndex, newProc) : SetWindowLong32(hWnd, nIndex, newProc);
    }
}
