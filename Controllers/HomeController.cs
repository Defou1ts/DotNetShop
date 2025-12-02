using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SimpleShop.Controllers
{
    public class HomeController : Controller
    {
        // Главная доступна без авторизации, остальные действия по умолчанию защищены
        [AllowAnonymous]
        [ResponseCache(Duration = 30)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
