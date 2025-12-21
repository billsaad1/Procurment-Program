using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProcurementManager.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly SuppliersViewModel _suppliersViewModel;
        private readonly ProductsViewModel _productsViewModel;

        public MainWindowViewModel(
            DashboardViewModel dashboardViewModel,
            SuppliersViewModel suppliersViewModel,
            ProductsViewModel productsViewModel)
        {
            _dashboardViewModel = dashboardViewModel;
            _suppliersViewModel = suppliersViewModel;
            _productsViewModel = productsViewModel;

            // Set default view to Dashboard
            _currentView = _dashboardViewModel;
            Title = _currentView.Title;
        }

        public ViewModelBase CurrentView
        {
            get => _currentView;
            set
            {
                SetProperty(ref _currentView, value);
                Title = value.Title;
            }
        }

        [RelayCommand]
        private void ShowDashboard()
        {
            CurrentView = _dashboardViewModel;
        }

        [RelayCommand]
        private void ShowSuppliers()
        {
            CurrentView = _suppliersViewModel;
        }

        [RelayCommand]
        private void ShowProducts()
        {
            CurrentView = _productsViewModel;
        }
    }
}
