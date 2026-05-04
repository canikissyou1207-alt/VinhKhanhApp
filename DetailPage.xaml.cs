using VinhKhanhApp.Models;
using VinhKhanhApp.Services;

namespace VinhKhanhApp;

public partial class DetailPage : ContentPage
{
    private readonly NarrationService _narrationService = new();
    private POI _currentPoi;
    private string _currentLang;

    public DetailPage(POI poi, string langCode)
    {
        InitializeComponent();

        // Lưu lại dữ liệu quán và ngôn ngữ
        _currentPoi = poi;
        _currentLang = langCode;

        // 1. Tự động cập nhật Tên quán (Lấy đúng tên quán bạn đã nhấn trên bản đồ)
        LblName.Text = poi.Name;

        // 2. Tự động cập nhật Ảnh
        ImgPoi.Source = poi.ImagePath;

        // 3. Hiển thị bài thuyết minh theo ngôn ngữ
        // Nếu ngôn ngữ đó bị trống (null), mặc định sẽ hiện tiếng Việt
        string description = langCode switch
        {
            "VN" => poi.Description_VN,
            "EN" => poi.Description_EN,
            "JP" => poi.Description_JP,
            "KR" => poi.Description_KR,
            "FR" => poi.Description_FR,
            _ => poi.Description_VN
        };

        // Kiểm tra: nếu bài thuyết minh bị trắng thì lấy tiếng Việt bù vào cho đẹp
        LblDesc.Text = string.IsNullOrEmpty(description) ? poi.Description_VN : description;

        // 4. Cập nhật chữ trên nút bấm và tiêu đề trang theo ngôn ngữ
        UpdateUIByLanguage(langCode);
    }

    private void UpdateUIByLanguage(string lang)
    {
        switch (lang)
        {
            case "VN":
                Title = "Chi tiết quán";
                BtnSpeakDetail.Text = "🔊 Nghe thuyết minh";
                break;
            case "EN":
                Title = "Details";
                BtnSpeakDetail.Text = "🔊 Listen to Guide";
                break;
            case "JP":
                Title = "詳細情報";
                BtnSpeakDetail.Text = "🔊 ガイドを聞く";
                break;
            case "KR":
                Title = "상세 정보";
                BtnSpeakDetail.Text = "🔊 가이드 듣기";
                break;
            case "FR":
                Title = "Détails";
                BtnSpeakDetail.Text = "🔊 Écouter le guide";
                break;
        }
    }

    private async void OnNarrateClicked(object sender, EventArgs e)
    {
        if (_currentPoi != null)
        {
            // Gọi giọng đọc thuyết minh
            await _narrationService.NarratePoiAsync(_currentPoi, _currentLang);
        }
    }
}