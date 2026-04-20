using VinhKhanhApp.Models;

namespace VinhKhanhApp.Services
{
    public class NarrationService
    {
        public async Task NarratePoiAsync(POI poi, bool isVietnamese)
        {
            if (poi == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(poi.AudioUrl))
            {
                try
                {
                    await Launcher.Default.OpenAsync(new Uri(poi.AudioUrl));
                    return;
                }
                catch
                {
                    // fallback xuống TTS
                }
            }

            var text = isVietnamese ? poi.Description_VN : poi.Description_EN;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = poi.Description_VN ?? poi.Description_EN ?? poi.Name;
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    await TextToSpeech.Default.SpeakAsync(text);
                }
                catch
                {
                }
            }
        }
    }
}
