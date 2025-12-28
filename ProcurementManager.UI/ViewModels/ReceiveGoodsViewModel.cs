using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProcurementManager.Core.Models;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class ReceiveGoodsViewModel : ValidationViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly PurchaseOrder _purchaseOrder;

        [ObservableProperty]
        private DateTime _receiptDate = DateTime.Today;

        [ObservableProperty]
        private string? _notes;

        [ObservableProperty]
        private ObservableCollection<ReceivableItemViewModel> _items;

        public ReceiveGoodsViewModel(IUserSessionService userSessionService, PurchaseOrder purchaseOrder)
        {
            _userSessionService = userSessionService;
            _purchaseOrder = purchaseOrder;
            Title = $"Receive Goods for PO #{purchaseOrder.POID}";
            
            _items = new ObservableCollection<ReceivableItemViewModel>(
                _purchaseOrder.Items.Select(poi => new ReceivableItemViewModel(poi))
            );

            SaveCommand = new RelayCommand(Save, CanSave);
        }

        public override object GetResult()
        {
            var goodsReceipt = new GoodsReceipt
            {
                POID = _purchaseOrder.POID,
                ReceiptDate = ReceiptDate,
                Notes = Notes,
                ReceivedByUserID = _userSessionService.CurrentUser?.UserID ?? 0,
                Items = new Collection<GoodsReceiptItem>()
            };

            foreach (var item in Items.Where(i => i.ReceivedQuantity > 0))
            {
                goodsReceipt.Items.Add(new GoodsReceiptItem
                {
                    POItemID = item.PurchaseOrderItem.POItemID,
                    ReceivedQuantity = item.ReceivedQuantity,
                    QualityStatus = "OK" // Default status
                });
            }
            
            return goodsReceipt;
        }

        private void Save() { }
        private bool CanSave() => Items.Any(i => i.ReceivedQuantity > 0);
    }

    public partial class ReceivableItemViewModel : ObservableObject
    {
        public PurchaseOrderItem PurchaseOrderItem { get; }

        [ObservableProperty]
        private decimal _receivedQuantity;

        public ReceivableItemViewModel(PurchaseOrderItem purchaseOrderItem)
        {
            PurchaseOrderItem = purchaseOrderItem;
        }
    }
}
