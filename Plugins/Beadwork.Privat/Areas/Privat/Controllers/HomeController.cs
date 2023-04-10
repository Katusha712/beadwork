using Microsoft.AspNetCore.Mvc;

namespace Beadwork.Privat.Areas.Privat.Controllers
{
    [Area("Privat")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Callback()
        {
            return View();
        }
    }
}
