using System.Runtime.InteropServices;

namespace WinUI3Utilities.Internal.PlatformInvoke.CoreMessaging;

internal static class CoreMessaging
{
    [DllImport("CoreMessaging.dll")]
    internal static extern int CreateDispatcherQueueController(DispatcherQueueOptions options, [MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);
}
