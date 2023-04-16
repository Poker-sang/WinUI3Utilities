using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities.Attributes;

namespace WinUI3Utilities.Test;

[SettingsViewModel<AppConfig>(nameof(AppConfig))]
public partial class SettingsViewModel : ObservableObject
{
    public AppConfig AppConfig => AppContext.AppConfig;
}
