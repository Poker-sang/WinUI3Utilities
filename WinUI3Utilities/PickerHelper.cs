using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static async Task<StorageFolder> PickSingleFolderAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FolderPicker
        {
            FileTypeFilter = { "*" }, /*不加会崩溃*/
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickSingleFolderAsync();

    /// <summary>
    /// Pick a single file
    /// </summary>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static async Task<StorageFile> PickSingleFileAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickSingleFileAsync();

    /// <summary>
    /// Pick multiple files
    /// </summary>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickMultipleFilesAsync();

    /// <summary>
    /// Pick a place to save file
    /// </summary>
    /// <param name="suggestedFileName"></param>
    /// <param name="fileTypeName"></param>
    /// <param name="fileTypeId">Wildcard characters</param>
    /// <param name="suggestedStartLocation"></param>
    /// <returns></returns>
    [Obsolete("Use other pickers instead")]
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(string suggestedFileName, string fileTypeName, string fileTypeId, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
        => new FileSavePicker
        {
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeChoices =
            {
                [fileTypeId] = new List<string> { fileTypeId }
            },
            SuggestedFileName = suggestedFileName
        }.InitializeWithWindow().PickSaveFileAsync();
}
