using System.ComponentModel.DataAnnotations;

namespace VinhKhanhApi.Models
{
    public class UserPosition
    {
        [Key]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public string? DeviceModel { get; set; }

        public string? CurrentLanguage { get; set; }

        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
}