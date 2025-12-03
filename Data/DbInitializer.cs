using SimpleShop.Models;

namespace SimpleShop.Data
{
    /// <summary>
    /// Инициализация БД начальными категориями и товарами.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Создаёт схему БД (если нужно) и заполняет базовыми категориями и товарами.
        /// Повторные вызовы безопасны: данные обновляются по имени товара/категории.
        /// </summary>
        public static void Initialize(ApplicationDbContext context)
        {
            // Создаём базу и таблицы если их нет
            context.Database.EnsureCreated();

            var categoriesSeed = new[]
            {
                "Electronics",
                "Books",
                "Home & Kitchen",
                "Fitness",
                "Gaming"
            };

            foreach (var categoryName in categoriesSeed)
            {
                if (!context.Categories.Any(c => c.Name == categoryName))
                {
                    context.Categories.Add(new Category { Name = categoryName });
                }
            }
            context.SaveChanges();

            var categoriesMap = context.Categories.ToDictionary(c => c.Name, c => c.Id);

            var products = new[]
            {
                new Product { Name="Wireless Mouse", Price=19.99m, ShortDescription="Беспроводная мышь с 2,4ГГц адаптером", Description="Удобная мышь с переключателем DPI и батареей на 12 месяцев.", Stock=50, CategoryId=categoriesMap["Electronics"], ImageUrl="https://images.unsplash.com/photo-1472851294608-062f824d29cc?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Mechanical Keyboard", Price=69.50m, ShortDescription="87 клавиш, синие свитчи", Description="Компактная механическая клавиатура с RGB подсветкой и металлической рамкой.", Stock=25, CategoryId=categoriesMap["Electronics"], ImageUrl="https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Noise Cancelling Headphones", Price=129.99m, ShortDescription="Bluetooth 5.0 + ANC", Description="Складные наушники с активным шумоподавлением и до 30 часов работы.", Stock=18, CategoryId=categoriesMap["Electronics"], ImageUrl="https://images.unsplash.com/photo-1487215078519-e21cc028cb29?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="C# in Depth 4th Ed.", Price=44.99m, ShortDescription="Иллюстрированное руководство по C#", Description="Актуальная книга по современному C# с примерами и упражнениями.", Stock=40, CategoryId=categoriesMap["Books"], ImageUrl="https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="ASP.NET Core Cookbook", Price=39.99m, ShortDescription="Практические рецепты", Description="Сборник рецептов для решения ежедневных задач ASP.NET Core.", Stock=30, CategoryId=categoriesMap["Books"], ImageUrl="https://images.unsplash.com/photo-1495446815901-a7297e633e8d?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Smart LED Bulb", Price=14.25m, ShortDescription="Wi-Fi лампа RGB", Description="Умная лампа с управлением через голосового ассистента и расписаниями.", Stock=100, CategoryId=categoriesMap["Home & Kitchen"], ImageUrl="https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Pour-over Coffee Set", Price=54.00m, ShortDescription="Набор для фильтр-кофе", Description="Стеклянный сервер, металлический фильтр и мерная ложка в комплекте.", Stock=22, CategoryId=categoriesMap["Home & Kitchen"], ImageUrl="https://images.unsplash.com/photo-1503481766315-7a586b20f66f?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Yoga Mat Pro", Price=29.90m, ShortDescription="Толщина 6мм", Description="Противоскользящий коврик из TPE с ремнем для переноски.", Stock=60, CategoryId=categoriesMap["Fitness"], ImageUrl="https://images.unsplash.com/photo-1549576490-b0b4831ef60a?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Adjustable Dumbbells", Price=159.00m, ShortDescription="2x24 кг", Description="Комплект регулируемых гантелей для домашних тренировок.", Stock=15, CategoryId=categoriesMap["Fitness"], ImageUrl="https://images.unsplash.com/photo-1586401100295-7a8096fd231d?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Gaming Mouse Pad XL", Price=24.99m, ShortDescription="900x400мм", Description="Большой коврик с прошитым краем и антискользящим основанием.", Stock=80, CategoryId=categoriesMap["Gaming"], ImageUrl="https://images.unsplash.com/photo-1545239351-1141bd82e8a6?auto=format&fit=crop&w=600&q=60"},
                new Product { Name="Next-gen Gamepad", Price=72.00m, ShortDescription="Совместим с ПК и консолями", Description="Беспроводной геймпад с виброоткликом и гравировкой кнопок.", Stock=35, CategoryId=categoriesMap["Gaming"], ImageUrl="https://images.unsplash.com/photo-1587202372775-98973d27b8ad?auto=format&fit=crop&w=600&q=60"}
            };

            foreach (var product in products)
            {
                var existing = context.Products.FirstOrDefault(p => p.Name == product.Name);
                if (existing == null)
                {
                    context.Products.Add(product);
                }
                else
                {
                    existing.Price = product.Price;
                    existing.ShortDescription = product.ShortDescription;
                    existing.Description = product.Description;
                    existing.Stock = product.Stock;
                    existing.ImageUrl = product.ImageUrl;
                    existing.CategoryId = product.CategoryId;
                }
            }
            context.SaveChanges();
        }
    }
}
