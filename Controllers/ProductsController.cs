using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Data;
using SimpleShop.Models;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleShop.Controllers
{
    /// <summary>
    /// Контроллер витрины: просмотр каталога, поиск товаров и детальная страница товара.
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private const string CategoriesCacheKey = "catalog_categories";
        private const string DefaultPlaceholderImage = "/images/product-placeholder.svg";
        private static readonly Dictionary<string, string> DefaultImageMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Wireless Mouse"] = "https://images.unsplash.com/photo-1472851294608-062f824d29cc?auto=format&fit=crop&w=600&q=60",
            ["Mechanical Keyboard"] = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=600&q=60",
            ["Noise Cancelling Headphones"] = "https://images.unsplash.com/photo-1487215078519-e21cc028cb29?auto=format&fit=crop&w=600&q=60",
            ["C# in Depth 4th Ed."] = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=600&q=60",
            ["ASP.NET Core Cookbook"] = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?auto=format&fit=crop&w=600&q=60",
            ["Smart LED Bulb"] = "https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&w=600&q=60",
            ["Pour-over Coffee Set"] = "https://images.unsplash.com/photo-1503481766315-7a586b20f66f?auto=format&fit=crop&w=600&q=60",
            ["Yoga Mat Pro"] = "https://images.unsplash.com/photo-1549576490-b0b4831ef60a?auto=format&fit=crop&w=600&q=60",
            ["Adjustable Dumbbells"] = "https://images.unsplash.com/photo-1586401100295-7a8096fd231d?auto=format&fit=crop&w=600&q=60",
            ["Gaming Mouse Pad XL"] = "https://images.unsplash.com/photo-1545239351-1141bd82e8a6?auto=format&fit=crop&w=600&q=60",
            ["Next-gen Gamepad"] = "https://images.unsplash.com/photo-1587202372775-98973d27b8ad?auto=format&fit=crop&w=600&q=60"
        };

        public ProductsController(ApplicationDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        /// <summary>
        /// Страница витрины с необязательной фильтрацией по разделу и строке поиска.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Category(int? id, string? searchTerm)
        {
            var categories = await GetCategoriesAsync();
            var productsQuery = _db.Products.Include(p => p.Category).AsNoTracking();

            if (id.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == id.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(term) ||
                    (p.ShortDescription != null && p.ShortDescription.Contains(term)) ||
                    (p.Description != null && p.Description.Contains(term)));
            }

            var products = await productsQuery.OrderBy(p => p.Name).ToListAsync();
            foreach (var product in products)
            {
                product.ImageUrl = ResolveImageUrl(product);
            }
            var vm = new CatalogViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = id,
                SearchTerm = searchTerm
            };

            return View(vm);
        }

        /// <summary>
        /// Отображает подробную информацию по выбранному товару.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var p = await _db.Products
                .AsNoTracking()
                .Include(prod => prod.Category)
                .FirstOrDefaultAsync(prod => prod.Id == id);
            if (p == null) return NotFound();
            p.ImageUrl = ResolveImageUrl(p);
            return View(p);
        }

        /// <summary>
        /// Точка входа для формы поиска в шапке. Перенаправляет на витрину с указанными фильтрами.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Search(string? term, int? categoryId)
        {
            return RedirectToAction(nameof(Category), new { id = categoryId, searchTerm = term });
        }

        private async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _cache.GetOrCreateAsync(CategoriesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            });

            return categories ?? Enumerable.Empty<Category>();
        }

        private string ResolveImageUrl(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                return product.ImageUrl;
            }

            if (DefaultImageMap.TryGetValue(product.Name, out var defaultUrl))
            {
                return defaultUrl;
            }

            return DefaultPlaceholderImage;
        }
    }
}
