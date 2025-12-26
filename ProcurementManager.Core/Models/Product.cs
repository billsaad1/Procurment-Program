using System;
using System.ComponentModel.DataAnnotations;

namespace ProcurementManager.Core.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        public decimal DefaultPrice { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
