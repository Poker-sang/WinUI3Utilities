using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3Utilities.Internal.PlatformInvoke.User32;

internal static partial class User32
{
    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(SystemMetric systemMetric);

#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out PointInt32 point);
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
}
