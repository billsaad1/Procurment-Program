using Microsoft.EntityFrameworkCore;
using ProcurementManager.Core.Models;

namespace ProcurementManager.DataAccess
{
    public class ProcurementManagerDbContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public ProcurementManagerDbContext(DbContextOptions<ProcurementManagerDbContext> options)
            : base(options)
        {
        }
    }
}
