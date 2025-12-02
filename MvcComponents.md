# SimpleShop — описание MVC-компонентов

## Модели (`Models`)

- **Product**

  - Поля: `Id`, `Name`, `Price`, `Description`, `CategoryId`, `Category`, `Stock`.
  - Содержит атрибуты валидации (`[Required]`, `[Range]`, `[StringLength]`).

- **Category**

  - Поля: `Id`, `Name`.
  - Используется для группировки товаров по категориям.

- **CartItem**

  - Поля: `ProductId`, `ProductName`, `UnitPrice`, `Quantity`, вычисляемое свойство `Total`.
  - Используется для представления позиции в корзине.

- **OrderViewModel**

  - Поля: `FullName`, `Email`, `Address`, `Phone`, `DeliveryMethod`, `PaymentMethod`, `SubscribeToNews`, `Comment`, `Items`.
  - Применяются атрибуты валидации для всех основных полей.

- **RegisterViewModel**

  - Поля: `Email`, `Password`, `ConfirmPassword`.
  - Используется на странице регистрации пользователя, содержит атрибуты валидации и сравнения пароля.

- **LoginViewModel**
  - Поля: `Email`, `Password`, `RememberMe`.
  - Используется на странице входа пользователя.

## Контроллеры (`Controllers`)

- **HomeController**

  - `Index()` — главная страница магазина, помечена атрибутом `[ResponseCache]` для кэширования.

- **ProductsController**

  - Зависимости: `ApplicationDbContext`, `IMemoryCache`.
  - `Category(int? id)` — витрина раздела (список товаров), использует кэширование в памяти и асинхронный доступ к данным.
  - `Details(int id)` — страница с деталями конкретного товара.

- **CartController**

  - Зависимость: `ApplicationDbContext`.
  - `Index()` — просмотр текущей корзины.
  - `AddToCart(int productId, int qty)` — добавление товара в корзину (асинхронный метод, подсчёт количества).
  - `UpdateQuantity(int productId, int quantity)` — изменение количества товара в корзине.
  - `Remove(int productId)` — удаление позиции из корзины.
  - `Checkout()` (GET) — показ формы оформления заказа.
  - `Checkout(OrderViewModel model)` (POST) — обработка формы, валидация, подсчёт итоговой суммы заказа и очистка корзины.

- **AccountController**
  - Зависимости: `UserManager<IdentityUser>`, `SignInManager<IdentityUser>`.
  - `Register()` (GET/POST) — регистрация нового пользователя.
  - `Login()` (GET/POST) — аутентификация пользователя.
  - `Logout()` (POST) — выход пользователя из системы.

## Представления (`Views`)

- **Home**

  - `Index.cshtml` — главная страница с описанием приложения, ссылками на витрину, корзину и краткой инструкцией (раздел «help»).

- **Products**

  - `Category.cshtml` — витрина товаров: таблица с товарами, формой добавления в корзину.
  - `Details.cshtml` — страница с деталями товара и формой добавления в корзину.

- **Cart**

  - `Index.cshtml` — корзина: список позиций, формы изменения количества и удаления, отображение общей суммы, переход к оформлению.
  - `Checkout.cshtml` — форма оформления заказа с различными компонентами веб-форм (текстовые поля, textarea, select, radio, checkbox и т.д.).

- **Account**

  - `Login.cshtml` — форма входа пользователя.
  - `Register.cshtml` — форма регистрации пользователя.

- **Shared**
  - `_Layout.cshtml` (в `Pages/Shared`) — общий макет приложения с меню навигации по основным разделам.
  - `_ValidationScriptsPartial.cshtml` (в `Views/Shared`) — подключение скриптов клиентской валидации.
