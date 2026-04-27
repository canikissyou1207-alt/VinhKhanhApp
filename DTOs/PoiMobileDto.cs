namespace VinhKhanhApi.DTOs
{
    public class PoiMobileDto
    {
        public int POIID { get; set; }
        public int CategoryID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public int Priority { get; set; }
        public string? ImagePath { get; set; }
        public string? AudioUrl { get; set; }
        public string? Name { get; set; }
        public string? Description_VN { get; set; }
        public string? Description_EN { get; set; }
    }
}
