using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Data;
using SimpleShop.Models;
using System.Security.Claims;

namespace SimpleShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CartController(ApplicationDbContext db) { _db = db; }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Пользователь не авторизован.");
            }
            return userId;
        }

        private List<CartItem> GetCart()
        {
            var userId = GetUserId();
            return _db.CartItems.Where(ci => ci.UserId == userId).ToList();
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            var p = await _db.Products.FindAsync(productId);
            if (p == null) return NotFound();

            var userId = GetUserId();
            var item = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (item == null)
            {
                item = new CartItem
                {
                    UserId = userId,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    UnitPrice = p.Price,
                    Quantity = qty
                };
                _db.CartItems.Add(item);
            }
            else
            {
                item.Quantity = Math.Min(100, item.Quantity + qty);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = GetUserId();
            var item = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (item != null)
            {
                item.Quantity = Math.Max(1, Math.Min(100, quantity));
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            var items = _db.CartItems.Where(ci => ci.UserId == userId && ci.ProductId == productId);

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Checkout (валидация, подсчёт суммы)
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            var vm = new OrderViewModel
            {
                FullName = User.Identity?.Name ?? string.Empty,
                Email = User.Identity?.Name ?? string.Empty,
                Items = cart
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // При ошибках валидации повторно подгружаем товары из корзины,
                // чтобы они отобразились в правой части формы
                model.Items = GetCart();
                return View(model);
            }

            // Демонстрация преобразований и подсчёта суммы
            var cart = GetCart();
            decimal total = cart.Sum(i => i.UnitPrice * i.Quantity);

            // Здесь производим "оформление заказа" — в реальности сохраняем в БД и т.д.
            TempData["OrderSuccess"] = $"Спасибо, {model.FullName}. Сумма заказа: {total:C}";

            // Очистка корзины текущего пользователя
            var userId = GetUserId();
            var items = _db.CartItems.Where(ci => ci.UserId == userId);
            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
