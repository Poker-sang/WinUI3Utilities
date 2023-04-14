using System;
using System.Collections.Generic;
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
    /// Pick a single folder
    /// </summary>
    /// <param name="hWnd">Default: <see cref="CurrentContext.HWnd"/></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<StorageFolder?> PickSingleFolderAsync(nint hWnd = 0, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        if (hWnd is 0)
            hWnd = (nint)CurrentContext.HWnd;
        return new FolderPicker
        {
            FileTypeFilter = { "*" }, /*不加会崩溃*/
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(hWnd).PickSingleFolderAsync();
    }

    /// <summary>
    /// Pick a single file
    /// </summary>
    /// <param name="hWnd">Default: <see cref="CurrentContext.HWnd"/></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<StorageFile?> PickSingleFileAsync(nint hWnd = 0, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        if (hWnd is 0)
            hWnd = (nint)CurrentContext.HWnd;
        return new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(hWnd).PickSingleFileAsync();
    }

    /// <summary>
    /// Pick multiple files
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(nint hWnd = 0, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        if (hWnd is 0)
            hWnd = (nint)CurrentContext.HWnd;
        return new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(hWnd).PickMultipleFilesAsync();
    }

    /// <summary>
    /// Pick a place to save file
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="suggestedFileName"></param>
    /// <param name="fileTypeName"></param>
    /// <param name="fileTypeId">Wildcard characters</param>
    /// <param name="suggestedStartLocation"></param>
    /// <returns></returns>
    [Obsolete($"Use {nameof(PickSingleFolderAsync)} instead")]
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(string suggestedFileName, string fileTypeName, string fileTypeId, nint hWnd = 0, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
    {
        return new FileSavePicker
        {
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeChoices = { [fileTypeId] = new List<string> { fileTypeId } },
            SuggestedFileName = suggestedFileName
        }.InitializeWithWindow(hWnd).PickSaveFileAsync();
    }
}
