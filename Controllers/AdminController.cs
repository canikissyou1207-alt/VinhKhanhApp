using VinhKhanhApi.Data;
using VinhKhanhApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Models;

namespace VinhKhanhApi.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [AdminAuthFilter]
        [Route("Admin/ActiveUsers")]
        public async Task<IActionResult> ActiveUsers()
        {
            var cutoff = DateTime.Now.AddMinutes(-2);
            var users = await _db.UserPositions
                .Where(u => u.LastUpdate >= cutoff)
                .OrderByDescending(u => u.LastUpdate)
                .ToListAsync();
            return View(users);
        }

        [AdminAuthFilter]
        [Route("Admin/ActiveUsers/Data")]
        public async Task<IActionResult> ActiveUsersData()
        {
            var cutoff = DateTime.Now.AddMinutes(-2);
            var count = await _db.UserPositions
                .CountAsync(u => u.LastUpdate >= cutoff);
            return Json(new { count });
        }

        [HttpPost]
        [Route("api/report")]
        public async Task<IActionResult> ReportPosition([FromBody] UserPosition data)
        {
            if (data == null) return BadRequest();

            var existing = await _db.UserPositions
                .FirstOrDefaultAsync(u => u.DeviceId == data.DeviceId);

            if (existing != null)
            {
                existing.Latitude = data.Latitude;
                existing.Longitude = data.Longitude;
                existing.CurrentLanguage = data.CurrentLanguage;
                existing.DeviceModel = data.DeviceModel;
                existing.LastUpdate = DateTime.Now;
            }
            else
            {
                data.LastUpdate = DateTime.Now;
                _db.UserPositions.Add(data);
            }

            await _db.SaveChangesAsync();
            return Ok(new { status = "success" });
        }
    }
}