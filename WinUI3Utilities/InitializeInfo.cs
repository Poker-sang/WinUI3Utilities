using Microsoft.UI;
using Windows.Graphics;

namespace WinUI3Utilities;

/// <summary>
/// Info type for <see cref="WindowHelper.Initialize"/>
/// </summary>
public record InitializeInfo
{
    /// <summary>
    /// Window size
    /// </summary>
    /// <remarks>
    /// Default: <see langword="default"/>
    /// </remarks>
    public SizeInt32 Size { get; set; }

    /// <summary>
    /// TitleBar type
    /// </summary>
    /// <remarks>
    /// Default: <see cref="TitleBarType.Window"/>
    /// </remarks>
    public TitleBarType TitleBarType { get; set; } = TitleBarType.Window;

    /// <summary>
    /// Backdrop type
    /// </summary>
    /// <remarks>
    /// Default: <see cref="BackdropType.MicaAlt"/>
    /// </remarks>
    public BackdropType BackdropType { get; set; } = BackdropType.MicaAlt;

    /// <summary>
    /// Icon's absolute path
    /// </summary>
    /// <remarks>
    /// Has the same effect as <see cref="IconId"/>, and only works when using default titlebar (<see cref="TitleBarType.Neither"/>)
    /// <br/>
    /// Default: ""
    /// </remarks>
    public string IconPath { get; set; } = "";

    /// <summary>
    /// Icon id 
    /// </summary>
    /// <remarks>
    /// Has the same effect as <see cref="IconPath"/>, and only works when using default titlebar (<see cref="TitleBarType.Neither"/>)
    /// <br/>
    /// Default: <see langword="default"/>
    /// </remarks>
    public IconId IconId { get; set; } = default;
}
