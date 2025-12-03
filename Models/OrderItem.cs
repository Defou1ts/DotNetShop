using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleShop.Models
{
    /// <summary>
    /// Отдельная строка (позиция) внутри заказа.
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required, StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Range(0.01, 100000)]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal Total => UnitPrice * Quantity;
    }
}


