using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class GoodsReceiptItem
    {
        [Key]
        public int GRItemID { get; set; }

        [Required]
        public int GRNID { get; set; }
        [ForeignKey("GRNID")]
        public GoodsReceipt? GoodsReceipt { get; set; }

        [Required]
        public int POItemID { get; set; }
        [ForeignKey("POItemID")]
        public PurchaseOrderItem? PurchaseOrderItem { get; set; }

        [Required]
        public decimal ReceivedQuantity { get; set; }

        [StringLength(50)]
        public string QualityStatus { get; set; } = "OK";
    }
}
