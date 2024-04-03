using Microsoft.UI;
using Microsoft.UI.Xaml;
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
    /// Window title
    /// </summary>
    /// <remarks>
    /// Default: ""
    /// </remarks>
    public string Title { get; set; } = "";

    /// <summary>
    /// Extend TitleBar if supported
    /// </summary>
    /// <remarks>
    /// Default: <see langword="true"/>
    /// </remarks>
    public bool ExtendTitleBar { get; set; } = true;

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
    /// Has the same effect as <see cref="IconId"/>
    /// <br/>
    /// Default: ""
    /// </remarks>
    public string IconPath { get; set; } = "";

    /// <summary>
    /// Icon id 
    /// </summary>
    /// <remarks>
    /// Has the same effect as <see cref="IconPath"/>
    /// <br/>
    /// Default: <see langword="default"/>
    /// </remarks>
    public IconId IconId { get; set; } = default;

    /// <summary>
    /// Is maximized
    /// </summary>
    /// <remarks>
    /// Default: <see langword="false"/>
    /// </remarks>
    public bool IsMaximized { get; set; }

    /// <summary>
    /// Theme
    /// </summary>
    /// <remarks>
    /// Default: <see cref="ElementTheme.Default"/>
    /// </remarks>
    public ElementTheme Theme { get; set; }
}
