namespace SimpleShop.Models
{
    /// <summary>
    /// Модель представления для витрины, объединяющая разделы, строку поиска и список товаров.
    /// </summary>
    public class CatalogViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();

        public int? SelectedCategoryId { get; set; }

        public string? SearchTerm { get; set; }

        public int TotalCategories => Categories.Count();

        public int TotalProducts => Products.Count();
    }
}


