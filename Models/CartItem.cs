using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleShop.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        [NotMapped]
        public decimal Total => UnitPrice * Quantity;
    }
}
