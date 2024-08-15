using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DuitkuMvpNetApp.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MsItemsId { get; set; }

        public string MsItemsNama { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public double MsItemsHarga { get; set; }

        [Required]
        [ForeignKey("Transaction")]
        public string MsItemsTransactionId { get; set; }

        // Navigation property for related Transaction
        public virtual Transaction Transaction { get; set; }
    }
}