using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3Utilities.Internal.PlatformInvoke;

internal static partial class User32
{
    [LibraryImport(nameof(User32))]
    internal static partial int GetSystemMetrics(SystemMetric systemMetric);

    [LibraryImport(nameof(User32))]
    internal static partial int GetDpiForWindow(nint hWnd);

    [LibraryImport(nameof(User32))]
    internal static partial nint SendMessage(nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    [LibraryImport(nameof(User32), SetLastError = true)]
    internal static partial nint GetActiveWindow();

#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
    [DllImport(nameof(User32))]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out PointInt32 point);

    [DllImport(nameof(User32))]
    internal static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    [DllImport(nameof(User32), EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport(nameof(User32), EntryPoint = "SetWindowLongPtr")]
    private static extern nint SetWindowLongPtr64(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码

    internal delegate nint WinProc(nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    internal static nint SetWindowLongPtr(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
    {
        return nint.Size is 8 ? SetWindowLongPtr64(hWnd, nIndex, newProc) : SetWindowLong32(hWnd, nIndex, newProc);
    }
}
