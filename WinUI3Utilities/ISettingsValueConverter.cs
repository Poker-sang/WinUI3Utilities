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
    /// <returns>Only primitive types are allowed</returns>
    object? Convert<T>(T? value);

    /// <summary>
    /// Convert <paramref name="value"/> to type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    T ConvertBack<T>(object? value);
}
