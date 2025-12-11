using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class SuppliersViewModel : ObservableObject
    {
        private readonly ProcurementManagerDbContext _context;
        private readonly IDialogService _dialogService;
        private ObservableCollection<Supplier> _allSuppliers;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers;

        [ObservableProperty]
        private string _searchText;

        public SuppliersViewModel(ProcurementManagerDbContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        public async Task InitializeAsync()
        {
            await LoadSuppliers();
        }

        private async Task LoadSuppliers()
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            _allSuppliers = new ObservableCollection<Supplier>(suppliers);
            FilterSuppliers();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterSuppliers();
        }

        private void FilterSuppliers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Suppliers = new ObservableCollection<Supplier>(_allSuppliers);
            }
            else
            {
                Suppliers = new ObservableCollection<Supplier>(_allSuppliers.Where(s => s.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)));
            }
        }

        [RelayCommand]
        private async Task AddNewSupplier()
        {
            var viewModel = new AddSupplierViewModel(_context);
            var result = await _dialogService.ShowDialogAsync<Supplier>(viewModel);
            if (result != null)
            {
                await LoadSuppliers();
            }
        }

        [RelayCommand]
        private async Task EditSupplier(Supplier supplier)
        {
            if (supplier != null)
            {
                var viewModel = new EditSupplierViewModel(_context, supplier);
                var result = await _dialogService.ShowDialogAsync<Supplier>(viewModel);
                if (result != null)
                {
                    await LoadSuppliers();
                }
            }
        }

        [RelayCommand]
        private async Task DeleteSupplier(Supplier supplier)
        {
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
                await LoadSuppliers();
            }
        }
    }
}
