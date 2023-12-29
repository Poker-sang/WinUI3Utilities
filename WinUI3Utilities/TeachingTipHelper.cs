using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Helper for <see cref="TeachingTip"/>
/// </summary>
public static class TeachingTipHelper
{
    /// <summary>
    /// Create a <see cref="TeachingTip"/> with <paramref name="xamlRoot"/> and <paramref name="target"/>
    /// </summary>
    /// <param name="xamlRoot"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static TeachingTip CreateTeachingTip(this UIElement xamlRoot, FrameworkElement? target = null)
    {
        return new()
        {
            XamlRoot = xamlRoot.XamlRoot,
            Target = target,
            IsOpen = false
        };
    }

    /// <inheritdoc cref="Show(TeachingTip, string, IconSource?, string, bool)"/>
    public static TeachingTip ShowTeachingTip(this UIElement xamlRoot, string title, IconSource? icon, string subtitle = "", bool isLightDismissEnabled = false, FrameworkElement? target = null)
    {
        var teachingTip = xamlRoot.CreateTeachingTip(target);
        teachingTip.Show(title, icon, subtitle, isLightDismissEnabled);
        return teachingTip;
    }

    /// <inheritdoc cref="Show(TeachingTip, string, TeachingTipSeverity, string, bool)"/>
    public static TeachingTip ShowTeachingTip(this UIElement xamlRoot, string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", bool isLightDismissEnabled = false, FrameworkElement? target = null)
    {
        var teachingTip = xamlRoot.CreateTeachingTip(target);
        teachingTip.Show(title, icon, subtitle, isLightDismissEnabled);
        return teachingTip;
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, IconSource?, string, int, bool)"/>
    public static TeachingTip ShowTeachingTipAndHide(this UIElement xamlRoot, string title, IconSource? icon, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true, FrameworkElement? target = null)
    {
        var teachingTip = xamlRoot.CreateTeachingTip(target);
        teachingTip.ShowAndHide(title, icon, subtitle, mSec, isLightDismissEnabled);
        return teachingTip;
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static TeachingTip ShowTeachingTipAndHide(this UIElement xamlRoot, string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true, FrameworkElement? target = null)
    {
        var teachingTip = xamlRoot.CreateTeachingTip(target);
        teachingTip.ShowAndHide(title, icon, subtitle, mSec, isLightDismissEnabled);
        return teachingTip;
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
    /// Show <paramref name="teachingTip"/>
    /// </summary>
    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void Show(this TeachingTip teachingTip, string title, TeachingTipSeverity icon = TeachingTipSeverity.Ok, string subtitle = "", bool isLightDismissEnabled = false)
    {
        teachingTip.Show(title, icon.GetIconSource(), subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void ShowAndHide(this TeachingTip teachingTip, string title, IconSource? icon, string subtitle = "", int mSec = 3000, bool isLightDismissEnabled = true)
    {
        ref var hideTime = ref CollectionsMarshal.GetValueRefOrAddDefault(HideTimes, teachingTip, out var exists);

        if (!exists)
            teachingTip.Unloaded += (o, e) => HideTimes.Remove(teachingTip);

        hideTime = DateTime.Now + TimeSpan.FromMilliseconds(mSec - 100);

        teachingTip.Show(title, icon, subtitle, isLightDismissEnabled);

        Hide(teachingTip, hideTime, mSec);
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

    private static async void Hide(this TeachingTip teachingTip, DateTime hideTime, int mSec)
    {
        await Task.Delay(mSec);
        if (DateTime.Now > hideTime)
            teachingTip.IsOpen = false;
    }

    private static Dictionary<TeachingTip, DateTime> HideTimes { get; } = [];

    private static FontIconSource? GetIconSource(this TeachingTipSeverity severity)
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
                    TeachingTipSeverity.Processing => "\xE9F5", // Processing
                    _ => ThrowHelper.ArgumentOutOfRange<TeachingTipSeverity, string>(severity)
                }
            };
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="TeachingTip"></param>
/// <param name="HideSnakeBarTime">Value type members require property to enable thread sharing</param>
file record TeachingTipHelperCore(TeachingTip TeachingTip, DateTime HideSnakeBarTime);
