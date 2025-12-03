using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Data;
using SimpleShop.Models;
using SimpleShop.Services;
using System.Security.Claims;

namespace SimpleShop.Controllers
{
    /// <summary>
    /// Контроллер управления корзиной, оформлением и подтверждением заказа.
    /// </summary>
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrderNotificationService _notifier;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext db, IOrderNotificationService notifier, ILogger<CartController> logger)
        {
            _db = db;
            _notifier = notifier;
            _logger = logger;
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Пользователь не авторизован.");
            }
            return userId;
        }

        private async Task<List<CartItem>> GetCartAsync()
        {
            var userId = GetUserId();
            return await _db.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetCartAsync();
            ViewBag.CartTotal = cart.Sum(i => i.Total);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            qty = Math.Max(1, qty);
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.Stock <= 0)
            {
                TempData["CartWarning"] = $"Товар «{product.Name}» сейчас отсутствует на складе.";
                return RedirectToAction("Category", "Products");
            }

            var userId = GetUserId();
            var item = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            var requestedQuantity = qty + (item?.Quantity ?? 0);
            requestedQuantity = Math.Min(100, requestedQuantity);

            if (requestedQuantity > product.Stock)
            {
                requestedQuantity = product.Stock;
                TempData["CartWarning"] = $"В наличии только {product.Stock} шт. товара «{product.Name}». Количество автоматически изменено.";
            }

            if (requestedQuantity <= 0)
            {
                TempData["CartWarning"] = $"Товар «{product.Name}» временно недоступен.";
                return RedirectToAction("Category", "Products");
            }

            if (item == null)
            {
                item = new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = requestedQuantity
                };
                _db.CartItems.Add(item);
            }
            else
            {
                item.Quantity = requestedQuantity;
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
                var product = await _db.Products.FindAsync(productId);
                if (product == null || product.Stock <= 0)
                {
                    _db.CartItems.Remove(item);
                    TempData["CartWarning"] = "Эта позиция больше не продаётся и была удалена из корзины.";
                }
                else
                {
                    var normalized = Math.Max(1, Math.Min(100, quantity));
                    if (normalized > product.Stock)
                    {
                        normalized = product.Stock;
                        TempData["CartWarning"] = $"Доступно только {product.Stock} шт. товара «{product.Name}».";
                    }
                    item.Quantity = normalized;
                }
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

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await GetCartAsync();
            if (!cart.Any())
            {
                TempData["CartWarning"] = "Корзина пуста.";
                return RedirectToAction("Index");
            }
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
            var cart = await GetCartAsync();
            if (!cart.Any())
            {
                ModelState.AddModelError(string.Empty, "Корзина пуста. Добавьте товары перед оформлением.");
            }

            var productIds = cart.Select(c => c.ProductId).ToArray();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var cartItem in cart)
            {
                if (!products.TryGetValue(cartItem.ProductId, out var product))
                {
                    ModelState.AddModelError(string.Empty, $"Товар «{cartItem.ProductName}» больше не доступен.");
                    continue;
                }

                if (product.Stock < cartItem.Quantity)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Товар «{product.Name}» доступен только в количестве {product.Stock} шт. Обновите корзину.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Items = cart;
                return View(model);
            }

            var order = new Order
            {
                UserId = GetUserId(),
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                Phone = model.Phone,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = model.PaymentMethod,
                SubscribeToNews = model.SubscribeToNews,
                Comment = model.Comment
            };

            foreach (var cartItem in cart)
            {
                var product = products[cartItem.ProductId];
                product.Stock -= cartItem.Quantity;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cart);

            await _db.SaveChangesAsync();

            try
            {
                await _notifier.NotifyAsync(order, model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось отправить email с подтверждением заказа {OrderId}", order.Id);
            }

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = GetUserId();
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}
