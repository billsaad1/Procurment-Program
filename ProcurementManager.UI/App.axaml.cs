using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.Services;
using ProcurementManager.UI.ViewModels;
using ProcurementManager.UI.Views;
using System;

namespace ProcurementManager.UI
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }



        public override void OnFrameworkInitializationCompleted()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddDbContext<ProcurementManagerDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<SuppliersViewModel>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<AddSupplierViewModel>();
            services.AddTransient<EditSupplierViewModel>();
            services.AddTransient<AddProductViewModel>();
            services.AddTransient<EditProductViewModel>();

            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
                mainWindowViewModel.Initialize();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
