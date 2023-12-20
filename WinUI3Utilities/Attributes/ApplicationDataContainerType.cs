using Windows.Storage;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Type of <see cref="ApplicationDataContainer"/>
/// </summary>
public enum ApplicationDataContainerType
{
    /// <summary>
    /// <see cref="ApplicationData.LocalSettings"/>
    /// </summary>
    Local,

    /// <summary>
    /// <see cref="ApplicationData.RoamingSettings"/>
    /// </summary>
    Roaming
}
