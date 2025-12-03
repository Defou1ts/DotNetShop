using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SimpleShop.Controllers
{
    /// <summary>
    /// Отдаёт информационные страницы: главную и встроенное руководство пользователя.
    /// </summary>
    public class HomeController : Controller
    {
        // Главная доступна без авторизации, остальные действия по умолчанию защищены
        [AllowAnonymous]
        [ResponseCache(Duration = 30)]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Manual()
        {
            return View();
        }
    }
}
