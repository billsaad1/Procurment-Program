using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;
using ProcurementManager.Core.Models;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class ProductsViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private Product? _selectedProduct;

        public ProductsViewModel(ProcurementManagerDbContext dbContext, IDialogService dialogService)
        {
            _dbContext = dbContext;
            _dialogService = dialogService;
            Title = "Products";
            _products = new ObservableCollection<Product>();

            LoadProductsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            var products = await _dbContext.Products.ToListAsync();
            Products = new ObservableCollection<Product>(products);
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            var viewModel = new AddEditProductViewModel();
            var newProduct = await _dialogService.ShowDialogAsync<Product>(viewModel);

            if (newProduct != null)
            {
                _dbContext.Products.Add(newProduct);
                await _dbContext.SaveChangesAsync();
                Products.Add(newProduct);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteProduct))]
        private async Task EditProduct()
        {
            if (SelectedProduct == null) return;

            var viewModel = new AddEditProductViewModel(SelectedProduct);
            var updatedProduct = await _dialogService.ShowDialogAsync<Product>(viewModel);

            if (updatedProduct != null)
            {
                _dbContext.Entry(SelectedProduct).CurrentValues.SetValues(updatedProduct);
                await _dbContext.SaveChangesAsync();

                var index = Products.IndexOf(SelectedProduct);
                if (index != -1)
                {
                    Products[index] = updatedProduct;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteProduct))]
        private async Task DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var confirmed = await _dialogService.ShowConfirmationDialogAsync(
                "Confirm Delete",
                $"Are you sure you want to delete the product '{SelectedProduct.Name}'?");

            if (confirmed)
            {
                _dbContext.Products.Remove(SelectedProduct);
                await _dbContext.SaveChangesAsync();
                Products.Remove(SelectedProduct);
            }
        }

        private bool CanEditOrDeleteProduct() => SelectedProduct != null;
    }
}
