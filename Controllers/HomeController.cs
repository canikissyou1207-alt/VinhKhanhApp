using Microsoft.AspNetCore.Mvc;

namespace VinhKhanhApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "AdminPois");
    }
}
