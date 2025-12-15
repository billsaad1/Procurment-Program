using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Fluent;
using ProcurementManager.UI.Views;
using System.IO;
using Xunit;

namespace ProcurementManager.UI.Tests;

public class UITests
{
    [AvaloniaFact]
    public void LoginView_ShouldRenderCorrectly()
    {
        // Arrange
        Application.Current.Styles.Add(new FluentTheme());
        var loginView = new LoginView();

        // Act
        loginView.Show();

        // Assert
        var bitmap = new RenderTargetBitmap(new Avalonia.PixelSize((int)loginView.Width, (int)loginView.Height));
        bitmap.Render(loginView);

        var verificationDir = "/home/jules/verification";
        Directory.CreateDirectory(verificationDir);
        var screenshotPath = Path.Combine(verificationDir, "LoginView.png");
        bitmap.Save(screenshotPath);

        Assert.True(File.Exists(screenshotPath));
    }
}
