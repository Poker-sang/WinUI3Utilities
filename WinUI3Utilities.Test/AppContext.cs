using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[AppContext<AppConfig>(CastMethod = "WinUI3Utilities.Misc.ToNotNull")]
public static partial class AppContext
{
    public static AppConfig AppConfig { get; private set; } = null!;
}
