using System;

namespace ProcurementManager.Core.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public string? SKU { get; set; }
        public string? Category { get; set; }

        public int SupplierID { get; set; }
        public Supplier? Supplier { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
