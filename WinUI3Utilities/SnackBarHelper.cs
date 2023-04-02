using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for snack bar(non-targeted <see cref="TeachingTip"/>)
/// </summary>
public static class SnackBarHelper
{
    /// <summary>
    /// Root snack bar (generally placed on main window)
    /// </summary>
    public static TeachingTip RootSnackBar { get; set; } = null!;

    /// <remarks>
    /// Value type members require property to enable thread sharing
    /// </remarks>
    private static DateTime HideSnakeBarTime { get; set; }

    /// <summary>
    /// Assign <see cref="RootSnackBar"/> when <paramref name="sender"/> is loaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void TeachingTipLoaded(object sender, RoutedEventArgs e) => RootSnackBar = sender.To<TeachingTip>();

    /// <summary>
    /// Show <see cref="RootSnackBar"/>
    /// </summary>
    /// <param name="message"><see cref="TeachingTip.Title"/></param>
    /// <param name="severity"><see cref="TeachingTip.IconSource"/></param>
    /// <param name="hint"><see cref="TeachingTip.Subtitle"/></param>
    /// <param name="isLightDismissEnabled"><see cref="TeachingTip.IsLightDismissEnabled"/></param>
    public static void Show(string message, Severity severity = Severity.Ok, string hint = "", bool isLightDismissEnabled = false)
    {
        RootSnackBar.IsLightDismissEnabled = isLightDismissEnabled;
        RootSnackBar.Title = message;
        RootSnackBar.Subtitle = hint;
        RootSnackBar.IconSource = new FontIconSource
        {
            Glyph = severity switch
            {
                Severity.Ok => "\xE10B", // Accept
                Severity.Information => "\xE946", // Info
                Severity.Important => "\xE171", // Important
                Severity.Warning => "\xE7BA", // Warning
                Severity.Error => "\xEA39", // ErrorBadge
                _ => ThrowHelper.ArgumentOutOfRange<Severity, string>(severity)
            }
        };

        RootSnackBar.IsOpen = true;
    }

    /// <summary>
    /// Show <see cref="RootSnackBar"/> and hide after <paramref name="mSec"/> microseconds
    /// </summary>
    /// <param name="message"><see cref="TeachingTip.Title"/></param>
    /// <param name="severity"><see cref="TeachingTip.IconSource"/></param>
    /// <param name="hint"><see cref="TeachingTip.Subtitle"/></param>
    /// <param name="mSec">Automatically hide after <paramref name="mSec"/> milliseconds</param>
    /// <param name="isLightDismissEnabled"><see cref="TeachingTip.IsLightDismissEnabled"/></param>
    public static async void ShowAndHide(string message, Severity severity = Severity.Ok, string hint = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        HideSnakeBarTime = DateTime.Now + TimeSpan.FromMilliseconds(mSec - 100);

        Show(message, severity, hint, isLightDismissEnabled);

        await Task.Delay(mSec);

        if (DateTime.Now > HideSnakeBarTime)
            RootSnackBar.IsOpen = false;
    }

    /// <summary>
    /// Snack bar severity on <see cref="TeachingTip.IconSource"/> (Segoe Fluent Icons font)
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Accept (E10B)
        /// </summary>
        Ok,
        /// <summary>
        /// Info (E946)
        /// </summary>
        Information,
        /// <summary>
        /// Important (E171)
        /// </summary>
        Important,
        /// <summary>
        /// Warning (E7BA)
        /// </summary>
        Warning,
        /// <summary>
        /// ErrorBadge (EA39)
        /// </summary>
        Error
    }
}

