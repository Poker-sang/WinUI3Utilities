using WinUI3Utilities.Internal.PlatformInvoke.CoreMessaging;

namespace WinUI3Utilities.Internal;

internal class WindowsSystemDispatcherQueueHelper
{
    private object? _dispatcherQueueController;

    public unsafe void EnsureWindowsSystemDispatcherQueueController()
    {
        // one already exists, so we'll just use it.
        if (Windows.System.DispatcherQueue.GetForCurrentThread() is not null)
            return;

        if (_dispatcherQueueController is null)
        {
            var options = new DispatcherQueueOptions
            {
                DwSize = sizeof(DispatcherQueueOptions),
                ThreadType = 2,
                ApartmentType = 2
            };

            _ = CoreMessaging.CreateDispatcherQueueController(options, ref _dispatcherQueueController!);
        }
    }
}
