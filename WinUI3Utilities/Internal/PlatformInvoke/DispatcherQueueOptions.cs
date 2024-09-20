using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WinUI3Utilities.Internal.PlatformInvoke;

[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("Style", "IDE1006:命名样式")]
internal ref struct DispatcherQueueOptions
{
    internal int dwSize;
    internal int threadType;
    internal int apartmentType;
}
