using System;
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
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;

        [ObservableProperty]
        private int _supplierCount;

        [ObservableProperty]
        private int _productCount;

        [ObservableProperty]
        private int _pendingRequisitionsCount;

        [ObservableProperty]
        private int _openPurchaseOrdersCount;

        [ObservableProperty]
        private decimal _monthlyPurchaseValue;

        [ObservableProperty]
        private ObservableCollection<PurchaseOrder> _recentPurchaseOrders = new();

        public IAsyncRelayCommand LoadDataCommand { get; }

        public DashboardViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Dashboard";
            LoadDataCommand = new AsyncRelayCommand(() => LoadData(null));
            LoadDataCommand.Execute(null);
        }

        public async Task LoadData(DateTime? now)
        {
            var currentTime = now ?? DateTime.Now;

            SupplierCount = await _dbContext.Suppliers.CountAsync();
            ProductCount = await _dbContext.Products.CountAsync();
            PendingRequisitionsCount = await _dbContext.PurchaseRequisitions
                .CountAsync(pr => pr.Status == "Pending Approval");

            OpenPurchaseOrdersCount = await _dbContext.PurchaseOrders
                .CountAsync(po => po.Status != "Received in Full" && po.Status != "Closed");

            var startOfMonth = new DateTime(currentTime.Year, currentTime.Month, 1);
            var monthlyOrders = await _dbContext.PurchaseOrders
                .Where(po => po.OrderDate >= startOfMonth && po.OrderDate <= currentTime)
                .ToListAsync();
            MonthlyPurchaseValue = monthlyOrders.Sum(po => po.TotalAmount);

            var recentOrders = await _dbContext.PurchaseOrders
                .Include(po => po.Supplier)
                .OrderByDescending(po => po.OrderDate)
                .Take(5)
                .ToListAsync();
            RecentPurchaseOrders = new ObservableCollection<PurchaseOrder>(recentOrders);
        }
    }
}
