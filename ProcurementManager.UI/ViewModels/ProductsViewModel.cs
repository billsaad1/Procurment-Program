using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;
using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private ObservableCollection<Product> _products;
        private Product? _selectedProduct;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public IAsyncRelayCommand LoadProductsCommand { get; }

        public ProductsViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Products";
            _products = new ObservableCollection<Product>();

            LoadProductsCommand = new AsyncRelayCommand(LoadProducts);
            AddProductCommand = new RelayCommand(() => { /* TODO */ });
            EditProductCommand = new RelayCommand(() => { /* TODO */ }, () => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(() => { /* TODO */ }, () => SelectedProduct != null);

            LoadProductsCommand.Execute(null);
        }

        private async Task LoadProducts()
        {
            var products = await _dbContext.Products.Include(p => p.Supplier).ToListAsync();
            Products = new ObservableCollection<Product>(products);
        }
    }
}
