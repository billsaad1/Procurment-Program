using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public partial class AddProductViewModel : DialogViewModelBase<Product>
    {
        private readonly ProcurementManagerDbContext _context;

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

        public AddProductViewModel(ProcurementManagerDbContext context)
        {
            _context = context;
        }

        [RelayCommand]
        private async Task Save()
        {
            var newProduct = new Product
            {
                Name = this.Name,
                SKU = this.SKU,
                Unit = this.Unit,
                DefaultPrice = this.DefaultPrice,
                Category = this.Category,
                Description = this.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            OnSaved(newProduct);
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCanceled();
        }
    }
}
