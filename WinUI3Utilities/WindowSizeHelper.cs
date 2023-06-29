using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinUI3Utilities.Attributes;
using WinUI3Utilities.Internal.PlatformInvoke;

namespace WinUI3Utilities;

/// <summary>
/// Helper to limit the size of the window, use <see cref="WindowSizeHelperAttribute"/>
/// </summary>
public static class WindowSizeHelper
{
    /// <summary>
    /// DO NOT MODIFY OR RELEASE<br/>
    /// Generated for <see cref="WindowSizeHelper"/>
    /// </summary>
    public class WindowSizeParameter
    {
        #region System.ExecutionEngineException would throw if delete the following field

        internal User32.WinProc NewWndProc = null!;

        internal nint OldWndProc;

        #endregion

        /// <summary>
        /// </summary>
        public int MinWidth;

        /// <summary>
        /// </summary>
        public int MinHeight;

        /// <summary>
        /// </summary>
        public int MaxWidth;

        /// <summary>
        /// </summary>
        public int MaxHeight;
    }

    /// <summary>
    /// </summary>
    public static void Register(Window window, WindowSizeParameter parameter)
    {
        parameter.NewWndProc = (wnd, msg, wParam, lParam) =>
            WndProc(parameter.MinWidth, parameter.MinHeight, parameter.MaxWidth, parameter.MaxHeight,
                parameter.OldWndProc, wnd, msg, wParam, lParam);
        parameter.OldWndProc = User32.SetWindowLongPtr((nint)window.AppWindow.Id.Value, WindowLongIndexFlags.GWL_WNDPROC, parameter.NewWndProc);
    }

    /// <summary>
    /// </summary>
    private static nint WndProc(int minWidth, int minHeight, int maxWidth, int maxHeight, nint oldWndProc, nint hWnd, WindowMessage msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case WindowMessage.WM_GETMINMAXINFO:
                var scalingFactor = WindowHelper.GetScaleAdjustment(hWnd);

                var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                if (minWidth is not 0)
                    minMaxInfo.ptMinTrackSize.X = (int)(minWidth * scalingFactor);
                if (maxWidth is not 0)
                    minMaxInfo.ptMaxTrackSize.X = (int)(maxWidth * scalingFactor);
                if (minHeight is not 0)
                    minMaxInfo.ptMinTrackSize.Y = (int)(minHeight * scalingFactor);
                if (maxHeight is not 0)
                    minMaxInfo.ptMaxTrackSize.Y = (int)(maxHeight * scalingFactor);
                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return User32.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
    }
}
