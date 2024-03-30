using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WinUI3Utilities;

/// <summary>
/// A set of pickers
/// </summary>
public static class PickerHelper
{
    /// <summary>
    /// Call <see cref="WinRT.Interop.InitializeWithWindow.Initialize"/> and return <paramref name="obj"/> itself
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="obj"></param>
    /// <param name="window"></param>
    /// <returns><paramref name="obj"/></returns>
    private static T InitializeWithWindow<T>(this T obj, Window window)
    {
        var hWnd = (nint)window.AppWindow.Id.Value;
        // When running on win32, FileOpenPicker needs to know the top-level hWnd via IInitializeWithWindow::Initialize.
        WinRT.Interop.InitializeWithWindow.Initialize(obj, hWnd);
        return obj;
    }

    /// <summary>
    /// Pick a single folder
    /// </summary>
    /// <inheritdoc cref="PickSaveFileAsync"/>
    /// <returns></returns>
    public static IAsyncOperation<StorageFolder?> PickSingleFolderAsync(this Window window, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        return new FolderPicker
        {
            FileTypeFilter = { "*" }, /*不加会崩溃*/
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(window).PickSingleFolderAsync();
    }

    /// <summary>
    /// Pick a single file
    /// </summary>
    /// <inheritdoc cref="PickSaveFileAsync"/>
    /// <returns></returns>
    public static IAsyncOperation<StorageFile?> PickSingleFileAsync(this Window window, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        return new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(window).PickSingleFileAsync();
    }

    /// <summary>
    /// Pick multiple files
    /// </summary>
    /// <inheritdoc cref="PickSaveFileAsync"/>
    /// <returns></returns>
    public static IAsyncOperation<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(this Window window, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        return new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(window).PickMultipleFilesAsync();
    }

    /// <summary>
    /// Pick a place to save file. (It is suggested to use <see cref="PickSingleFolderAsync"/> instead)
    /// </summary>
    /// <param name="window"></param>
    /// <param name="suggestedFileName"></param>
    /// <param name="fileTypeName"></param>
    /// <param name="fileTypeId">Wildcard characters</param>
    /// <param name="suggestedStartLocation"></param>
    /// <returns></returns>
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(this Window window, string suggestedFileName, string fileTypeName, string fileTypeId, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
    {
        return new FileSavePicker
        {
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeChoices = { [fileTypeName] = [fileTypeId] },
            SuggestedFileName = suggestedFileName
        }.InitializeWithWindow(window).PickSaveFileAsync();
    }
}
