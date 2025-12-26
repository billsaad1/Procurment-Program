using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.ViewModels
{
    public partial class AddEditSupplierViewModel : ValidationViewModelBase
    {
        [ObservableProperty]
        [Required]
        [MinLength(2)]
        private string _name = string.Empty;

        [ObservableProperty]
        private string? _contactPerson;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [EmailAddress]
        private string? _email;

        [ObservableProperty]
        private string? _phone;

        [ObservableProperty]
        private string? _address;

        [ObservableProperty]
        private string? _paymentTerms;

        [ObservableProperty]
        private string? _category;

        [ObservableProperty]
        private string _status = "Active";

        public IRelayCommand CancelCommand { get; }

        private readonly Supplier? _supplier;

        public AddEditSupplierViewModel()
        {
            Title = "Add New Supplier";
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
        }

        public AddEditSupplierViewModel(Supplier supplier)
        {
            Title = $"Edit Supplier: {supplier.Name}";
            _supplier = supplier;

            Name = supplier.Name;
            ContactPerson = supplier.ContactPerson;
            Email = supplier.Email;
            Phone = supplier.Phone;
            Address = supplier.Address;
            PaymentTerms = supplier.PaymentTerms;
            Category = supplier.Category;
            Status = supplier.Status;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => { });
        }

        public override object GetResult()
        {
            var supplier = _supplier ?? new Supplier();
            supplier.Name = Name;
            supplier.ContactPerson = ContactPerson;
            supplier.Email = Email;
            supplier.Phone = Phone;
            supplier.Address = Address;
            supplier.PaymentTerms = PaymentTerms;
            supplier.Category = Category;
            supplier.Status = Status;
            supplier.UpdatedAt = System.DateTime.UtcNow;
            if (supplier.SupplierID == 0)
            {
                supplier.CreatedAt = System.DateTime.UtcNow;
            }
            return supplier;
        }

        private void Save() { }
        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);
    }
}
