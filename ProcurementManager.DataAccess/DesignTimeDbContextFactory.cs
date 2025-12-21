using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProcurementManager.DataAccess
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProcurementManagerDbContext>
    {
        public ProcurementManagerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProcurementManagerDbContext>();
            optionsBuilder.UseSqlite("Data Source=procurement.db");

            return new ProcurementManagerDbContext(optionsBuilder.Options);
        }
    }
}
