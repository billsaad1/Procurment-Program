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
    public partial class AddEditPurchaseRequisitionViewModel : ValidationViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly PurchaseRequisition? _pr;

        [ObservableProperty]
        private string? _department;

        [ObservableProperty]
        private string? _budgetCode;

        [ObservableProperty]
        private string? _notes;

        [ObservableProperty]
        private ObservableCollection<PurchaseRequisitionItem> _items = new();

        [ObservableProperty]
        private ObservableCollection<Product> _availableProducts = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private Product? _selectedProductToAdd;

        [ObservableProperty]
        private decimal _quantityToAdd = 1;

        public IRelayCommand CancelCommand { get; }

        public AddEditPurchaseRequisitionViewModel(ProcurementManagerDbContext dbContext, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            Title = "Create Purchase Requisition";
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
            LoadProductsCommand.Execute(null);
        }

        public AddEditPurchaseRequisitionViewModel(ProcurementManagerDbContext dbContext, IUserSessionService userSessionService, PurchaseRequisition pr)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _pr = pr;
            Title = $"Edit Purchase Requisition #{pr.PRID}";

            Department = pr.Department;
            BudgetCode = pr.BudgetCode;
            Notes = pr.Notes;
            Items = new ObservableCollection<PurchaseRequisitionItem>(pr.Items);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
            LoadProductsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            var products = await _dbContext.Products.OrderBy(p => p.Name).ToListAsync();
            AvailableProducts = new ObservableCollection<Product>(products);
        }

        [RelayCommand(CanExecute = nameof(CanAddItem))]
        private void AddItem()
        {
            if (SelectedProductToAdd == null) return;

            var newItem = new PurchaseRequisitionItem
            {
                ProductID = SelectedProductToAdd.ProductID,
                Product = SelectedProductToAdd,
                Quantity = QuantityToAdd,
                UnitPrice = SelectedProductToAdd.DefaultPrice
            };
            Items.Add(newItem);
            QuantityToAdd = 1;
            SelectedProductToAdd = null;
        }

        private bool CanAddItem() => SelectedProductToAdd != null && QuantityToAdd > 0;

        [RelayCommand]
        private void RemoveItem(PurchaseRequisitionItem item)
        {
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public override object GetResult()
        {
            var pr = _pr ?? new PurchaseRequisition();
            pr.Department = Department;
            pr.BudgetCode = BudgetCode;
            pr.Notes = Notes;
            pr.Items = Items;
            pr.UpdatedAt = System.DateTime.UtcNow;
            if (pr.PRID == 0)
            {
                pr.CreatedAt = System.DateTime.UtcNow;
                pr.RequestDate = System.DateTime.UtcNow;
                if (_userSessionService.CurrentUser == null)
                {
                    throw new System.Exception("No user is logged in.");
                }
                pr.RequestedByUserID = _userSessionService.CurrentUser.UserID;
            }
            return pr;
        }

        private void Save() { }
        private bool CanSave() => Items.Any();
    }
}
