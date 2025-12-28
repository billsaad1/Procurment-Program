using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class PurchaseRequisitionItem
    {
        [Key]
        public int PRItemID { get; set; }

        [Required]
        public int PRID { get; set; }
        [ForeignKey("PRID")]
        public PurchaseRequisition? PurchaseRequisition { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product? Product { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public string? Notes { get; set; }
    }
}
