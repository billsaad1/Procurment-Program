using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class PurchaseRequisition
    {
        [Key]
        public int PRID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int RequestedByUserID { get; set; }
        [ForeignKey("RequestedByUserID")]
        public User? RequestedByUser { get; set; }

        [StringLength(255)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? BudgetCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Draft";

        public int? ApprovalByUserID { get; set; }
        [ForeignKey("ApprovalByUserID")]
        public User? ApprovalByUser { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string? Notes { get; set; }

        public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
