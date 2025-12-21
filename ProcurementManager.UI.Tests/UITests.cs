using Avalonia.Headless.XUnit;
using Xunit;
using ProcurementManager.UI;
using Avalonia;

namespace ProcurementManager.UI.Tests
{
    public class UITests
    {
        [AvaloniaFact]
        public void Application_Can_Be_Initialized()
        {
            var app = Application.Current;
            Assert.NotNull(app);
            Assert.True(App.IsTestEnvironment);
        }
    }
}
