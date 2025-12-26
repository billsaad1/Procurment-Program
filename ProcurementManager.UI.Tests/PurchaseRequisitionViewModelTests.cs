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
    public class PurchaseRequisitionViewModelTests
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IUserSessionService> _userSessionServiceMock;

        public PurchaseRequisitionViewModelTests()
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
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            dbContext.PurchaseRequisitions.AddRange(
                new PurchaseRequisition { PRID = 1, RequestedByUserID = 1, Department = "IT" },
                new PurchaseRequisition { PRID = 2, RequestedByUserID = 1, Department = "HR" }
            );
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task LoadPurchaseRequisitionsCommand_Should_Load_All_Requisitions()
        {
            // Arrange
            await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new PurchaseRequisitionsViewModel(dbContext, _dialogServiceMock.Object, _userSessionServiceMock.Object);

            // Act
            await viewModel.LoadPurchaseRequisitionsCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, viewModel.PurchaseRequisitions.Count);
            Assert.Equal("Test User", viewModel.PurchaseRequisitions.First().RequestedByUser?.FullName);
        }

        [Fact]
        public async Task DeletePurchaseRequisitionCommand_Should_Remove_Requisition_When_Confirmed()
        {
            // Arrange
            await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new PurchaseRequisitionsViewModel(dbContext, _dialogServiceMock.Object, _userSessionServiceMock.Object);
            await viewModel.LoadPurchaseRequisitionsCommand.ExecuteAsync(null);

            var prToDelete = viewModel.PurchaseRequisitions.First();
            viewModel.SelectedPurchaseRequisition = prToDelete;

            _dialogServiceMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await viewModel.DeletePurchaseRequisitionCommand.ExecuteAsync(null);

            // Assert
            Assert.Single(viewModel.PurchaseRequisitions);
            var prInDb = await dbContext.PurchaseRequisitions.FindAsync(prToDelete.PRID);
            Assert.Null(prInDb);
        }

        [Fact]
        public async Task DeletePurchaseRequisitionCommand_Should_Not_Remove_Requisition_When_Not_Confirmed()
        {
            // Arrange
            await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var viewModel = new PurchaseRequisitionsViewModel(dbContext, _dialogServiceMock.Object, _userSessionServiceMock.Object);
            await viewModel.LoadPurchaseRequisitionsCommand.ExecuteAsync(null);

            viewModel.SelectedPurchaseRequisition = viewModel.PurchaseRequisitions.First();

            _dialogServiceMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            await viewModel.DeletePurchaseRequisitionCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, viewModel.PurchaseRequisitions.Count);
        }
    }
}
