using Avalonia.Headless.XUnit;
using ProcurementManager.UI.Views;
using Xunit;
using System.Threading.Tasks;
using ProcurementManager.UI;
using Avalonia.Controls.ApplicationLifetimes;

namespace ProcurementManager.UI.Tests
{
    public class UITests
    {
        [AvaloniaFact]
        public async Task Application_Initial_View_Is_LoginView()
        {
            var app = Application.Current;
            Assert.NotNull(app);

            var lifetime = (IClassicDesktopStyleApplicationLifetime)app.ApplicationLifetime;
            Assert.NotNull(lifetime);

            var mainWindow = lifetime.MainWindow;
            Assert.NotNull(mainWindow);

            await Task.Delay(100);

            Assert.IsType<LoginView>(mainWindow);
        }
    }
}
