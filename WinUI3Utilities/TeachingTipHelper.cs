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
    /// Create a <see cref="TeachingTip"/> with <paramref name="frameworkElement"/> and <paramref name="target"/>
    /// </summary>
    /// <param name="frameworkElement"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static TeachingTip CreateTeachingTip(this FrameworkElement frameworkElement, FrameworkElement? target = null)
    {
        var teachingTip = new TeachingTip
        {
            Target = target,
            IsOpen = false
        };
        frameworkElement.Resources.Add(teachingTip.GetHashCode().ToString(), teachingTip);
        teachingTip.Closed += (o, e) => frameworkElement.Resources.Remove(o.GetHashCode().ToString());
        return teachingTip;
    }

    /// <inheritdoc cref="Show(TeachingTip, string, IconSource?, string, bool)"/>
    public static void ShowTeachingTip(
        this FrameworkElement frameworkElement,
        string title,
        IconSource? icon,
        string subtitle = "",
        bool isLightDismissEnabled = false,
        FrameworkElement? target = null)
    {
        frameworkElement.CreateTeachingTip(target).Show(title, icon, subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="Show(TeachingTip, string, TeachingTipSeverity, string, bool)"/>
    public static void ShowTeachingTip(
        this FrameworkElement frameworkElement,
        string title,
        TeachingTipSeverity icon = TeachingTipSeverity.Ok,
        string subtitle = "",
        bool isLightDismissEnabled = false,
        FrameworkElement? target = null)
    {
        frameworkElement.CreateTeachingTip(target).Show(title, icon, subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, IconSource?, string, int, bool)"/>
    public static void ShowTeachingTipAndHide(
        this FrameworkElement frameworkElement,
        string title,
        IconSource? icon,
        string subtitle = "",
        int milliseconds = 3000,
        bool isLightDismissEnabled = true,
        FrameworkElement? target = null)
    {
        frameworkElement.CreateTeachingTip(target).ShowAndHide(title, icon, subtitle, milliseconds, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void ShowTeachingTipAndHide(
        this FrameworkElement frameworkElement,
        string title,
        TeachingTipSeverity icon = TeachingTipSeverity.Ok,
        string subtitle = "",
        int milliseconds = 3000,
        bool isLightDismissEnabled = true,
        FrameworkElement? target = null)
    {
        frameworkElement.CreateTeachingTip(target).ShowAndHide(title, icon, subtitle, milliseconds, isLightDismissEnabled);
    }

    /// <inheritdoc cref="Show(TeachingTip, string, TeachingTipSeverity, string, bool)"/>
    public static void Show(
        this TeachingTip teachingTip,
        string title,
        IconSource? icon,
        string subtitle = "",
        bool isLightDismissEnabled = false)
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
    public static void Show(
        this TeachingTip teachingTip,
        string title,
        TeachingTipSeverity icon = TeachingTipSeverity.Ok,
        string subtitle = "",
        bool isLightDismissEnabled = false)
    {
        teachingTip.Show(title, icon.GetIconSource(), subtitle, isLightDismissEnabled);
    }

    /// <inheritdoc cref="ShowAndHide(TeachingTip, string, TeachingTipSeverity, string, int, bool)"/>
    public static void ShowAndHide(
        this TeachingTip teachingTip,
        string title,
        IconSource? icon,
        string subtitle = "",
        int milliseconds = 3000,
        bool isLightDismissEnabled = true)
    {
        ref var hideTime = ref CollectionsMarshal.GetValueRefOrAddDefault(HideTimes, teachingTip, out var exists);

        if (!exists)
            teachingTip.Closed += (o, e) => HideTimes.Remove(o);

        hideTime = DateTime.Now + TimeSpan.FromMilliseconds(milliseconds - 100);

        teachingTip.Show(title, icon, subtitle, isLightDismissEnabled);

        Hide(teachingTip, hideTime, milliseconds);
    }

    /// <summary>
    /// Show <paramref name="teachingTip"/> and hide after <paramref name="milliseconds"/> microseconds
    /// </summary>
    /// <param name="teachingTip"></param>
    /// <param name="title"><see cref="TeachingTip.Title"/></param>
    /// <param name="icon"><see cref="TeachingTip.IconSource"/></param>
    /// <param name="subtitle"><see cref="TeachingTip.Subtitle"/></param>
    /// <param name="milliseconds">Automatically hide after <paramref name="milliseconds"/> milliseconds</param>
    /// <param name="isLightDismissEnabled"><see cref="TeachingTip.IsLightDismissEnabled"/></param>
    public static void ShowAndHide(
        this TeachingTip teachingTip,
        string title,
        TeachingTipSeverity icon = TeachingTipSeverity.Ok,
        string subtitle = "",
        int milliseconds = 3000,
        bool isLightDismissEnabled = true)
    {
        teachingTip.ShowAndHide(title, icon.GetIconSource(), subtitle, milliseconds, isLightDismissEnabled);
    }

    private static async void Hide(this TeachingTip teachingTip, DateTime hideTime, int milliseconds)
    {
        await Task.Delay(milliseconds);
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
