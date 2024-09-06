using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Samples;

[AppContext<AppConfig, SettingsValueConverter>]
[AppContext<AppConfig>(MethodName = "Session", ConfigKey = "Session")]
public static partial class AppContext
{
    public static AppConfig AppConfig { get; private set; } = null!;
}
