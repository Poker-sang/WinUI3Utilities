using System.Runtime.InteropServices;

namespace WinUI3Utilities.PlatformInvoke.CoreMessaging;

[StructLayout(LayoutKind.Sequential)]
internal struct DispatcherQueueOptions
{
    internal int DwSize;
    internal int ThreadType;
    internal int ApartmentType;
}
