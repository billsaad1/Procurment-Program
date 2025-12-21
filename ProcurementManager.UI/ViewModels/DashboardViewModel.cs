using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private int _supplierCount;
        private int _productCount;

        public int SupplierCount
        {
            get => _supplierCount;
            set => SetProperty(ref _supplierCount, value);
        }

        public int ProductCount
        {
            get => _productCount;
            set => SetProperty(ref _productCount, value);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        public DashboardViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Dashboard";
            LoadDataCommand = new AsyncRelayCommand(LoadData);
            LoadDataCommand.Execute(null);
        }

        private async Task LoadData()
        {
            SupplierCount = await _dbContext.Suppliers.CountAsync();
            ProductCount = await _dbContext.Products.CountAsync();
        }
    }
}
