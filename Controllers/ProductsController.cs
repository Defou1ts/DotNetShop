using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Data;
using SimpleShop.Models;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        public ProductsController(ApplicationDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        // Витрина раздела (кэшируем)
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Category(int? id)
        {
            string cacheKey = $"category_{id ?? 0}";
            var vm = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                if (id == null)
                {
                    var all = await _db.Products.Include(p => p.Category).ToListAsync();
                    return all;
                }
                var list = await _db.Products.Where(p => p.CategoryId == id).Include(p => p.Category).ToListAsync();
                return list;
            });

            return View(vm);
        }

        // Детали, добавление продукта (демо для админа)
        public async Task<IActionResult> Details(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }
    }
}
