using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinhKhanhApi.Models
{
    public class POITranslation
    {
        [Key]
        public int TransID { get; set; }

        [Required]
        public int POIID { get; set; }

        [StringLength(10)]
        public string? LangCode { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? AudioPath { get; set; }

        [ForeignKey(nameof(POIID))]
        public POI? POI { get; set; }
    }
}
