using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.ViewModels
{
    public partial class AddEditProductViewModel : ValidationViewModelBase
    {
        [ObservableProperty]
        [Required]
        [MinLength(2)]
        private string _name = string.Empty;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        [Required]
        private string _unit = string.Empty;

        [ObservableProperty]
        private decimal _defaultPrice;

        [ObservableProperty]
        private string? _sku;

        [ObservableProperty]
        private string? _category;

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }

        private readonly Product? _product;

        // Constructor for adding a new product
        public AddEditProductViewModel()
        {
            Title = "Add New Product";
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
        }

        // Constructor for editing an existing product
        public AddEditProductViewModel(Product product)
        {
            Title = $"Edit Product: {product.Name}";
            _product = product;

            Name = product.Name;
            Description = product.Description;
            Unit = product.Unit;
            DefaultPrice = product.DefaultPrice;
            Sku = product.SKU;
            Category = product.Category;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
        }

        public Product GetProduct()
        {
            var product = _product ?? new Product();
            product.Name = Name;
            product.Description = Description;
            product.Unit = Unit;
            product.DefaultPrice = DefaultPrice;
            product.SKU = Sku;
            product.Category = Category;
            product.UpdatedAt = System.DateTime.UtcNow;
            if (product.ProductID == 0)
            {
                product.CreatedAt = System.DateTime.UtcNow;
            }
            return product;
        }

        private void Save() { }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Unit);
        }
    }
}
