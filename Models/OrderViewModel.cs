using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Models
{
    public class OrderViewModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Способ доставки")]
        public string DeliveryMethod { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Способ оплаты")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Подписаться на новости")]
        public bool SubscribeToNews { get; set; }

        [Display(Name = "Комментарий к заказу")]
        [StringLength(500)]
        public string? Comment { get; set; }

        public List<CartItem> Items { get; set; } = new();
    }
}
