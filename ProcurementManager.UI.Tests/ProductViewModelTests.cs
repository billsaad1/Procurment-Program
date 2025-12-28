using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.ViewModels;
using Xunit;
using Moq;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.Tests
{
    public class ProductViewModelTests
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;
        private readonly Mock<IDialogService> _dialogServiceMock;

        public ProductViewModelTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<ProcurementManagerDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            dbContext.Database.EnsureCreated();
            
            _dialogServiceMock = new Mock<IDialogService>();
        }

        private async Task SeedDatabaseAsync()
        {
            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            dbContext.Products.AddRange(
                new Product { Name = "Product A", Unit = "piece" },
                new Product { Name = "Product B", Unit = "kg" }
            );
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task LoadProductsCommand_Should_Load_All_Products_From_Database()
        {
            await SeedDatabaseAsync();
            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new ProductsViewModel(dbContext, _dialogServiceMock.Object);

            await viewModel.LoadProductsCommand.ExecuteAsync(null);

            Assert.Equal(2, viewModel.Products.Count);
        }

        [Fact]
        public async Task DeleteProductCommand_Should_Remove_Product_When_Confirmed()
        {
            await SeedDatabaseAsync();
            using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new ProductsViewModel(dbContext, _dialogServiceMock.Object);
            await viewModel.LoadProductsCommand.ExecuteAsync(null);

            var productToDelete = viewModel.Products.First();
            viewModel.SelectedProduct = productToDelete;

            _dialogServiceMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            await viewModel.DeleteProductCommand.ExecuteAsync(null);

            Assert.Single(viewModel.Products);
            var productInDb = await dbContext.Products.FindAsync(productToDelete.ProductID);
            Assert.Null(productInDb);
        }
    }
}
