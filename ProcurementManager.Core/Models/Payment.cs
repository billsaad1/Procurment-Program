using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcurementManager.Core.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int InvoiceID { get; set; }

        [ForeignKey("InvoiceID")]
        public virtual Invoice? Invoice { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaid { get; set; }

        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty; // e.g., Bank Transfer, Check, Credit Card

        [StringLength(255)]
        public string? ReferenceNumber { get; set; }

        public int? PaidByUserID { get; set; }

        [ForeignKey("PaidByUserID")]
        public virtual User? PaidByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
