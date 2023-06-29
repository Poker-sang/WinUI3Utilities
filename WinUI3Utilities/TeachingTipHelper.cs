using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// A set of methods for <see cref="TeachingTip"/>
/// </summary>
public static class TeachingTipHelper
{
    /// <summary>
    /// Root snack bar (generally placed on main window)
    /// </summary>
    public static TeachingTip RootTeachingTip { get; set; } = null!;

    /// <remarks>
    /// Value type members require property to enable thread sharing
    /// </remarks>
    private static DateTime HideSnakeBarTime { get; set; }

    /// <summary>
    /// Assign <see cref="RootTeachingTip"/> when <paramref name="sender"/> is loaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void TeachingTipLoaded(object sender, RoutedEventArgs e) => RootTeachingTip = sender.To<TeachingTip>();

    /// <inheritdoc cref="Show(string, Severity, string, bool)"/>
    public static void Show(string title, IconSource icon, string hint = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon, hint, isLightDismissEnabled);
    }

    /// <inheritdoc cref="Show(TeachingTip, string, Severity, string, bool)"/>
    public static void Show(this TeachingTip teachingTip, string title, IconSource? icon, string subtitle = "", bool isLightDismissEnabled = false)
    {
        teachingTip.IsLightDismissEnabled = isLightDismissEnabled;
        teachingTip.Title = title;
        teachingTip.Subtitle = subtitle;
        teachingTip.IconSource = icon;
        teachingTip.IsOpen = true;
    }

    /// <summary>
    /// Show <see cref="RootTeachingTip"/>
    /// </summary>
    /// <inheritdoc cref="Show(TeachingTip, string, Severity, string, bool)"/>
    public static void Show(string title, Severity icon = Severity.Ok, string subtitle = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon, subtitle, isLightDismissEnabled);
    }

    /// <summary>
    /// Show <paramref name="teachingTip"/>
    /// </summary>
    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, Severity, string, int, bool)"/>
    public static void Show(this TeachingTip teachingTip, string title, Severity icon = Severity.Ok, string subtitle = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon.GetIconSource(), subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, IconSource?, string, int, bool)"/>
    public static void ShowAndHide(string title, IconSource? icon, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        RootTeachingTip.ShowAndHide(title, icon, subtitle, mSec, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, Severity, string, int, bool)"/>
    public static async void ShowAndHide(this TeachingTip teachingTip, string title, IconSource? icon, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        HideSnakeBarTime = DateTime.Now + TimeSpan.FromMilliseconds(mSec - 100);

        teachingTip.Show(title, icon, subtitle, isLightDismissEnabled);

        await Task.Delay(mSec);

        if (DateTime.Now > HideSnakeBarTime)
            teachingTip.IsOpen = false;
    }

    /// <summary>
    /// Show <see cref="RootTeachingTip"/> and hide after <paramref name="mSec"/> microseconds
    /// </summary>
    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, Severity, string, int, bool)"/>
    public static void ShowAndHide(string title, Severity icon = Severity.Ok, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        RootTeachingTip.ShowAndHide(title, icon, subtitle, mSec, isLightDismissEnabled);
    }

    /// <summary>
    /// Show <paramref name="teachingTip"/> and hide after <paramref name="mSec"/> microseconds
    /// </summary>
    /// <param name="teachingTip"></param>
    /// <param name="title"><see cref="TeachingTip.Title"/></param>
    /// <param name="icon"><see cref="TeachingTip.IconSource"/></param>
    /// <param name="subtitle"><see cref="TeachingTip.Subtitle"/></param>
    /// <param name="mSec">Automatically hide after <paramref name="mSec"/> milliseconds</param>
    /// <param name="isLightDismissEnabled"><see cref="TeachingTip.IsLightDismissEnabled"/></param>
    public static void ShowAndHide(this TeachingTip teachingTip, string title, Severity icon = Severity.Ok, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        teachingTip.ShowAndHide(title, icon.GetIconSource(), subtitle, mSec, isLightDismissEnabled);
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
        Error,
        /// <summary>
        /// <see langword="null"/>
        /// </summary>
        None
    }

    private static IconSource? GetIconSource(this Severity severity)
    {
        return severity is Severity.None
            ? null
            : new FontIconSource
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
    }
}
