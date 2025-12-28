using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
    public partial class AddEditPurchaseOrderViewModel : ValidationViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly PurchaseOrder? _po;

        [ObservableProperty]
        private Supplier? _selectedSupplier;

        [ObservableProperty]
        private DateTime? _deliveryDate;

        [ObservableProperty]
        private string? _shippingAddress;

        [ObservableProperty]
        private string? _paymentTerms;

        [ObservableProperty]
        private ObservableCollection<PurchaseOrderItem> _items = new();

        [ObservableProperty]
        private ObservableCollection<Supplier> _availableSuppliers = new();

        [ObservableProperty]
        private ObservableCollection<Product> _availableProducts = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private Product? _selectedProduct;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private int _newItemQuantity = 1;

        public IRelayCommand CancelCommand { get; }
        public IRelayCommand AddItemCommand { get; }
        public IRelayCommand<PurchaseOrderItem> RemoveItemCommand { get; }

        public AddEditPurchaseOrderViewModel(ProcurementManagerDbContext dbContext, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            Title = "Create Purchase Order";
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand<PurchaseOrderItem>(RemoveItem);
            LoadSuppliersAndProductsCommand.Execute(null);
        }

        public AddEditPurchaseOrderViewModel(ProcurementManagerDbContext dbContext, IUserSessionService userSessionService, PurchaseOrder po)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _po = po;
            Title = $"Edit Purchase Order #{po.POID}";

            SelectedSupplier = po.Supplier;
            DeliveryDate = po.DeliveryDate;
            ShippingAddress = po.ShippingAddress;
            PaymentTerms = po.PaymentTerms;
            Items = new ObservableCollection<PurchaseOrderItem>(po.Items);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand<PurchaseOrderItem>(RemoveItem);
            LoadSuppliersAndProductsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadSuppliersAndProducts()
        {
            var suppliers = await _dbContext.Suppliers.OrderBy(s => s.Name).ToListAsync();
            AvailableSuppliers = new ObservableCollection<Supplier>(suppliers);
            var products = await _dbContext.Products.OrderBy(p => p.Name).ToListAsync();
            AvailableProducts = new ObservableCollection<Product>(products);
        }

        private void AddItem()
        {
            if (SelectedProduct == null || NewItemQuantity <= 0) return;

            var newItem = new PurchaseOrderItem
            {
                ProductID = SelectedProduct.ProductID,
                Product = SelectedProduct,
                Quantity = NewItemQuantity,
                UnitPrice = SelectedProduct.DefaultPrice,
                TotalPrice = NewItemQuantity * SelectedProduct.DefaultPrice
            };
            Items.Add(newItem);
            SaveCommand.NotifyCanExecuteChanged();
        }

        private bool CanAddItem() => SelectedProduct != null && NewItemQuantity > 0;

        private void RemoveItem(PurchaseOrderItem? item)
        {
            if (item != null)
            {
                Items.Remove(item);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public override object GetResult()
        {
            var po = _po ?? new PurchaseOrder();
            po.SupplierID = SelectedSupplier?.SupplierID ?? 0;
            po.DeliveryDate = DeliveryDate;
            po.ShippingAddress = ShippingAddress;
            po.PaymentTerms = PaymentTerms;
            po.Items = Items;
            po.TotalAmount = Items.Sum(i => i.TotalPrice);
            po.UpdatedAt = System.DateTime.UtcNow;

            if (po.POID == 0)
            {
                po.CreatedAt = System.DateTime.UtcNow;
                po.OrderDate = System.DateTime.UtcNow;
                if (_userSessionService.CurrentUser == null)
                {
                    throw new System.Exception("No user is logged in.");
                }
                po.IssuedByUserID = _userSessionService.CurrentUser.UserID;
            }
            return po;
        }

        private void Save() { }
        private bool CanSave() => SelectedSupplier != null && Items.Any();
    }
}
