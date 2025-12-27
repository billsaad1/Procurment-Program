using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public partial class GoodsReceiptsViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<GoodsReceipt> _goodsReceipts;

        public GoodsReceiptsViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Goods Receipt History";
            _goodsReceipts = new ObservableCollection<GoodsReceipt>();

            LoadGoodsReceiptsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadGoodsReceipts()
        {
            var receipts = await _dbContext.GoodsReceipts
                .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Supplier)
                .Include(gr => gr.ReceivedByUser)
                .OrderByDescending(gr => gr.ReceiptDate)
                .ToListAsync();
            GoodsReceipts = new ObservableCollection<GoodsReceipt>(receipts);
        }
    }
}
