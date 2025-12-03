using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Models
{
    /// <summary>
    /// Хранит оформленный заказ с данными пользователя и списком позиций заказа.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string DeliveryMethod { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        public bool SubscribeToNews { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }
}


