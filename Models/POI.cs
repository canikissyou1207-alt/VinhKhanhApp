using System.ComponentModel.DataAnnotations;

namespace VinhKhanhApi.Models
{
    public class POI
    {
        [Key]
        public int POIID { get; set; }

        public int CategoryID { get; set; } = 1;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public double Radius { get; set; } = 25;

        public int Priority { get; set; } = 1;

        [StringLength(500)]
        public string? ImagePath { get; set; }

        [StringLength(500)]
        public string? AudioUrl { get; set; }

        [Required, StringLength(200)]
        public string? Name { get; set; }

        public string? Description_VN { get; set; }

        public string? Description_EN { get; set; }

        public ICollection<POITranslation> Translations { get; set; } = new List<POITranslation>();
    }
}
