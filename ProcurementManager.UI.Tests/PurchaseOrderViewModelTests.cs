using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.Services;
using ProcurementManager.UI.ViewModels;
using Xunit;

namespace ProcurementManager.UI.Tests
{
    public class PurchaseOrderViewModelTests
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IUserSessionService> _userSessionServiceMock;

        public PurchaseOrderViewModelTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<ProcurementManagerDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            dbContext.Database.EnsureCreated();

            _dialogServiceMock = new Mock<IDialogService>();
            _userSessionServiceMock = new Mock<IUserSessionService>();
            _userSessionServiceMock.Setup(s => s.CurrentUser).Returns(new User { UserID = 1, FullName = "Test User" });
        }

        private async Task SeedDatabaseAsync()
        {
            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var user = new User { UserID = 1, FullName = "Test User" };
            var supplier = new Supplier { SupplierID = 1, Name = "Test Supplier" };
            dbContext.Users.Add(user);
            dbContext.Suppliers.Add(supplier);
            await dbContext.SaveChangesAsync();

            dbContext.PurchaseOrders.AddRange(
                new PurchaseOrder { POID = 1, IssuedByUserID = 1, SupplierID = 1 },
                new PurchaseOrder { POID = 2, IssuedByUserID = 1, SupplierID = 1 }
            );
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task LoadPurchaseOrdersCommand_Should_Load_All_Orders()
        {
            await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new PurchaseOrdersViewModel(dbContext, _dialogServiceMock.Object, _userSessionServiceMock.Object);

            await viewModel.LoadPurchaseOrdersCommand.ExecuteAsync(null);

            Assert.Equal(2, viewModel.PurchaseOrders.Count);
            Assert.Equal("Test User", viewModel.PurchaseOrders.First().IssuedByUser?.FullName);
            Assert.Equal("Test Supplier", viewModel.PurchaseOrders.First().Supplier?.Name);
        }

        [Fact]
        public async Task DeletePurchaseOrderCommand_Should_Remove_Order_When_Confirmed()
        {
            await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new PurchaseOrdersViewModel(dbContext, _dialogServiceMock.Object, _userSessionServiceMock.Object);
            await viewModel.LoadPurchaseOrdersCommand.ExecuteAsync(null);

            var poToDelete = viewModel.PurchaseOrders.First();
            viewModel.SelectedPurchaseOrder = poToDelete;

            _dialogServiceMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            await viewModel.DeletePurchaseOrderCommand.ExecuteAsync(null);

            Assert.Single(viewModel.PurchaseOrders);
            var poInDb = await dbContext.PurchaseOrders.FindAsync(poToDelete.POID);
            Assert.Null(poInDb);
        }
    }
}
