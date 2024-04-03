using System.Runtime.InteropServices;

namespace WinUI3Utilities.Internal.PlatformInvoke;

internal static partial class CoreMessaging
{
    [LibraryImport("CoreMessaging.dll")]
    internal static partial int CreateDispatcherQueueController(in DispatcherQueueOptions options, out nint dispatcherQueueController);
}
