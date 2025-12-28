using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class GoodsReceipt
    {
        [Key]
        public int GRNID { get; set; }

        [Required]
        public int POID { get; set; }
        [ForeignKey("POID")]
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int ReceivedByUserID { get; set; }
        [ForeignKey("ReceivedByUserID")]
        public User? ReceivedByUser { get; set; }

        public string? Notes { get; set; }

        public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
