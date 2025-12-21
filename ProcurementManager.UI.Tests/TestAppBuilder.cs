using Avalonia;
using Avalonia.Headless;
using Avalonia.Skia;
using ProcurementManager.UI;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseSkia()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions
        {
            UseHeadlessDrawing = false
        })
        .AfterSetup(_ => App.IsTestEnvironment = true);
}
