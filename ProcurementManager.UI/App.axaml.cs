using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProcurementManager.UI.ViewModels;
using ProcurementManager.UI.Views;

namespace ProcurementManager.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginViewModel = new LoginViewModel();
            var loginView = new LoginView
            {
                DataContext = loginViewModel
            };

            loginViewModel.LoginSuccessful += (sender, e) =>
            {
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                mainWindow.Show();
                desktop.MainWindow = mainWindow;
                loginView.Close();
            };

            desktop.MainWindow = loginView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
