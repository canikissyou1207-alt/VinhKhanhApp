using Newtonsoft.Json;

namespace VinhKhanhApp.Models
{
    public class POI
    {
        [JsonProperty("POIID")] public int POIID { get; set; }
        [JsonProperty("CategoryID")] public int CategoryID { get; set; }
        [JsonProperty("Latitude")] public double Latitude { get; set; }
        [JsonProperty("Longitude")] public double Longitude { get; set; }
        [JsonProperty("Radius")] public double Radius { get; set; }
        [JsonProperty("Priority")] public int Priority { get; set; }
        [JsonProperty("ImagePath")] public string? ImagePath { get; set; }
        [JsonProperty("AudioUrl")] public string? AudioUrl { get; set; }
        [JsonProperty("Name")] public string? Name { get; set; }

        // --- THÔNG TIN CHI TIẾT ---
        [JsonProperty("Address")] public string? Address { get; set; }
        [JsonProperty("Rating")] public double Rating { get; set; }

        // --- MÔ TẢ ĐA NGÔN NGỮ ---
        [JsonProperty("Description_VN")] public string? Description_VN { get; set; }
        [JsonProperty("Description_EN")] public string? Description_EN { get; set; }
        [JsonProperty("Description_JP")] public string? Description_JP { get; set; }
        [JsonProperty("Description_KR")] public string? Description_KR { get; set; }
        [JsonProperty("Description_FR")] public string? Description_FR { get; set; }

        [JsonIgnore] public string? imagePath => ImagePath;
    }
}