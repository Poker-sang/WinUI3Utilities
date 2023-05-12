# WinUI3Utilities

Useful template methods for WinUI3 projects

## Build

![.NET](https://img.shields.io/badge/.Net-7.0-512BD4?&style=for-the-badge&logo=.NET&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-10.0.19041-0078D6?&style=for-the-badge&logo=Windows&logoColor=white)

## Usage

The following is the general usage.
For more details, see the XAML docs of the corresponding attributes.

### Initialize the `CurrentContext`

It is recommended to assign the `CurrentContext.Window` in the first line of the `MainWindow`'s constructor,
because it is possible that other methods in the program will call `CurrentContext.Window` before the end of the constructor.
This will avoid unassigned exceptions.

App.xaml.cs:

```csharp
using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace Sample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        CurrentContext.Title = nameof(Sample);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _ = new MainWindow();
        AppHelper.Initialize(new()
        {
            Size = WindowHelper.EstimatedWindowSize(),
            ...
        });
    }
}
```

MainWindow.xaml.cs:

```csharp
using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace Sample;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        CurrentContext.Window = this;
        InitializeComponent();
    }
}
```

### Settings Persistence

**Only for packaged applications.**

AppContext.cs:

```csharp
using Windows.Storage;
using WinUI3Utilities.Attributes;

namespace Sample;

[AppContext<AppConfig>]
public static partial class AppContext
{
    public static string AppLocalFolder { get; private set; } = null!;

    public static void Initialize()
    {
        AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
        InitializeConfigurationContainer();
        AppConfig = LoadConfiguration() is not { } appConfigurations)
            ? new() : appConfigurations;
    }

    public static void SetDefaultAppConfig() => AppConfig = new();

    public static AppConfig AppConfig { get; private set; } = null!;
}
```

AppConfig.cs:

```csharp
using WinUI3Utilities.Attributes;

namespace Sample;

[GenerateConstructor]
public partial record AppConfig
{
    public int WindowWidth { get; set; } = 1280;
    public int WindowHeight { get; set; } = 720;
    ...
}
```

When saving configuration, you can use the following code:

```csharp
AppContext.SaveConfiguration(AppContext.AppConfig)
```

### Localization

Generate all the .resw files under the specified namespace.

[Reference](https://platform.uno/blog/using-msbuild-items-and-properties-in-c-9-source-generators/)

Sample.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
    ... 
    <PropertyGroup>
        <EnableDefaultPriItems>false</EnableDefaultPriItems>
    </PropertyGroup>
    <Target Name="InjectAdditionalFiles" BeforeTargets="GenerateMSBuildEditorConfigFileShouldRun">
        <ItemGroup>
            <AdditionalFiles Include="@(PRIResource)" SourceItemGroup="PRIResource" />
        </ItemGroup>
    </Target>
    <ItemGroup>
        <CompilerVisibleItemMetadata Include="AdditionalFiles" MetadataName="SourceItemGroup" />
    </ItemGroup>
    ...
    <ItemGroup>
        <PRIResource Include="XXX\APage.resw" />
        <PRIResource Include="XXX\BWindow.resw" />
    </ItemGroup>
    ...
</Project>
```

AssemblyInfo.cs:

```csharp
[assembly: LocalizedStringResources(nameof(Sample))]
```

XXX\APage.resw: ...

XXX\BWindow.resw: ...

## Project Link 

[![NuGet](https://img.shields.io/badge/Nuget-WinUI3Utilities-004880?&style=for-the-badge&logo=NuGet&logoColor=white)](https://www.nuget.org/packages/WinUI3Utilities)
[![GitHub](https://img.shields.io/badge/GitHub-WinUI3Utilities-181717?&style=for-the-badge&logo=Github&logoColor=white)](https://github.com/Poker-sang/WinUI3Utilities)

## Contact Me

[![Poker](https://img.shields.io/badge/Poker-poker__sang@outlook.com-0078D4?style=for-the-badge&logo=microsoft-outlook&logoColor=white)](mailto:poker_sang@outlook.com)

## License

[MIT License](https://github.com/Poker-sang/WinUI3Utilities/blob/master/LICENSE)
