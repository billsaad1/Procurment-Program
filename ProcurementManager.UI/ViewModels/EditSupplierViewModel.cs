using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public partial class EditSupplierViewModel : DialogViewModelBase<Supplier>
    {
        private readonly ProcurementManagerDbContext _context;
        private readonly Supplier _supplier;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _contactPerson;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _phone;

        [ObservableProperty]
        private string _address;

        [ObservableProperty]
        private string _paymentTerms;

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private string _status;

        public List<string> Statuses { get; } = new List<string> { "Active", "Inactive" };

        public EditSupplierViewModel(ProcurementManagerDbContext context, Supplier supplier)
        {
            _context = context;
            _supplier = _context.Suppliers.Find(supplier.SupplierID);

            // Initialize properties with the supplier's data
            Name = _supplier.Name;
            ContactPerson = _supplier.ContactPerson;
            Email = _supplier.Email;
            Phone = _supplier.Phone;
            Address = _supplier.Address;
            PaymentTerms = _supplier.PaymentTerms;
            Category = _supplier.Category;
            Status = _supplier.Status;
        }

        [RelayCommand]
        private async Task Save()
        {
            // Update the supplier's properties
            _supplier.Name = Name;
            _supplier.ContactPerson = ContactPerson;
            _supplier.Email = Email;
            _supplier.Phone = Phone;
            _supplier.Address = Address;
            _supplier.PaymentTerms = PaymentTerms;
            _supplier.Category = Category;
            _supplier.Status = Status;
            _supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            OnSaved(_supplier);
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCanceled();
        }
    }
}
