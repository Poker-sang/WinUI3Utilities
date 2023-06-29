using System.Runtime.InteropServices;
using Windows.Graphics;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
#pragma warning disable IDE1006

namespace WinUI3Utilities.Internal.PlatformInvoke;

[StructLayout(LayoutKind.Sequential)]
internal struct MINMAXINFO
{
    public PointInt32 ptReserved;
    public PointInt32 ptMaxSize;
    public PointInt32 ptMaxPosition;
    public PointInt32 ptMinTrackSize;
    public PointInt32 ptMaxTrackSize;
}
