using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Models;

namespace SimpleShop.Data
{
    /// <summary>
    /// Главный контекст БД приложения, включающий сущности магазина и схемы Identity.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Разделы каталога (категории товаров).
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Карточки товаров, отображаемые на витрине.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Позиции корзины для аутентифицированных пользователей.
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; }

        /// <summary>
        /// Оформленные заказы.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Строки заказа (товары в заказе).
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(12, 2);
            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(10, 2);
        }
    }

}
