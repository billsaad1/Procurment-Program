using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public partial class EditProductViewModel : DialogViewModelBase<Product>
    {
        private readonly ProcurementManagerDbContext _context;
        private readonly Product _product;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _sku;

        [ObservableProperty]
        private string _unit;

        [ObservableProperty]
        private decimal _defaultPrice;

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private string _description;

        public EditProductViewModel(ProcurementManagerDbContext context, Product product)
        {
            _context = context;
            _product = _context.Products.Find(product.ProductID);

            // Initialize properties with the product's data
            Name = _product.Name;
            SKU = _product.SKU;
            Unit = _product.Unit;
            DefaultPrice = _product.DefaultPrice;
            Category = _product.Category;
            Description = _product.Description;
        }

        [RelayCommand]
        private async Task Save()
        {
            // Update the product's properties
            _product.Name = Name;
            _product.SKU = SKU;
            _product.Unit = Unit;
            _product.DefaultPrice = DefaultPrice;
            _product.Category = Category;
            _product.Description = Description;
            _product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            OnSaved(_product);
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCanceled();
        }
    }
}
