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

    /// <inheritdoc cref="Show(string, TeachingTipSeverity, string, bool)"/>
    public static void Show(string title, IconSource icon, string hint = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon, hint, isLightDismissEnabled);
    }

    /// <inheritdoc cref="Show(TeachingTip, string, TeachingTipSeverity, string, bool)"/>
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
    /// <inheritdoc cref="Show(TeachingTip, string, TeachingTipSeverity, string, bool)"/>
    public static void Show(string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon, subtitle, isLightDismissEnabled);
    }

    /// <summary>
    /// Show <paramref name="teachingTip"/>
    /// </summary>
    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void Show(this TeachingTip teachingTip, string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", bool isLightDismissEnabled = false)
    {
        RootTeachingTip.Show(title, icon.GetIconSource(), subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, IconSource?, string, int, bool)"/>
    public static void ShowAndHide(string title, IconSource? icon, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        RootTeachingTip.ShowAndHide(title, icon, subtitle, mSec, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
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
    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void ShowAndHide(string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
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
    public static void ShowAndHide(this TeachingTip teachingTip, string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        teachingTip.ShowAndHide(title, icon.GetIconSource(), subtitle, mSec, isLightDismissEnabled);
    }

    private static IconSource? GetIconSource(this TeachingTipSeverity severity)
    {
        return severity is TeachingTipSeverity.None
            ? null
            : new FontIconSource
            {
                Glyph = severity switch
                {
                    TeachingTipSeverity.Ok => "\xE10B", // Accept
                    TeachingTipSeverity.Information => "\xE946", // Info
                    TeachingTipSeverity.Important => "\xE171", // Important
                    TeachingTipSeverity.Warning => "\xE7BA", // Warning
                    TeachingTipSeverity.Error => "\xEA39", // ErrorBadge
                    _ => ThrowHelper.ArgumentOutOfRange<TeachingTipSeverity, string>(severity)
                }
            };
    }
}
