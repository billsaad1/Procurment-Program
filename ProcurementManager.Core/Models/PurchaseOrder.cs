using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int POID { get; set; }

        public int? PRID { get; set; }
        [ForeignKey("PRID")]
        public PurchaseRequisition? PurchaseRequisition { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public Supplier? Supplier { get; set; }

        [Required]
        public int IssuedByUserID { get; set; }
        [ForeignKey("IssuedByUserID")]
        public User? IssuedByUser { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [StringLength(500)]
        public string? BillingAddress { get; set; }

        [StringLength(100)]
        public string? PaymentTerms { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Draft";

        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
