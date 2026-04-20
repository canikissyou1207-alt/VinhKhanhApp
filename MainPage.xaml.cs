using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using VinhKhanhApp.Models;
using VinhKhanhApp.Services;

namespace VinhKhanhApp;

public partial class MainPage : ContentPage
{
    private readonly NarrationService _narrationService = new();
    private readonly IDispatcherTimer _timer;
    private readonly HttpClient _httpClient;
    private List<POI> _poiList = new();
    private string _lastAskedPOI = string.Empty;
    private bool _isUsingMockLocation = true;
    private bool _isSpeaking;

    // Sử dụng cổng 7139 cho HTTPS để đồng bộ với trang Admin
    private static string ApiBaseUrl => DeviceInfo.Platform == DevicePlatform.Android
        ? "https://10.0.2.2:7139"
        : "https://localhost:7139";

    public MainPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(5);
        _timer.Tick += OnTimerTick;
        _ = CheckAndRequestLocationPermission();
    }

    // --- XỬ LÝ CHỌN NGÔN NGỮ BAN ĐẦU ---
    private void OnSelectVN(object sender, EventArgs e)
    {
        LanguagePicker.SelectedIndex = 0;
        StartApp();
    }

    private void OnSelectEN(object sender, EventArgs e)
    {
        LanguagePicker.SelectedIndex = 1;
        StartApp();
    }

    private async void StartApp()
    {
        LanguageOverlay.IsVisible = false;
        MainAppContent.IsVisible = true;
        ApplyLanguageText();
        await Task.Delay(400);
        LoadMap(new List<POI>());
        _timer.Start();
        await LoadPoiDataAsync(showSuccessMessage: false);
    }

    private bool IsVietnamese() => LanguagePicker.SelectedIndex == 0;

    private string GetCurrentLanguageCode()
    {
        return LanguagePicker.SelectedIndex switch
        {
            0 => "VN",
            1 => "EN",
            2 => "JP",
            3 => "KR",
            _ => "EN"
        };
    }

    private void ApplyLanguageText()
    {
        var isVN = IsVietnamese();
        LocationToggleBtn.Text = _isUsingMockLocation ? (isVN ? "📍 Giả lập" : "📍 Mock") : (isVN ? "🌍 Thực tế" : "🌍 Live");
        CounterBtn.Text = isVN ? "Nạp dữ liệu" : "Load data";
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (!MainAppContent.IsVisible) return;
        ApplyLanguageText();
        LoadMap(_poiList);
    }

    // --- CHỨC NĂNG TÌM KIẾM & BỘ LỌC ---
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";
        var filtered = _poiList.Where(p =>
            (p.Name?.ToLower().Contains(searchText) ?? false) ||
            (p.Description_VN?.ToLower().Contains(searchText) ?? false)
        ).ToList();
        LoadMap(filtered);
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        var category = btn.CommandParameter?.ToString() ?? "";
        var filtered = string.IsNullOrEmpty(category)
            ? _poiList
            : _poiList.Where(p => (p.Name?.Contains(category) ?? false) || (p.Description_VN?.Contains(category) ?? false)).ToList();
        LoadMap(filtered);
    }

    // --- ĐIỀU KHIỂN VỊ TRÍ & GOOGLE MAPS STYLE ---
    private async void OnToggleLocationClicked(object sender, EventArgs e)
    {
        _isUsingMockLocation = !_isUsingMockLocation;
        ApplyLanguageText();
        LocationToggleBtn.BackgroundColor = _isUsingMockLocation ? Color.FromArgb("#2196F3") : Color.FromArgb("#4CAF50");
        if (!_isUsingMockLocation) await UpdateCurrentLocation();
    }

    private async void OnRecenterClicked(object sender, EventArgs e)
    {
        await UpdateCurrentLocation();
    }

    private async void OnCounterClicked(object? sender, EventArgs e)
    {
        await LoadPoiDataAsync(showSuccessMessage: true);
    }

    private async Task LoadPoiDataAsync(bool showSuccessMessage)
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{ApiBaseUrl}/api/POIs");
            _poiList = JsonConvert.DeserializeObject<List<POI>>(response) ?? new List<POI>();
            LoadMap(_poiList);
            if (showSuccessMessage) await DisplayAlert(IsVietnamese() ? "Thông báo" : "Notice", IsVietnamese() ? "Đã nạp dữ liệu!" : "Data loaded!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(IsVietnamese() ? "Lỗi" : "Error", (IsVietnamese() ? "Không connection được API.\n" : "API Connection error.\n") + ex.Message, "OK");
        }
    }

    // --- HIỂN THỊ BẢN ĐỒ NHẤP NHÁY MƯỢT MÀ ---
    private void LoadMap(List<POI> pois)
    {
        bool isVN = IsVietnamese();
        var poisJson = JsonConvert.SerializeObject(pois);
        string txtRoute = isVN ? "CHỈ ĐƯỜNG" : "DIRECTIONS";
        string txtSpeak = isVN ? "NGHE" : "LISTEN";
        string txtDetail = isVN ? "CHI TIẾT" : "DETAILS";

        var sb = new StringBuilder();
        sb.AppendLine("<html><head><meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no'/>");
        sb.AppendLine("<link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/><script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>");
        sb.AppendLine("<style>html,body,#map{height:100%;margin:0;padding:0}.gm-popup{width:220px;font-family:sans-serif}.gm-img{width:100%;height:100px;object-fit:cover;border-radius:5px}.gm-title{font-weight:bold;margin-top:5px;display:block;color:#d32f2f}.gm-btn{width:100%;margin-top:5px;padding:8px;background:#2196F3;color:white;border:none;border-radius:4px;font-weight:bold;cursor:pointer}");
        sb.AppendLine(".pulse-icon { position: relative; }.dot { width: 14px; height: 14px; background: #4285F4; border: 2px solid white; border-radius: 50%; position: absolute; top: 3px; left: 3px; z-index: 2; box-shadow: 0 0 5px rgba(0,0,0,0.3); }.pulse { width: 40px; height: 40px; background: rgba(66, 133, 244, 0.4); border-radius: 50%; position: absolute; top: -10px; left: -10px; animation: pulsate 2s infinite ease-out; z-index: 1; }@keyframes pulsate { 0% { transform: scale(0.1); opacity: 1; } 100% { transform: scale(1.5); opacity: 0; } }</style></head>");
        sb.AppendLine("<body><div id='map'></div><script>");
        sb.AppendLine("var pois = " + poisJson + ";");
        sb.AppendLine("var map = L.map('map', {zoomControl:false}).setView([10.7580, 106.7020], 17);");
        sb.AppendLine("L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', { attribution: '&copy; OpenStreetMap' }).addTo(map); var userMarker;");
        sb.AppendLine("function updateUserLocation(lat, lng){ if(!userMarker){ var pulseIcon = L.divIcon({ className: 'pulse-icon', html: '<div class=\"dot\"></div><div class=\"pulse\"></div>', iconSize: [20, 20], iconAnchor: [10, 10] }); userMarker = L.marker([lat, lng], {icon: pulseIcon}).addTo(map); } else { userMarker.setLatLng([lat, lng]); } map.flyTo([lat, lng], map.getZoom(), { animate: true, duration: 1.5 }); }");
        sb.AppendLine("for(var i=0; i<pois.length; i++){ var p = pois[i]; var imgUrl = p.ImagePath || p.imagePath || ''; var html = '<div class=\"gm-popup\"><img class=\"gm-img\" src=\"'+imgUrl+'\" onerror=\"this.style.display=\\'none\\'\"/><span class=\"gm-title\">'+(p.Name || 'POI')+'</span>' + '<button class=\"gm-btn\" onclick=\"window.location.href=\\'route://\\'+p.Latitude+\\',\\'+p.Longitude\">" + txtRoute + "</button>' + '<button class=\"gm-btn\" style=\"background:#4CAF50\" onclick=\"window.location.href=\\'speak://\\'+encodeURIComponent(p.Name)\">" + txtSpeak + "</button>' + '<button class=\"gm-btn\" style=\"background:#FF9800\" onclick=\"window.location.href=\\'detail://\\'+encodeURIComponent(p.Name)\">" + txtDetail + "</button></div>'; L.marker([p.Latitude, p.Longitude]).addTo(map).bindPopup(html); }");
        sb.AppendLine("</script></body></html>");

        MapWebView.Source = new HtmlWebViewSource { Html = sb.ToString() };
    }

    private async void OnMapNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("route://", StringComparison.OrdinalIgnoreCase)) { e.Cancel = true; await Launcher.OpenAsync($"geo:0,0?q={e.Url.Replace("route://", string.Empty)}"); return; }
        if (e.Url.StartsWith("speak://", StringComparison.OrdinalIgnoreCase)) { e.Cancel = true; if (_isSpeaking) return; _isSpeaking = true; try { string name = Uri.UnescapeDataString(e.Url.Replace("speak://", string.Empty)); var poi = _poiList.FirstOrDefault(p => p.Name == name); if (poi != null) await _narrationService.NarratePoiAsync(poi, IsVietnamese()); } finally { _isSpeaking = false; } return; }
        if (e.Url.StartsWith("detail://", StringComparison.OrdinalIgnoreCase)) { e.Cancel = true; string name = Uri.UnescapeDataString(e.Url.Replace("detail://", string.Empty)); var poi = _poiList.FirstOrDefault(p => p.Name == name); if (poi != null) await Navigation.PushAsync(new DetailPage(poi, IsVietnamese())); }
    }

    private async void OnTimerTick(object? sender, EventArgs e) => await UpdateCurrentLocation();

    private async Task UpdateCurrentLocation()
    {
        try
        {
            Location? loc = _isUsingMockLocation ? new Location(10.75837, 106.70512) : await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10)));
            if (loc == null) return;
            await MapWebView.EvaluateJavaScriptAsync($"updateUserLocation({loc.Latitude.ToString(CultureInfo.InvariantCulture)}, {loc.Longitude.ToString(CultureInfo.InvariantCulture)})");
            _ = Task.Run(async () => {
                try
                {
                    var posData = new { DeviceId = DeviceInfo.Current.Name, Latitude = loc.Latitude, Longitude = loc.Longitude, DeviceModel = $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}", CurrentLanguage = GetCurrentLanguageCode() };
                    var content = new StringContent(JsonConvert.SerializeObject(posData), Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync($"{ApiBaseUrl}/api/UserPositions/update", content);
                }
                catch { }
            });
            foreach (var poi in _poiList.OrderBy(x => x.Priority))
            {
                double dist = loc.CalculateDistance(poi.Latitude, poi.Longitude, DistanceUnits.Kilometers) * 1000;
                if (dist < (poi.Radius > 0 ? poi.Radius : 25) && _lastAskedPOI != poi.Name)
                {
                    _lastAskedPOI = poi.Name ?? string.Empty;
                    if (await DisplayAlert(IsVietnamese() ? "Khám phá" : "Explore", IsVietnamese() ? $"Gần {poi.Name}. Nghe giới thiệu?" : $"Near {poi.Name}. Listen?", "OK", "Skip")) await _narrationService.NarratePoiAsync(poi, IsVietnamese());
                    break;
                }
            }
        }
        catch { }
    }

    private async Task CheckAndRequestLocationPermission()
    {
        if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted) await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
}