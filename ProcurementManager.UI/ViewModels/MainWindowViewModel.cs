using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProcurementManager.UI.Views;
using System;

namespace ProcurementManager.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private object _currentView;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Initialize()
        {
            await ShowSuppliers();
        }

        [RelayCommand]
        private async Task ShowSuppliers()
        {
            var suppliersViewModel = _serviceProvider.GetRequiredService<SuppliersViewModel>();
            await suppliersViewModel.InitializeAsync();
            CurrentView = new SuppliersView { DataContext = suppliersViewModel };
        }

        [RelayCommand]
        private async Task ShowProducts()
        {
            var productsViewModel = _serviceProvider.GetRequiredService<ProductsViewModel>();
            await productsViewModel.InitializeAsync();
            CurrentView = new ProductsView { DataContext = productsViewModel };
        }
    }
}
