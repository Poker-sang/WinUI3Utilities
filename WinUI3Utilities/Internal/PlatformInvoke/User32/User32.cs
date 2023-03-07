using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3Utilities.Internal.PlatformInvoke.User32;

internal static partial class User32
{
    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(SystemMetric systemMetric);


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out PointInt32 point);
}
