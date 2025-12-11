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
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly ProcurementManagerDbContext _context;
        private readonly IDialogService _dialogService;
        private ObservableCollection<Product> _allProducts;

        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        private string _searchText;

        public ProductsViewModel(ProcurementManagerDbContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        public async Task InitializeAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var products = await _context.Products.ToListAsync();
            _allProducts = new ObservableCollection<Product>(products);
            FilterProducts();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Products = new ObservableCollection<Product>(_allProducts);
            }
            else
            {
                Products = new ObservableCollection<Product>(_allProducts.Where(p => p.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)));
            }
        }

        [RelayCommand]
        private async Task AddNewProduct()
        {
            var viewModel = new AddProductViewModel(_context);
            var result = await _dialogService.ShowDialogAsync<Product>(viewModel);
            if (result != null)
            {
                await LoadProducts();
            }
        }

        [RelayCommand]
        private async Task EditProduct(Product product)
        {
            if (product != null)
            {
                var viewModel = new EditProductViewModel(_context, product);
                var result = await _dialogService.ShowDialogAsync<Product>(viewModel);
                if (result != null)
                {
                    await LoadProducts();
                }
            }
        }

        [RelayCommand]
        private async Task DeleteProduct(Product product)
        {
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                await LoadProducts();
            }
        }
    }
}
