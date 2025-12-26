using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;

namespace ProcurementManager.DataAccess
{
    public class ProcurementManagerDbContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public ProcurementManagerDbContext(DbContextOptions<ProcurementManagerDbContext> options)
            : base(options)
        {
        }
    }
}
