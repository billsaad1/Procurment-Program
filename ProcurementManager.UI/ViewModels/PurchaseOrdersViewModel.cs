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

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task ReceiveGoods()
        {
            if (SelectedPurchaseOrder == null) return;

            // Ensure the selected PO has all its items loaded
            await _dbContext.Entry(SelectedPurchaseOrder)
                .Collection(po => po.Items)
                .Query()
                .Include(item => item.Product)
                .LoadAsync();
            
            var viewModel = new ReceiveGoodsViewModel(_userSessionService, SelectedPurchaseOrder);
            var newReceipt = await _dialogService.ShowDialogAsync<GoodsReceipt>(viewModel);

            if (newReceipt != null && newReceipt.Items.Any())
            {
                // Ensure foreign keys are set correctly from the current context.
                newReceipt.POID = SelectedPurchaseOrder.POID;
                newReceipt.ReceivedByUserID = _userSessionService.CurrentUser!.UserID;

                _dbContext.GoodsReceipts.Add(newReceipt);

                // Update PO status
                var totalOrdered = SelectedPurchaseOrder.Items.Sum(i => i.Quantity);
                var allReceiptsForPo = await _dbContext.GoodsReceiptItems
                    .Where(gri => gri.PurchaseOrderItem.POID == SelectedPurchaseOrder.POID)
                    .ToListAsync();
                var totalReceived = allReceiptsForPo.Sum(i => i.ReceivedQuantity) + newReceipt.Items.Sum(i => i.ReceivedQuantity);

                if (totalReceived >= totalOrdered)
                {
                    SelectedPurchaseOrder.Status = "Received in Full";
                }
                else
                {
                    SelectedPurchaseOrder.Status = "Partially Received";
                }

                await _dbContext.SaveChangesAsync();
                await LoadPurchaseOrders();
            }
        }

        private bool CanEditOrDelete() => SelectedPurchaseOrder != null;
    }
}
