using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Test;

public partial class App : Application
{
    public static AppConfig AppConfig { get; private set; } = null!;

    public App()
    {
        InitializeComponent();
        CurrentContext.Title = nameof(WinUI3Utilities);
        AppContext.InitializeConfigurationContainer();
        AppConfig = AppContext.LoadConfiguration() ?? new AppConfig();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        _ = new MainWindow();

        AppHelper.Initialize(new()
        {
            Size = WindowHelper.EstimatedWindowSize()
        });
    }
}