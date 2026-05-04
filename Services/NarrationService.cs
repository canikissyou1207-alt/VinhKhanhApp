using VinhKhanhApp.Models;
namespace VinhKhanhApp.Services;
public class NarrationService
{
    public async Task NarratePoiAsync(POI poi, string langCode)
    {
        string description = langCode switch
        {
            "VN" => poi.Description_VN,
            "EN" => poi.Description_EN,
            "JP" => poi.Description_JP,
            "KR" => poi.Description_KR,
            "FR" => poi.Description_FR,
            _ => poi.Description_VN
        };
        if (string.IsNullOrEmpty(description)) description = poi.Description_VN;

        var locales = await TextToSpeech.Default.GetLocalesAsync();

        var locale = langCode switch
        {
            "VN" => locales.FirstOrDefault(l => l.Language == "vi"),
            "EN" => locales.FirstOrDefault(l => l.Language == "en"),
            "JP" => locales.FirstOrDefault(l => l.Language == "ja"),
            "KR" => locales.FirstOrDefault(l => l.Language == "ko"),
            "FR" => locales.FirstOrDefault(l => l.Language == "fr"),
            _ => locales.FirstOrDefault(l => l.Language == "en")
        };

        // Fallback: nếu không có giọng đọc ngôn ngữ đó thì dùng giọng mặc định
        locale ??= locales.FirstOrDefault(l => l.Language == "en")
                ?? locales.FirstOrDefault();

        await TextToSpeech.Default.SpeakAsync(description, new SpeechOptions { Locale = locale });
    }
}