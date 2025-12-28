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

namespace ProcurementManager.UI.ViewModels
{
    public partial class AddEditInvoiceViewModel : ValidationViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly Invoice? _invoice;

        [ObservableProperty]
        private ObservableCollection<PurchaseOrder> _purchaseOrders = new();

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private PurchaseOrder? _selectedPurchaseOrder;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private Supplier? _selectedSupplier;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        private string _invoiceNumber = string.Empty;

        [ObservableProperty]
        private DateTime _invoiceDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Today.AddDays(30);

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        private decimal _amount;

        [ObservableProperty]
        private decimal _taxAmount;

        public decimal TotalAmountDue => Amount + TaxAmount;
        public bool IsSupplierSelectionEnabled => SelectedPurchaseOrder == null;

        public AddEditInvoiceViewModel(ProcurementManagerDbContext dbContext, Invoice? invoice = null)
        {
            _dbContext = dbContext;
            _invoice = invoice;
            Title = invoice == null ? "Create Invoice" : "Edit Invoice";

            LoadPurchaseOrdersAndSuppliersCommand.Execute(null);

            if (invoice != null)
            {
                InvoiceNumber = invoice.InvoiceNumber;
                InvoiceDate = invoice.InvoiceDate;
                DueDate = invoice.DueDate;
                Amount = invoice.Amount;
                TaxAmount = invoice.TaxAmount;
                // Selected properties will be set after loading
            }

            // Listen to property changes to update TotalAmountDue
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Amount) || e.PropertyName == nameof(TaxAmount))
                {
                    OnPropertyChanged(nameof(TotalAmountDue));
                }
            };

            SaveCommand = new RelayCommand(() => {}, CanSave);
        }

        [RelayCommand]
        private async Task LoadPurchaseOrdersAndSuppliers()
        {
            PurchaseOrders = new ObservableCollection<PurchaseOrder>(await _dbContext.PurchaseOrders.ToListAsync());
            Suppliers = new ObservableCollection<Supplier>(await _dbContext.Suppliers.ToListAsync());

            if (_invoice != null)
            {
                SelectedPurchaseOrder = PurchaseOrders.FirstOrDefault(p => p.POID == _invoice.POID);
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierID == _invoice.SupplierID);
            }
        }

        partial void OnSelectedPurchaseOrderChanged(PurchaseOrder? value)
        {
            if (value != null)
            {
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierID == value.SupplierID);
                Amount = value.Items.Sum(i => i.TotalPrice);
            }
        }

        public override object GetResult()
        {
            var result = _invoice ?? new Invoice();

            result.POID = SelectedPurchaseOrder!.POID;
            result.SupplierID = SelectedSupplier!.SupplierID;
            result.InvoiceNumber = InvoiceNumber;
            result.InvoiceDate = InvoiceDate;
            result.DueDate = DueDate;
            result.Amount = Amount;
            result.TaxAmount = TaxAmount;
            result.TotalAmountDue = TotalAmountDue;
            result.UpdatedAt = DateTime.UtcNow;

            return result;
        }

        private new bool CanSave()
        {
            return !HasErrors && SelectedPurchaseOrder != null && SelectedSupplier != null;
        }
    }
}
