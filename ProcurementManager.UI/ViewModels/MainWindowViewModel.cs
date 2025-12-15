using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProcurementManager.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;

        public MainWindowViewModel()
        {
            // Set default view to Dashboard
            _currentView = new DashboardViewModel();
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
            CurrentView = new DashboardViewModel();
        }

        [RelayCommand]
        private void ShowSuppliers()
        {
            CurrentView = new SuppliersViewModel();
        }

        [RelayCommand]
        private void ShowProducts()
        {
            CurrentView = new ProductsViewModel();
        }
    }
}
