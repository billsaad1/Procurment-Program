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
    public partial class PurchaseRequisitionsViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IDialogService _dialogService;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private ObservableCollection<PurchaseRequisition> _purchaseRequisitions;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditPurchaseRequisitionCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeletePurchaseRequisitionCommand))]
        private PurchaseRequisition? _selectedPurchaseRequisition;

        public PurchaseRequisitionsViewModel(ProcurementManagerDbContext dbContext, IDialogService dialogService, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _dialogService = dialogService;
            _userSessionService = userSessionService;
            Title = "Purchase Requisitions";
            _purchaseRequisitions = new ObservableCollection<PurchaseRequisition>();

            LoadPurchaseRequisitionsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadPurchaseRequisitions()
        {
            var requisitions = await _dbContext.PurchaseRequisitions
                .Include(pr => pr.RequestedByUser)
                .Include(pr => pr.Items)
                .ThenInclude(item => item.Product)
                .ToListAsync();
            PurchaseRequisitions = new ObservableCollection<PurchaseRequisition>(requisitions);
        }

        [RelayCommand]
        private async Task CreatePurchaseRequisition()
        {
            var viewModel = new AddEditPurchaseRequisitionViewModel(_dbContext, _userSessionService);
            var newPr = await _dialogService.ShowDialogAsync<PurchaseRequisition>(viewModel);

            if (newPr != null)
            {
                _dbContext.PurchaseRequisitions.Add(newPr);
                await _dbContext.SaveChangesAsync();
                await LoadPurchaseRequisitions(); // Reload to get all includes
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task EditPurchaseRequisition()
        {
            if (SelectedPurchaseRequisition == null) return;

            var viewModel = new AddEditPurchaseRequisitionViewModel(_dbContext, _userSessionService, SelectedPurchaseRequisition);
            var updatedPr = await _dialogService.ShowDialogAsync<PurchaseRequisition>(viewModel);

            if (updatedPr != null)
            {
                _dbContext.Entry(SelectedPurchaseRequisition).CurrentValues.SetValues(updatedPr);
                await _dbContext.SaveChangesAsync();
                await LoadPurchaseRequisitions(); // Reload to refresh data
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private async Task DeletePurchaseRequisition()
        {
            if (SelectedPurchaseRequisition == null) return;

            var confirmed = await _dialogService.ShowConfirmationDialogAsync(
                "Confirm Delete",
                $"Are you sure you want to delete PR #{SelectedPurchaseRequisition.PRID}?");

            if (confirmed)
            {
                _dbContext.PurchaseRequisitions.Remove(SelectedPurchaseRequisition);
                await _dbContext.SaveChangesAsync();
                PurchaseRequisitions.Remove(SelectedPurchaseRequisition);
            }
        }

        private bool CanEditOrDelete() => SelectedPurchaseRequisition != null;
    }
}
