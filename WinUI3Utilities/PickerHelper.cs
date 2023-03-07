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
    /// <inheritdoc cref="PickSingleFolderAsync(nint, PickerLocationId, PickerViewMode)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    public static IAsyncOperation<StorageFolder?> PickSingleFolderAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => PickSingleFolderAsync(CurrentContext.HWnd, suggestedStartLocation, viewMode);

    /// <summary>
    /// Pick a single folder
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<StorageFolder?> PickSingleFolderAsync(nint hWnd, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => new FolderPicker
        {
            FileTypeFilter = { "*" }, /*不加会崩溃*/
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(hWnd).PickSingleFolderAsync();

    /// <inheritdoc cref="PickSingleFileAsync(nint, PickerLocationId, PickerViewMode)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    public static IAsyncOperation<StorageFile?> PickSingleFileAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => PickSingleFileAsync(CurrentContext.HWnd, suggestedStartLocation, viewMode);

    /// <summary>
    /// Pick a single file
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<StorageFile?> PickSingleFileAsync(nint hWnd, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow(hWnd).PickSingleFileAsync();

    /// <inheritdoc cref="PickMultipleFilesAsync(nint, PickerLocationId, PickerViewMode)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    public static IAsyncOperation<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => PickMultipleFilesAsync(CurrentContext.HWnd, suggestedStartLocation, viewMode);

    /// <summary>
    /// Pick multiple files
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="suggestedStartLocation"></param>
    /// <param name="viewMode"></param>
    /// <returns></returns>
    public static IAsyncOperation<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(nint hWnd, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickMultipleFilesAsync();

    /// <inheritdoc cref="PickSaveFileAsync(nint, string, string, string, PickerLocationId)"/>
    /// <remarks>
    /// Assign Prerequisites:
    /// <list type="bullet">
    /// <item><term><see cref="CurrentContext.Window"/></term></item>
    /// </list>
    /// </remarks>
    [Obsolete($"Use {nameof(PickSingleFolderAsync)} instead")]
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(string suggestedFileName, string fileTypeName, string fileTypeId, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
        => PickSaveFileAsync(CurrentContext.HWnd, suggestedFileName, fileTypeName, fileTypeId, suggestedStartLocation);

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
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(nint hWnd, string suggestedFileName, string fileTypeName, string fileTypeId, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
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
