using System.ComponentModel.DataAnnotations;

namespace VinhKhanhApi.ViewModels
{
    public class PoiAdminEditViewModel
    {
        public int? POIID { get; set; }

        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; } = 1;

        [Required, Display(Name = "Vĩ độ")]
        public double Latitude { get; set; }

        [Required, Display(Name = "Kinh độ")]
        public double Longitude { get; set; }

        [Display(Name = "Bán kính kích hoạt (m)")]
        public double Radius { get; set; } = 25;

        [Display(Name = "Độ ưu tiên")]
        public int Priority { get; set; } = 1;

        [Required, StringLength(200), Display(Name = "Tên điểm / quán")]
        public string? Name { get; set; }

        [Display(Name = "Mô tả tiếng Việt")]
        public string? Description_VN { get; set; }

        [Display(Name = "Mô tả tiếng Anh")]
        public string? Description_EN { get; set; }

        [Display(Name = "Ảnh hiện tại")]
        public string? ExistingImagePath { get; set; }

        [Display(Name = "Audio hiện tại")]
        public string? ExistingAudioUrl { get; set; }

        [Display(Name = "Upload ảnh")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Upload audio")]
        public IFormFile? AudioFile { get; set; }
    }
}
