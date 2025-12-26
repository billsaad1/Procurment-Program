using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.ViewModels;
using ProcurementManager.UI.Views;
using ProcurementManager.UI.Services;
using System;
using System.Data.Common;
using System.Linq;

namespace ProcurementManager.UI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static bool IsTestEnvironment { get; set; }

    // Keep the connection alive for the duration of the test run
    private static DbConnection? _dbConnection;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ProcurementManagerDbContext>();
            if (IsTestEnvironment)
            {
                dbContext.Database.EnsureCreated();
            }
            else
            {
                dbContext.Database.Migrate();
            }
            SeedDatabase(dbContext);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (s, e) => _dbConnection?.Dispose();

            var loginViewModel = Services.GetRequiredService<LoginViewModel>();
            var loginView = new LoginView
            {
                DataContext = loginViewModel
            };

            loginViewModel.LoginSuccessful += (sender, e) =>
            {
                var mainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>(),
                };
                mainWindow.Show();
                desktop.MainWindow = mainWindow;
                loginView.Close();
            };

            desktop.MainWindow = loginView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SeedDatabase(ProcurementManagerDbContext dbContext)
    {
        if (!dbContext.Users.Any())
        {
            dbContext.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = "admin",
                FullName = "Administrator",
                Email = "admin@example.com",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            dbContext.SaveChanges();
        }
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        if (IsTestEnvironment)
        {
            _dbConnection = new SqliteConnection("DataSource=:memory:");
            _dbConnection.Open();
            services.AddDbContext<ProcurementManagerDbContext>(options =>
                options.UseSqlite(_dbConnection), ServiceLifetime.Singleton);
        }
        else
        {
            services.AddDbContext<ProcurementManagerDbContext>(options =>
                options.UseSqlite("Data Source=procurement.db"), ServiceLifetime.Singleton);
        }

        services.AddTransient<LoginViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<SuppliersViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<PurchaseRequisitionsViewModel>();
        services.AddTransient<PurchaseOrdersViewModel>();

        // Views (for DI in DialogService)
        services.AddTransient<AddEditSupplierView>();
        services.AddTransient<AddEditProductView>();
        services.AddTransient<AddEditPurchaseRequisitionView>();
        services.AddTransient<AddEditPurchaseOrderView>();

        // Services
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IUserSessionService, UserSessionService>();
    }
}
