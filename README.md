# WinUI3Utilities

Useful template methods for WinUI3 projects

## Build

![.NET](https://img.shields.io/badge/.Net-7.0-512BD4?&style=for-the-badge&logo=.NET&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-10.0.19041-0078D6?&style=for-the-badge&logo=Windows&logoColor=white)

## Usage

The following is the general usage.
For more details, see the XAML docs of the corresponding attributes.

### Available APIs

#### Helpers

| Helpers | Usage |
| - | - |
| AppHelper | Helper for `Application` |
| WindowHelper | Get and set properties of `Window` |
| WindowSizeHelper | Limit the size of the window (works with `WindowSizeHelperAttribute`) |
| DragMoveAndResizeHelper | Apply drag-move and resize function to (`UIElement`)`RootPanel` |
| DragZoneHelper | Calculate non-dragging zones for `AppWindowTitleBar.SetDragRectangles` |
| TitleBarHelper | Set title bar |
| BackdropHelper | Set backdrop |
| TeachingTipHelper | Helper for `TeachingTip` |
| FrameworkElementHelper | Extension methods for `FrameworkElement` |
| Misc | Miscellaneous extension methods |
| ThrowHelper | Throw helper |

#### Attributes (Generators)

| Attributes | Usage |
| - | - |
| DependencyPropertyAttribute | Generate a dependency property |
| LocalizedStringResourcesAttribute | Generate for all the .resw files in `PRIResource` under the specified namespace |
| __SettingsViewModelAttribute__* | Generate property according to the properties of the settings class `T` for the specified viewmodel class |
| __GenerateConstructorAttribute__* | Generate constructor like `record` for the specified type, according to the properties of it |
| __AppContextAttribute__* | Generate field `_configurationContainer` and methods `Load/SaveConfiguration` for the specified class |
| WindowSizeHelperAttribute | Generate helper properties to limit the window size (works with `WindowSizeHelper`) |
| AttributeIgnoreAttribute | Ignores the effect of the specified attributes on this target |
| DisableSourceGeneratorAttribute | Indicates that the source generator is disabled. This is usually used for debug purpose |

* Asterisk ("*") or __boldness__ indicates that the attribute supports `AttributeIgnoreAttribute`

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
        AppConfig = LoadConfiguration() is not { } appConfigurations
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

### Localization ([Reference](https://platform.uno/blog/using-msbuild-items-and-properties-in-c-9-source-generators/))

Generate for all the .resw files in `PRIResource` under the specified namespace.

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
