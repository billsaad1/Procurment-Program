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
    public partial class InvoicesViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IDialogService _dialogService;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private ObservableCollection<Invoice> _invoices;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditInvoiceCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteInvoiceCommand))]
        [NotifyCanExecuteChangedFor(nameof(RecordPaymentCommand))]
        private Invoice? _selectedInvoice;

        public InvoicesViewModel(ProcurementManagerDbContext dbContext, IDialogService dialogService, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _dialogService = dialogService;
            _userSessionService = userSessionService;
            Title = "Invoices";
            _invoices = new ObservableCollection<Invoice>();

            LoadInvoicesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadInvoices()
        {
            var invoices = await _dbContext.Invoices
                .Include(i => i.Supplier)
                .Include(i => i.PurchaseOrder)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
            Invoices = new ObservableCollection<Invoice>(invoices);
        }

        [RelayCommand]
        private async Task CreateInvoice()
        {
            var viewModel = new AddEditInvoiceViewModel(_dbContext);
            var newInvoice = await _dialogService.ShowDialogAsync<Invoice>(viewModel);
            
            if (newInvoice != null)
            {
                _dbContext.Invoices.Add(newInvoice);
                await _dbContext.SaveChangesAsync();
                await LoadInvoices();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteOrPay))]
        private async Task EditInvoice()
        {
            if (SelectedInvoice == null) return;
            
            var viewModel = new AddEditInvoiceViewModel(_dbContext, SelectedInvoice);
            var updatedInvoice = await _dialogService.ShowDialogAsync<Invoice>(viewModel);
            
            if (updatedInvoice != null)
            {
                _dbContext.Entry(SelectedInvoice).CurrentValues.SetValues(updatedInvoice);
                await _dbContext.SaveChangesAsync();
                await LoadInvoices();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteOrPay))]
        private async Task RecordPayment()
        {
            if (SelectedInvoice == null) return;

            await _dbContext.Entry(SelectedInvoice)
                .Collection(i => i.Payments)
                .LoadAsync();
            
            var viewModel = new RecordPaymentViewModel(SelectedInvoice, _userSessionService);
            var newPayment = await _dialogService.ShowDialogAsync<Payment>(viewModel);

            if (newPayment != null)
            {
                // By adding to the navigation property collection, EF's change tracker
                // will correctly mark the new payment as an insert.
                SelectedInvoice.Payments.Add(newPayment);

                // Update invoice status by summing the collection, which now includes the new payment.
                // This avoids the double-counting bug.
                var totalPaid = SelectedInvoice.Payments.Sum(p => p.AmountPaid);
                if (totalPaid >= SelectedInvoice.TotalAmountDue)
                {
                    SelectedInvoice.Status = "Paid";
                }
                else
                {
                    SelectedInvoice.Status = "Partially Paid";
                }

                await _dbContext.SaveChangesAsync();
                await LoadInvoices();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteOrPay))]
        private async Task DeleteInvoice()
        {
            if (SelectedInvoice == null) return;

            var confirmed = await _dialogService.ShowConfirmationDialogAsync(
                "Confirm Delete",
                $"Are you sure you want to delete Invoice #{SelectedInvoice.InvoiceNumber}?");

            if (confirmed)
            {
                _dbContext.Invoices.Remove(SelectedInvoice);
                await _dbContext.SaveChangesAsync();
                Invoices.Remove(SelectedInvoice);
            }
        }

        private bool CanEditOrDeleteOrPay() => SelectedInvoice != null;
    }
}
