using System.Runtime.InteropServices;

namespace WinUI3Utilities.PlatformInvoke.Shcore;

internal partial class Shcore
{
    [LibraryImport("Shcore.dll", SetLastError = true)]
    internal static partial int GetDpiForMonitor(nint hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
}
