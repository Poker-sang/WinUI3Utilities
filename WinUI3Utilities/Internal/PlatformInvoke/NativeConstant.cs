using System.Diagnostics.CodeAnalysis;
using System;
#pragma warning disable CA1069

namespace WinUI3Utilities.Internal.PlatformInvoke;

#pragma warning disable IDE0079 // 请删除不必要的忽略
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
#pragma warning restore IDE0079 // 请删除不必要的忽略
[Flags]
internal enum NativeConstant
{
    WH_MOUSE_LL = 14,

    WA_ACTIVE = 0x01,
    WA_CLICKACTIVE = 0x02,
    WA_INACTIVE = 0x00,

    SC_CLOSE = 0xF060,
    SC_CONTEXTHELP = 0xF180,
    SC_DEFAULT = 0xF160,
    SC_HOTKEY = 0xF150,
    SC_HSCROLL = 0xF080,
    SCF_ISSECURE = 0x00000001,
    SC_KEYMENU = 0xF100,
    SC_MAXIMIZE = 0xF030,
    SC_MINIMIZE = 0xF020,
    SC_MONITORPOWER = 0xF170,
    SC_MOUSEMENU = 0xF090,
    SC_MOVE = 0xF010,
    SC_NEXTWINDOW = 0xF040,
    SC_PREVWINDOW = 0xF050,
    SC_RESTORE = 0xF120,
    SC_SCREENSAVE = 0xF140,
    SC_SIZE = 0xF000,
    SC_TASKLIST = 0xF130,
    SC_VSCROLL = 0xF070,

    WMSZ_BOTTOM = 6,
    WMSZ_BOTTOMLEFT = 7,
    WMSZ_BOTTOMRIGHT = 8,
    WMSZ_LEFT = 1,
    WMSZ_RIGHT = 2,
    WMSZ_TOP = 3,
    WMSZ_TOPLEFT = 4,
    WMSZ_TOPRIGHT = 5,

    HTBORDER = 18,
    HTBOTTOM = 15,
    HTBOTTOMLEFT = 16,
    HTBOTTOMRIGHT = 17,
    HTCAPTION = 2,
    HTCLIENT = 1,
    HTCLOSE = 20,
    HTERROR = -2,
    HTGROWBOX = 4,
    HTHELP = 21,
    HTHSCROLL = 6,
    HTLEFT = 10,
    HTMENU = 5,
    HTMAXBUTTON = 9,
    HTMINBUTTON = 8,
    HTNOWHERE = 0,
    HTREDUCE = 8,
    HTRIGHT = 11,
    HTSIZE = 4,
    HTSYSMENU = 3,
    HTTOP = 12,
    HTTOPLEFT = 13,
    HTTOPRIGHT = 14,
    HTTRANSPARENT = -1,
    HTVSCROLL = 7,
    HTZOOM = 9,

    HC_ACTION = 0,
    HC_NOREMOVE = 3
}
