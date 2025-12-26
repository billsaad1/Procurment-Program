using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int POItemID { get; set; }

        [Required]
        public int POID { get; set; }
        [ForeignKey("POID")]
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product? Product { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public string? Notes { get; set; }
    }
}
