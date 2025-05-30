using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinUI3Utilities.Attributes;

[assembly: LocalizedStringResources(nameof(WinUI3Utilities))]

namespace WinUI3Utilities.Samples;

public partial class App : Application
{
    public static AppConfig AppConfig { get; private set; } = null!;

    public static MainWindow MainWindow { get; private set; } = null!;

    public App()
    {
        InitializeComponent();
        AppContext.InitializeConfiguration();
        AppConfig = AppContext.LoadConfiguration() ?? new AppConfig();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        MainWindow = new MainWindow();
        MainWindow.Initialize(new()
        {
            Size = new(500, 400),
            ExtendTitleBar = true,
            Title = nameof(Samples)
        });
        MainWindow.AppWindow.Show();
    }
}
