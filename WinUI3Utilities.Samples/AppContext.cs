using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Samples;

[AppContext<AppConfig>(CastMethod = "WinUI3Utilities.Misc.ToNotNull")]
[AppContext<AppConfig>(MethodName = "Session", Type = ApplicationDataContainerType.Roaming, ConfigKey = "Session")]
public static partial class AppContext
{
    public static AppConfig AppConfig { get; private set; } = null!;
}
