using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class PurchaseOrdersViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IDialogService _dialogService;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private ObservableCollection<PurchaseOrder> _purchaseOrders;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditPurchaseOrderCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeletePurchaseOrderCommand))]
        private PurchaseOrder? _selectedPurchaseOrder;

        public PurchaseOrdersViewModel(ProcurementManagerDbContext dbContext, IDialogService dialogService, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _dialogService = dialogService;
            _userSessionService = userSessionService;
            Title = "Purchase Orders";
            _purchaseOrders = new ObservableCollection<PurchaseOrder>();

            LoadPurchaseOrdersCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadPurchaseOrders()
        {
            var orders = await _dbContext.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.IssuedByUser)
                .Include(po => po.Items)
                .ThenInclude(item => item.Product)
                .ToListAsync();
            PurchaseOrders = new ObservableCollection<PurchaseOrder>(orders);
        }

        [RelayCommand]
        private async Task CreatePurchaseOrder()
        {
            var viewModel = new AddEditPurchaseOrderViewModel(_dbContext, _userSessionService);
            var newPo = await _dialogService.ShowDialogAsync<PurchaseOrder>(viewModel);

            if (newPo != null)
            {
                _dbContext.PurchaseOrders.Add(newPo);
                await _dbContext.SaveChangesAsync();
                await LoadPurchaseOrders();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task EditPurchaseOrder()
        {
            if (SelectedPurchaseOrder == null) return;

            var viewModel = new AddEditPurchaseOrderViewModel(_dbContext, _userSessionService, SelectedPurchaseOrder);
            var updatedPo = await _dialogService.ShowDialogAsync<PurchaseOrder>(viewModel);

            if (updatedPo != null)
            {
                _dbContext.Entry(SelectedPurchaseOrder).CurrentValues.SetValues(updatedPo);
                await _dbContext.SaveChangesAsync();
                await LoadPurchaseOrders();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task DeletePurchaseOrder()
        {
            if (SelectedPurchaseOrder == null) return;

            var confirmed = await _dialogService.ShowConfirmationDialogAsync(
                "Confirm Delete",
                $"Are you sure you want to delete PO #{SelectedPurchaseOrder.POID}?");

            if (confirmed)
            {
                _dbContext.PurchaseOrders.Remove(SelectedPurchaseOrder);
                await _dbContext.SaveChangesAsync();
                PurchaseOrders.Remove(SelectedPurchaseOrder);
            }
        }

        private bool CanEditOrDelete() => SelectedPurchaseOrder != null;
    }
}
