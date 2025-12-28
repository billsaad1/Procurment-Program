using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.ViewModels;
using Xunit;

namespace ProcurementManager.UI.Tests
{
    public class DashboardViewModelTests : IDisposable
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;

        public DashboardViewModelTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<ProcurementManagerDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            dbContext.Database.EnsureCreated();
        }

        private async Task SeedDatabaseAsync(DateTime baseDate)
        {
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);

            var user = new User { UserID = 1, FullName = "Test User", Username = "test", PasswordHash = "a" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var supplier = new Supplier { SupplierID = 1, Name = "Supplier 1" };
            dbContext.Suppliers.Add(supplier);
            dbContext.Products.Add(new Product { Name = "Product 1" });
            await dbContext.SaveChangesAsync();

            dbContext.PurchaseRequisitions.AddRange(
                new PurchaseRequisition { Status = "Pending Approval", RequestedByUserID = user.UserID },
                new PurchaseRequisition { Status = "Approved", RequestedByUserID = user.UserID },
                new PurchaseRequisition { Status = "Pending Approval", RequestedByUserID = user.UserID }
            );

            var startOfMonth = new DateTime(baseDate.Year, baseDate.Month, 1);
            dbContext.PurchaseOrders.AddRange(
                new PurchaseOrder { POID = 1, Status = "Draft", OrderDate = startOfMonth.AddDays(1), TotalAmount = 100, IssuedByUserID = user.UserID, SupplierID = supplier.SupplierID },
                new PurchaseOrder { POID = 2, Status = "Received in Full", OrderDate = startOfMonth.AddDays(2), TotalAmount = 200, IssuedByUserID = user.UserID, SupplierID = supplier.SupplierID },
                new PurchaseOrder { POID = 3, Status = "Partially Received", OrderDate = baseDate.AddDays(-30), TotalAmount = 50, IssuedByUserID = user.UserID, SupplierID = supplier.SupplierID },
                new PurchaseOrder { POID = 4, Status = "Sent", OrderDate = startOfMonth.AddDays(3), TotalAmount = 150, IssuedByUserID = user.UserID, SupplierID = supplier.SupplierID },
                new PurchaseOrder { POID = 5, Status = "Closed", OrderDate = startOfMonth.AddDays(4), TotalAmount = 300, IssuedByUserID = user.UserID, SupplierID = supplier.SupplierID }
            );

            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task LoadData_CalculatesKpisCorrectly()
        {
            // Arrange
            var baseDate = new DateTime(2024, 7, 15);
            await SeedDatabaseAsync(baseDate);
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new DashboardViewModel(dbContext);

            // Act
            await viewModel.LoadData(baseDate);

            // Assert
            Assert.Equal(1, viewModel.SupplierCount);
            Assert.Equal(1, viewModel.ProductCount);
            Assert.Equal(2, viewModel.PendingRequisitionsCount);
            Assert.Equal(3, viewModel.OpenPurchaseOrdersCount); // Draft, Sent, and Partially Received
            Assert.Equal(750, viewModel.MonthlyPurchaseValue); // 100 + 200 + 150 + 300 from this month's orders
        }

        [Fact]
        public async Task LoadData_LoadsRecentPurchaseOrdersCorrectly()
        {
            // Arrange
            var baseDate = new DateTime(2024, 7, 15);
            await SeedDatabaseAsync(baseDate);
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new DashboardViewModel(dbContext);

            // Act
            await viewModel.LoadData(baseDate);

            // Assert
            Assert.Equal(5, viewModel.RecentPurchaseOrders.Count);
            Assert.Equal(5, viewModel.RecentPurchaseOrders.First().POID); // The latest one by date is POID 5
            Assert.Equal(300, viewModel.RecentPurchaseOrders.First().TotalAmount);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
