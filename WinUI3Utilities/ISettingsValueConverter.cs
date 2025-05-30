using System.Diagnostics.CodeAnalysis;

namespace WinUI3Utilities;

/// <summary>
/// Convert object to the type that app settings support
/// </summary>
/// <remarks>
/// <seealso href="https://learn.microsoft.com/windows/apps/design/app-settings/store-and-retrieve-app-data"/>
/// </remarks>
public interface ISettingsValueConverter
{
    /// <summary>
    /// Convert <paramref name="value"/> to type 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns>Only primitive types are allowed</returns>
    bool TryConvert<T>(T? value, out object? result);

    /// <summary>
    /// Convert <paramref name="value"/> to type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="isNullable"><typeparamref name="T"/>is nullable</param>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryConvertBack<T>(object? value, bool isNullable, out T? result) where T : notnull;
}
