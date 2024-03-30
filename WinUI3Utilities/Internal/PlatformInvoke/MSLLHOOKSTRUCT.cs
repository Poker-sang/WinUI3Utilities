using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3Utilities.Internal.PlatformInvoke;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
[StructLayout(LayoutKind.Sequential)]
internal struct MSLLHOOKSTRUCT
{
    public PointInt32 pt;
    public uint mouseData;
    public uint flags;
    public uint time;
    public nint dwExtraInfo;
}
