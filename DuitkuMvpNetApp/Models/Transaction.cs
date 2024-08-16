using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DuitkuMvpNetApp.Models
{
    public class Transaction
    {
        [Key]
        public string MsTransactionId { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be a positive number.")]
        public double MsTransactionTotalAmount { get; set; }

        [Required]
        public DateTime MsTransactionTimestamp { get; set; }

        [MaxLength(500)]
        public string MsTransactionDescription { get; set; } = string.Empty;

        [Required]
        public TransactionStatus MsTransactionStatus { get; set; } = TransactionStatus.Pending;

        [MaxLength(100)]
        public string MsTransactionReference { get; set; } = string.Empty;

        public virtual ICollection<Item> Items { get; set; } = new HashSet<Item>();
    }

    public enum TransactionStatus
    {
        Failed = 0,
        Success = 1,
        Pending = 2
    }
}