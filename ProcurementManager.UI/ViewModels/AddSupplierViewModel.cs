using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public partial class AddSupplierViewModel : DialogViewModelBase<Supplier>
    {
        private readonly ProcurementManagerDbContext _context;

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
        private string _status = "Active";

        public List<string> Statuses { get; } = new List<string> { "Active", "Inactive" };

        public AddSupplierViewModel(ProcurementManagerDbContext context)
        {
            _context = context;
        }

        [RelayCommand]
        private async Task Save()
        {
            var newSupplier = new Supplier
            {
                Name = this.Name,
                ContactPerson = this.ContactPerson,
                Email = this.Email,
                Phone = this.Phone,
                Address = this.Address,
                PaymentTerms = this.PaymentTerms,
                Category = this.Category,
                Status = this.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Suppliers.AddAsync(newSupplier);
            await _context.SaveChangesAsync();
            OnSaved(newSupplier);
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCanceled();
        }
    }
}
