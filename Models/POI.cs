using Newtonsoft.Json;

namespace VinhKhanhApp.Models
{
    public class POI
    {
        [JsonProperty("POIID")]
        public int POIID { get; set; }

        [JsonProperty("CategoryID")]
        public int CategoryID { get; set; }

        [JsonProperty("Latitude")]
        public double Latitude { get; set; }

        [JsonProperty("Longitude")]
        public double Longitude { get; set; }

        [JsonProperty("Radius")]
        public double Radius { get; set; }

        [JsonProperty("Priority")]
        public int Priority { get; set; }

        [JsonProperty("ImagePath")]
        public string? ImagePath { get; set; }

        [JsonProperty("AudioUrl")]
        public string? AudioUrl { get; set; }

        [JsonProperty("Name")]
        public string? Name { get; set; }

        [JsonProperty("Description_VN")]
        public string? Description_VN { get; set; }

        [JsonProperty("Description_EN")]
        public string? Description_EN { get; set; }

        [JsonIgnore]
        public string? imagePath => ImagePath;
    }
}
