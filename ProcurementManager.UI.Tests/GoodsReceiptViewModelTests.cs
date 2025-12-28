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
    public class GoodsReceiptViewModelTests
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;
        private readonly Mock<IUserSessionService> _userSessionServiceMock;

        public GoodsReceiptViewModelTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<ProcurementManagerDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            dbContext.Database.EnsureCreated();

            _userSessionServiceMock = new Mock<IUserSessionService>();
            _userSessionServiceMock.Setup(s => s.CurrentUser).Returns(new User { UserID = 1, FullName = "Test User" });
        }

        private async Task<PurchaseOrder> SeedDatabaseAsync()
        {
            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var user = new User { UserID = 1, FullName = "Test User" };
            var supplier = new Supplier { SupplierID = 1, Name = "Test Supplier" };
            var product = new Product { ProductID = 1, Name = "Test Product", Unit = "pcs" };
            dbContext.Users.Add(user);
            dbContext.Suppliers.Add(supplier);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var po = new PurchaseOrder 
            {
                POID = 1, 
                IssuedByUserID = 1, 
                SupplierID = 1,
                Items = new[] { new PurchaseOrderItem { POItemID = 1, ProductID = 1, Quantity = 10, UnitPrice = 5 } }
            };
            dbContext.PurchaseOrders.Add(po);
            await dbContext.SaveChangesAsync();
            return po;
        }

        [Fact]
        public async Task ReceiveGoodsViewModel_GetResult_CreatesCorrectGoodsReceipt()
        {
            // Arrange
            var po = await SeedDatabaseAsync();
            var viewModel = new ReceiveGoodsViewModel(_userSessionServiceMock.Object, po);

            // Act
            viewModel.Items.First().ReceivedQuantity = 7;
            var result = viewModel.GetResult() as GoodsReceipt;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(po.POID, result.POID);
            Assert.Equal(1, result.ReceivedByUserID);
            Assert.Single(result.Items);
            Assert.Equal(7, result.Items.First().ReceivedQuantity);
            Assert.Equal(1, result.Items.First().POItemID);
        }

        [Fact]
        public async Task PurchaseOrdersViewModel_ReceiveGoodsCommand_UpdatesPoStatus()
        {
            // Arrange
            var seededPo = await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var dialogServiceMock = new Mock<IDialogService>();

            // The ViewModel's constructor will load all POs, including the one we seeded.
            var viewModel = new PurchaseOrdersViewModel(dbContext, dialogServiceMock.Object, _userSessionServiceMock.Object);

            // Set the selected PO from the ViewModel's collection to ensure it's tracked by the context.
            viewModel.SelectedPurchaseOrder = viewModel.PurchaseOrders.FirstOrDefault(p => p.POID == seededPo.POID);
            Assert.NotNull(viewModel.SelectedPurchaseOrder);

            var goodsReceipt = new GoodsReceipt
            {
                Items = new[] { new GoodsReceiptItem { POItemID = 1, ReceivedQuantity = 10 } }
            };
            dialogServiceMock.Setup(d => d.ShowDialogAsync<GoodsReceipt>(It.IsAny<IDialogViewModel>()))
                .ReturnsAsync(goodsReceipt);

            // Act
            await viewModel.ReceiveGoodsCommand.ExecuteAsync(null);

            // Assert
            // Use a new context to fetch the latest state from the database and avoid tracking issues.
            await using var assertDbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var updatedPo = await assertDbContext.PurchaseOrders.FindAsync(seededPo.POID);
            Assert.Equal("Received in Full", updatedPo?.Status);
        }
    }
}
