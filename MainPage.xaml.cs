using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Net.Http;
using VinhKhanhApp.Models;
using VinhKhanhApp.Services;

namespace VinhKhanhApp;

public partial class MainPage : ContentPage
{
    private readonly NarrationService _narrationService = new();
    private readonly IDispatcherTimer _timer;
    private List<POI> _poiList = new();
    private string _lastAskedPOI = string.Empty;
    private bool _isMapLoaded = false;
    private bool _isUsingMockLocation = false;

    public MainPage()
    {
        InitializeComponent();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(5);
        _timer.Tick += (s, e) => _ = UpdateCurrentLocation(false);
        _ = CheckAndRequestLocationPermission();
    }

    private void OnSelectVN(object sender, EventArgs e) { LanguagePicker.SelectedIndex = 0; StartApp(); }
    private void OnSelectEN(object sender, EventArgs e) { LanguagePicker.SelectedIndex = 1; StartApp(); }
    private void OnSelectJP(object sender, EventArgs e) { LanguagePicker.SelectedIndex = 2; StartApp(); }
    private void OnSelectKR(object sender, EventArgs e) { LanguagePicker.SelectedIndex = 3; StartApp(); }
    private void OnSelectFR(object sender, EventArgs e) { LanguagePicker.SelectedIndex = 4; StartApp(); }

    private async void StartApp()
    {
        LanguageOverlay.IsVisible = false;
        MainAppContent.IsVisible = true;
        _poiList = await LoadPoisFromApiAsync();
        ApplyLanguageText();
        _timer.Start();
        await Task.Delay(2000);
        _isMapLoaded = true;
        await UpdateCurrentLocation(true);
    }

    private async Task<List<POI>> LoadPoisFromApiAsync()
    {
        try
        {
            using var client = new HttpClient();
#if ANDROID
            var url = "http://192.168.1.21:5174/api/POIs";
#else
            var url = "http://localhost:5174/api/POIs";
#endif
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<POI>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<POI>>(json) ?? new List<POI>();
        }
        catch
        {
            return new List<POI>();
        }
    }

    private static string GetOrCreateDeviceId()
    {
        const string key = "device_id";
        if (Preferences.Default.ContainsKey(key))
            return Preferences.Default.Get(key, "");
        var newId = Guid.NewGuid().ToString();
        Preferences.Default.Set(key, newId);
        return newId;
    }

    private string GetCurrentLanguageCode() => LanguagePicker.SelectedIndex switch { 0 => "VN", 1 => "EN", 2 => "JP", 3 => "KR", 4 => "FR", _ => "VN" };

    private void ApplyLanguageText()
    {
        string lang = GetCurrentLanguageCode();
        LocationToggleBtn.Text = _isUsingMockLocation
            ? (lang switch { "VN" => "📍 Giả lập", "EN" => "📍 Mock", "JP" => "📍 模擬", "KR" => "📍 모의", "FR" => "📍 Simulé", _ => "📍 Mock" })
            : (lang switch { "VN" => "🌍 Vị trí thật", "EN" => "🌍 GPS", "JP" => "🌍 Thực tế", "KR" => "🌍 thực tế", "FR" => "🌍 Réel", _ => "🌍 GPS" });
        LocationToggleBtn.BackgroundColor = _isUsingMockLocation ? Color.FromArgb("#2196F3") : Color.FromArgb("#4CAF50");
        CounterBtn.Text = lang switch { "VN" => "Nạp dữ liệu", "EN" => "Reload", "JP" => "更新", "KR" => "갱신", "FR" => "Charger", _ => "Reload" };
        LoadMap(_poiList);
    }

    private void LoadMap(List<POI> pois)
    {
        string lang = GetCurrentLanguageCode();
        string detailText = lang switch { "VN" => "Chi tiết", "EN" => "Detail", "JP" => "詳細", "KR" => "상세보기", "FR" => "Détails", _ => "Detail" };
        var json = JsonConvert.SerializeObject(pois);
        json = json.Replace("https://localhost:7139", "http://192.168.1.21:5174");
        json = json.Replace("http://localhost:5174", "http://192.168.1.21:5174");

        var sb = new StringBuilder();
        sb.AppendLine("<html><head><meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no'/>");
        sb.AppendLine("<link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/><script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>");
        sb.AppendLine("<style>html,body,#map{height:100%;margin:0;padding:0}.p{width:180px;font-family:sans-serif}.img{width:100%;height:95px;object-fit:cover;border-radius:10px}.n{font-size:14px;font-weight:bold;margin:5px 0 2px 0;display:block}.s{color:#FFC107;font-size:11px;font-weight:bold}.a{color:#666;font-size:10px;margin-bottom:8px;display:block}.btn{width:100%;background:#2196F3;color:white;border:none;padding:8px;border-radius:6px;font-weight:bold;cursor:pointer}</style></head>");
        sb.AppendLine("<body><div id='map'></div><script>");
        sb.AppendLine("var map=L.map('map',{zoomControl:false}).setView([10.7765, 106.6853],17);");
        sb.AppendLine("L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png').addTo(map); var uM;");
        sb.AppendLine("function updateUserLocation(lat,lng,f){ if(!uM){ uM=L.marker([lat,lng],{icon:L.divIcon({className:'u',html:'<div style=\"width:14px;height:14px;background:#4285F4;border:2px solid white;border-radius:50%\"></div>',iconSize:[14,14]})}).addTo(map); }else{ uM.setLatLng([lat,lng]); } if(f) map.flyTo([lat,lng],17); }");
        sb.AppendLine("var ps = " + json + ";");
        sb.AppendLine("for(var i=0; i<ps.length; i++){");
        sb.AppendLine("  var p = ps[i];");
        sb.AppendLine("  var encodedName = encodeURIComponent(p.Name);");
        sb.AppendLine("  var h = '<div class=\"p\">' + ");
        sb.AppendLine("    '<img class=\"img\" src=\"' + p.ImagePath + '\"/>' + ");
        sb.AppendLine("    '<b class=\"n\">' + p.Name + '</b>' + ");
        sb.AppendLine("    '<span class=\"s\">⭐ ' + (p.Rating||0) + ' / 5.0</span>' + ");
        sb.AppendLine("    '<span class=\"a\">📍 ' + (p.Address||'') + '</span>' + ");
        sb.AppendLine("    '<button class=\"btn\" onclick=\"window.location.href=\\'detail://poi?name=\\' + encodedName\">' + '" + detailText + "' + '</button></div>';");
        sb.AppendLine("  L.marker([p.Latitude, p.Longitude],{icon:L.icon({iconUrl:'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',shadowUrl:'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',iconSize:[25,41],iconAnchor:[12,41]})}).addTo(map).bindPopup(h);");
        sb.AppendLine("}");
        sb.AppendLine("</script></body></html>");
        MapWebView.Source = new HtmlWebViewSource { Html = sb.ToString() };
    }

    private async Task UpdateCurrentLocation(bool forceFly)
    {
        if (!_isMapLoaded) return;
        try
        {
            Location? loc = _isUsingMockLocation ? new Location(10.7765, 106.6853) : await Geolocation.Default.GetLocationAsync();
            if (loc == null) return;
            string latStr = loc.Latitude.ToString(CultureInfo.InvariantCulture);
            string lngStr = loc.Longitude.ToString(CultureInfo.InvariantCulture);
            string flyFlag = forceFly ? "true" : "false";
            await MapWebView.EvaluateJavaScriptAsync($"updateUserLocation({latStr}, {lngStr}, {flyFlag})");

            var nearest = _poiList.Select(p => new { P = p, D = loc.CalculateDistance(p.Latitude, p.Longitude, DistanceUnits.Kilometers) * 1000 }).Where(x => x.D < 500).OrderBy(x => x.D).FirstOrDefault();
            if (nearest != null && _lastAskedPOI != nearest.P.Name)
            {
                _lastAskedPOI = nearest.P.Name;
                string q = GetCurrentLanguageCode() switch { "VN" => $"Bạn gần {nearest.P.Name}. Nghe giới thiệu?", "EN" => $"Near {nearest.P.Name}. Listen?", _ => "Guide?" };
                if (await DisplayAlert("Food Guide", q, "OK", "Cancel")) await _narrationService.NarratePoiAsync(nearest.P, GetCurrentLanguageCode());
            }

            try
            {
                using var client = new HttpClient();
                var userData = new
                {
                    deviceId = GetOrCreateDeviceId(),
                    deviceModel = DeviceInfo.Model,
                    latitude = loc.Latitude,
                    longitude = loc.Longitude,
                    currentLanguage = GetCurrentLanguageCode()
                };
                var jsonContent = JsonConvert.SerializeObject(userData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
#if ANDROID
                await client.PostAsync("http://192.168.1.21:5174/api/report", content);
#else
                await client.PostAsync("http://localhost:5174/api/report", content);
#endif
            }
            catch { }
        }
        catch { }
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        _poiList = await LoadPoisFromApiAsync();
        ApplyLanguageText();
    }

    private async void OnToggleLocationClicked(object s, EventArgs e) { _isUsingMockLocation = !_isUsingMockLocation; ApplyLanguageText(); await UpdateCurrentLocation(true); }
    private async void OnRecenterClicked(object s, EventArgs e) => await UpdateCurrentLocation(true);
    private void OnSearchTextChanged(object s, TextChangedEventArgs e) { var t = e.NewTextValue?.ToLower() ?? ""; LoadMap(_poiList.Where(p => (p.Name?.ToLower().Contains(t) ?? false)).ToList()); }
    private async void OnMapNavigating(object s, WebNavigatingEventArgs e)
    {
        if (e.Url.Contains("detail://"))
        {
            e.Cancel = true;
            try
            {
                var uri = new Uri(e.Url.Replace("detail://", "http://fake/"));
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string n = query["name"] ?? "";
                n = Uri.UnescapeDataString(n);
                var p = _poiList.FirstOrDefault(x => x.Name == n);
                if (p != null)
                    await Navigation.PushAsync(new DetailPage(p, GetCurrentLanguageCode()));
                else
                    await DisplayAlert("Debug", $"Tên: '{n}' | Số POI: {_poiList.Count}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", ex.Message, "OK");
            }
        }
    }
    private async Task CheckAndRequestLocationPermission()
    {
        if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
}