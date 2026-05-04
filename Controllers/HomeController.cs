using Microsoft.AspNetCore.Mvc;
using VinhKhanhApi.Filters;

namespace VinhKhanhApi.Controllers
{
    [AdminAuthFilter]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}