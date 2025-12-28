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
    public class InvoiceViewModelTests
    {
        private readonly DbContextOptions<ProcurementManagerDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;
        private readonly Mock<IUserSessionService> _userSessionServiceMock;

        public InvoiceViewModelTests()
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

        private async Task<Invoice> SeedDatabaseAsync()
        {
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);

            var user = new User { UserID = 1, FullName = "Test User" };
            var supplier = new Supplier { SupplierID = 1, Name = "Test Supplier" };
            dbContext.Users.Add(user);
            dbContext.Suppliers.Add(supplier);
            await dbContext.SaveChangesAsync();

            var po = new PurchaseOrder { POID = 1, IssuedByUserID = 1, SupplierID = 1 };
            dbContext.PurchaseOrders.Add(po);
            await dbContext.SaveChangesAsync();

            var invoice = new Invoice
            {
                InvoiceID = 1,
                POID = 1,
                SupplierID = 1,
                InvoiceNumber = "INV-001",
                TotalAmountDue = 100,
                Status = "Unpaid"
            };
            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            return invoice;
        }

        [Fact]
        public async Task RecordPaymentCommand_UpdatesInvoiceStatusToPaid()
        {
            // Arrange
            var seededInvoice = await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var dialogServiceMock = new Mock<IDialogService>();
            var viewModel = new InvoicesViewModel(dbContext, dialogServiceMock.Object, _userSessionServiceMock.Object);

            viewModel.SelectedInvoice = viewModel.Invoices.First(i => i.InvoiceID == seededInvoice.InvoiceID);

            var payment = new Payment { AmountPaid = 100 };
            dialogServiceMock.Setup(d => d.ShowDialogAsync<Payment>(It.IsAny<IDialogViewModel>()))
                .ReturnsAsync(payment);

            // Act
            await viewModel.RecordPaymentCommand.ExecuteAsync(null);

            // Assert
            await using var assertDbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var updatedInvoice = await assertDbContext.Invoices.FindAsync(seededInvoice.InvoiceID);
            Assert.Equal("Paid", updatedInvoice?.Status);
        }

        [Fact]
        public async Task RecordPaymentCommand_UpdatesInvoiceStatusToPartiallyPaid()
        {
            // Arrange
            var seededInvoice = await SeedDatabaseAsync();
            await using var dbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var dialogServiceMock = new Mock<IDialogService>();
            var viewModel = new InvoicesViewModel(dbContext, dialogServiceMock.Object, _userSessionServiceMock.Object);

            viewModel.SelectedInvoice = viewModel.Invoices.First(i => i.InvoiceID == seededInvoice.InvoiceID);

            var payment = new Payment { AmountPaid = 50 };
            dialogServiceMock.Setup(d => d.ShowDialogAsync<Payment>(It.IsAny<IDialogViewModel>()))
                .ReturnsAsync(payment);

            // Act
            await viewModel.RecordPaymentCommand.ExecuteAsync(null);

            // Assert
            await using var assertDbContext = new ProcurementManagerDbContext(_dbContextOptions);
            var updatedInvoice = await assertDbContext.Invoices.FindAsync(seededInvoice.InvoiceID);
            Assert.Equal("Partially Paid", updatedInvoice?.Status);
        }
    }
}
