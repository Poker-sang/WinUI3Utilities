using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Foundation;
using Windows.Storage;

namespace WinUI3Utilities;

/// <summary>
/// Basic setting value converter
/// </summary>
public class SettingsValueConverter : ISettingsValueConverter
{
    /// <summary>
    /// Must be set before use
    /// </summary>
    public static JsonSerializerContext Context { get; set; } = null!;

    /// <inheritdoc />
    public bool TryConvert<T>(T? value, out object? result)
    {
        switch (value)
        {
            case null:
                result = null;
                return typeof(T) == typeof(string) || typeof(T) == typeof(ApplicationDataCompositeValue);
            case Enum e:
                result = e.GetHashCode();
                return true;
            case sbyte or byte or short or ushort or int or uint or long or ulong or nint or nuint or float or double
                or bool or char or string or DateTimeOffset or TimeSpan or Guid or Point or Size or Rect
                or ApplicationDataCompositeValue:
                result = value;
                return true;
            case IEnumerable:
                result = JsonSerializer.Serialize(value, typeof(T), Context);
                return true;
            default:
                result = null;
                // "Only primitive, enumerable, enum and some other types are supported. " +
                // "For more information, see: https://learn.microsoft.com/windows/apps/design/app-settings/store-and-retrieve-app-data.");
                return false;
        }
    }

    /// <inheritdoc />
    public bool TryConvertBack<T>(object? value, bool isNullable, out T? result) where T : notnull
    {
        if (value is null && !isNullable)
        {
            result = default;
            return false;
        }
        result = value switch
        {
            null when !isNullable => ThrowHelper.ArgumentNull<object, T>(value),
            string s when typeof(T) != typeof(string) && typeof(T).IsAssignableTo(typeof(IEnumerable)) => (T)JsonSerializer.Deserialize(s, typeof(T), Context)!,
            _ => (T?)value
        };
        return true;
    }
}
