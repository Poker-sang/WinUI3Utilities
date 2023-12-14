using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Samples;

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
        new MainWindow().Initialize(new()
        {
            Size = new(500, 400),
            TitleBarType = TitleBarType.AppWindow,
        });
        CurrentContext.Window.SetAppWindowTitleBarButtonColor(false);
        CurrentContext.AppWindow.Show();
    }
}
