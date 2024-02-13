using System;
using System.Collections;
using System.Text.Json;
using Windows.Foundation;
using Windows.Storage;

namespace WinUI3Utilities;

/// <summary>
/// Basic setting value converter
/// </summary>
public class SettingsValueConverter : ISettingsValueConverter
{
    /// <inheritdoc />
    public object? Convert<T>(T? value) =>
        value switch
        {
            null when typeof(T) == typeof(string) || typeof(T) == typeof(ApplicationDataCompositeValue) => null,
            null => ThrowHelper.ArgumentNull<object?, object?>(value),
            Enum e => e.GetHashCode(),
            IEnumerable => JsonSerializer.Serialize(value),
            sbyte or byte or short or ushort or int or uint or long or ulong or nint or nuint or float or double or bool
                or char or DateTimeOffset or TimeSpan or Guid or Point or Size or Rect
                or ApplicationDataCompositeValue => value,
            _ => ThrowHelper.ArgumentOutOfRange<object?, object?>(typeof(T),
                "Only primitive, enumerable, enum and some other types are supported. " +
                "For more information, see: https://learn.microsoft.com/windows/apps/design/app-settings/store-and-retrieve-app-data.")
        };

    /// <inheritdoc />
    public T ConvertBack<T>(object? value) =>
        value switch
        {
            null => default!,
            string s when typeof(T).IsAssignableTo(typeof(IEnumerable)) => JsonSerializer.Deserialize<T>(s)!,
            _ => (T)value
        };
}
