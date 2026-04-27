using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace VinhKhanhApi.Controllers
{
    public class AdminController : Controller
    {
        // Giả sử bạn có một danh sách static để lưu trữ tạm thời người dùng báo cáo về
        // Trong thực tế, bạn sẽ lấy dữ liệu này từ Database (SQL Server/MySQL)
        public static List<UserPosition> ActiveUsersList = new List<UserPosition>();

        [Route("Admin/ActiveUsers")]
        public IActionResult ActiveUsers()
        {
            // Truyền danh sách người dùng sang View
            return View(ActiveUsersList);
        }

        // Action nhận dữ liệu từ điện thoại gửi về (API)
        [HttpPost]
        [Route("api/report")]
        public IActionResult ReportPosition([FromBody] UserPosition data)
        {
            if (data == null) return BadRequest();

            // Cập nhật nếu đã tồn tại, hoặc thêm mới nếu chưa có
            var existing = ActiveUsersList.FirstOrDefault(u => u.DeviceId == data.DeviceId);
            if (existing != null)
            {
                existing.Latitude = data.Latitude;
                existing.Longitude = data.Longitude;
                existing.CurrentLanguage = data.CurrentLanguage;
                existing.DeviceModel = data.DeviceModel;
                existing.LastUpdate = System.DateTime.Now;
            }
            else
            {
                data.LastUpdate = System.DateTime.Now;
                ActiveUsersList.Add(data);
            }

            return Ok(new { status = "success" });
        }
    }

    public class UserPosition
    {
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CurrentLanguage { get; set; }
        public System.DateTime LastUpdate { get; set; }
    }
}