using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Helper for <see cref="ContentDialog"/>
/// </summary>
public static class ContentDialogHelper
{
    /// <summary>
    /// Create a <see cref="ContentDialog"/> with <paramref name="uiElement"/>
    /// </summary>
    /// <param name="uiElement"></param>
    /// <returns></returns>
    public static ContentDialog CreateContentDialog(this UIElement uiElement)
    {
        return new() { XamlRoot = uiElement.XamlRoot };
    }

    /// <inheritdoc cref="ShowAsync"/>
    public static IAsyncOperation<ContentDialogResult> ShowContentDialogAsync(
        this UIElement uiElement,
        object? title,
        object? content,
        string primaryButtonText = "",
        string secondaryButtonText = "",
        string closeButtonText = "",
        ContentDialogButton defaultButton = ContentDialogButton.Primary,
        bool fullSizeDesired = false)
    {
        return uiElement.CreateContentDialog().ShowAsync(title, content, primaryButtonText, secondaryButtonText, closeButtonText, defaultButton, fullSizeDesired);
    }

    /// <summary>
    /// Show <paramref name="contentDialog"/>
    /// </summary>
    /// <param name="contentDialog"></param>
    /// <param name="title"><see cref="ContentDialog.Title"/></param>
    /// <param name="content"><see cref="ContentDialog.Content"/></param>
    /// <param name="primaryButtonText"><see cref="ContentDialog.PrimaryButtonText"/></param>
    /// <param name="secondaryButtonText"><see cref="ContentDialog.SecondaryButtonText"/></param>
    /// <param name="closeButtonText"><see cref="ContentDialog.CloseButtonText"/></param>
    /// <param name="defaultButton"><see cref="ContentDialog.DefaultButton"/></param>
    /// <param name="fullSizeDesired"><see cref="ContentDialog.FullSizeDesired"/></param>
    public static IAsyncOperation<ContentDialogResult> ShowAsync(
        this ContentDialog contentDialog,
        object? title,
        object? content,
        string primaryButtonText = "",
        string secondaryButtonText = "",
        string closeButtonText = "",
        ContentDialogButton defaultButton = ContentDialogButton.Primary,
        bool fullSizeDesired = false)
    {
        contentDialog.Title = title;
        contentDialog.Content = content;
        contentDialog.PrimaryButtonText = primaryButtonText;
        contentDialog.SecondaryButtonText = secondaryButtonText;
        contentDialog.CloseButtonText = closeButtonText;
        contentDialog.DefaultButton = defaultButton;
        contentDialog.FullSizeDesired = fullSizeDesired;
        return contentDialog.ShowAsync();
    }
}
