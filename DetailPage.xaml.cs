using VinhKhanhApp.Models;
using VinhKhanhApp.Services;

namespace VinhKhanhApp;

public partial class DetailPage : ContentPage
{
    private readonly POI _poi;
    private readonly bool _isVN;
    private readonly NarrationService _narrationService = new();

    public DetailPage(POI poi, bool isVN)
    {
        InitializeComponent();
        _poi = poi;
        _isVN = isVN;

        Title = poi.Name;
        LblName.Text = poi.Name;
        LblDesc.Text = isVN ? poi.Description_VN : poi.Description_EN;
        BtnSpeakDetail.Text = isVN ? "🔊 NGHE GIỚI THIỆU CHI TIẾT" : "🔊 LISTEN TO NARRATION";

        ImgPoi.Source = !string.IsNullOrWhiteSpace(poi.ImagePath)
            ? poi.ImagePath
            : "https://placehold.co/600x400?text=No+Image";
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSpeakDetailClicked(object sender, EventArgs e)
    {
        await _narrationService.NarratePoiAsync(_poi, _isVN);
    }
}
