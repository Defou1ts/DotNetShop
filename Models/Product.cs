using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleShop.Models
{
    /// <summary>
    /// Товар, который отображается на витрине и может быть добавлен в корзину.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        [StringLength(250)]
        public string? ShortDescription { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(1024)]
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
