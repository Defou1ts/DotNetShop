using SimpleShop.Models;

namespace SimpleShop.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Создаём базу и таблицы если их нет
            context.Database.EnsureCreated();

            // теперь можно безопасно проверять наличие данных
            if (context.Categories.Any()) return;

            var cats = new[]
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Books" },
                new Category { Name = "Home" }
            };

            context.Categories.AddRange(cats);
            context.SaveChanges();

            var products = new[]
            {
                new Product { Name="Wireless Mouse", Price=19.99m, CategoryId=cats[0].Id, Description="Comfortable mouse" , Stock=50},
                new Product { Name="Keyboard", Price=29.50m, CategoryId=cats[0].Id, Description="Mechanical keyboard", Stock=20},
                new Product { Name="C# in Depth", Price=39.99m, CategoryId=cats[1].Id, Description="Programming book", Stock=12},
                new Product { Name="Coffee Mug", Price=9.99m, CategoryId=cats[2].Id, Description="Ceramic mug", Stock=100},
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
