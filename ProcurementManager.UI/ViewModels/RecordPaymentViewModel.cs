using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class RecordPaymentViewModel : ValidationViewModelBase
    {
        private readonly Invoice _invoice;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private DateTime _paymentDate = DateTime.Today;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        private decimal _amountPaid;

        [ObservableProperty]
        private string _paymentMethod = "Bank Transfer";

        [ObservableProperty]
        private string? _referenceNumber;

        public RecordPaymentViewModel(Invoice invoice, IUserSessionService userSessionService)
        {
            _invoice = invoice;
            _userSessionService = userSessionService;
            Title = $"Record Payment for Invoice #{invoice.InvoiceNumber}";
            AmountPaid = _invoice.TotalAmountDue; // Default to the full amount

            SaveCommand = new RelayCommand(() => {}, CanSave);
        }

        public override object GetResult()
        {
            return new Payment
            {
                InvoiceID = _invoice.InvoiceID,
                PaymentDate = PaymentDate,
                AmountPaid = AmountPaid,
                PaymentMethod = PaymentMethod,
                ReferenceNumber = ReferenceNumber,
                PaidByUserID = _userSessionService.CurrentUser?.UserID
            };
        }

        private new bool CanSave()
        {
            return !HasErrors;
        }
    }
}
