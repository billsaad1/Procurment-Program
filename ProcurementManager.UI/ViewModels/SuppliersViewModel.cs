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
    public partial class SuppliersViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditSupplierCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteSupplierCommand))]
        private Supplier? _selectedSupplier;

        public SuppliersViewModel(ProcurementManagerDbContext dbContext, IDialogService dialogService)
        {
            _dbContext = dbContext;
            _dialogService = dialogService;
            Title = "Suppliers";
            _suppliers = new ObservableCollection<Supplier>();

            LoadSuppliersCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadSuppliers()
        {
            var suppliers = await _dbContext.Suppliers.ToListAsync();
            Suppliers = new ObservableCollection<Supplier>(suppliers);
        }

        [RelayCommand]
        private async Task AddSupplier()
        {
            var viewModel = new AddEditSupplierViewModel();
            var newSupplier = await _dialogService.ShowDialogAsync<Supplier>(viewModel);

            if (newSupplier != null)
            {
                _dbContext.Suppliers.Add(newSupplier);
                await _dbContext.SaveChangesAsync();
                Suppliers.Add(newSupplier);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteSupplier))]
        private async Task EditSupplier()
        {
            if (SelectedSupplier == null) return;

            var viewModel = new AddEditSupplierViewModel(SelectedSupplier);
            var updatedSupplier = await _dialogService.ShowDialogAsync<Supplier>(viewModel);

            if (updatedSupplier != null)
            {
                _dbContext.Entry(SelectedSupplier).CurrentValues.SetValues(updatedSupplier);
                await _dbContext.SaveChangesAsync();

                // Refresh the collection to reflect changes
                var index = Suppliers.IndexOf(SelectedSupplier);
                if (index != -1)
                {
                    Suppliers[index] = updatedSupplier;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteSupplier))]
        private async Task DeleteSupplier()
        {
            if (SelectedSupplier == null) return;

            var confirmed = await _dialogService.ShowConfirmationDialogAsync(
                "Confirm Delete",
                $"Are you sure you want to delete the supplier '{SelectedSupplier.Name}'?");

            if (confirmed)
            {
                _dbContext.Suppliers.Remove(SelectedSupplier);
                await _dbContext.SaveChangesAsync();
                Suppliers.Remove(SelectedSupplier);
            }
        }

        private bool CanEditOrDeleteSupplier() => SelectedSupplier != null;
    }
}
