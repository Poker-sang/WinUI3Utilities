namespace WinUI3Utilities;

/// <summary>
/// Backdrop type
/// </summary>
public enum BackdropType
{
    /// <summary>
    /// None
    /// </summary>
    None,

    /// <summary>
    /// Desktop Acrylic
    /// </summary>
    Acrylic,

    /// <summary>
    /// Mica Base
    /// </summary>
    /// <remarks>Mica isn't supported on all systems. Where it's not supported, a solid color is used instead of the Mica effect.</remarks>
    Mica,

    /// <summary>
    /// Mica Base Alt
    /// </summary>
    /// <remarks>Mica isn't supported on all systems. Where it's not supported, a solid color is used instead of the Mica effect.</remarks>
    MicaAlt,

    /// <summary>
    /// Maintaining the status quo
    /// </summary>
    Maintain
}
