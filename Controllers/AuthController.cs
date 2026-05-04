using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Data;
using VinhKhanhApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace VinhKhanhApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("Admin/Login")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("Admin/Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var hash = HashPassword(password);
            var user = await _db.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            HttpContext.Session.SetString("AdminLoggedIn", "true");
            HttpContext.Session.SetString("AdminUsername", username);
            return RedirectToAction("Index", "Home");
        }

        [Route("Admin/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}