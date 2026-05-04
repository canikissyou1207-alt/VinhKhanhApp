using System.Net.Http.Json;
using Microsoft.Maui.Devices;

namespace VinhKhanhApp.Services
{
    public class DeviceService
    {
        private readonly HttpClient _http;

#if ANDROID
    private const string ApiUrl = "http://192.168.1.21:5174/api/report";
#else
        private const string ApiUrl = "http://localhost:5174/api/report";
#endif

        public DeviceService(HttpClient http)
        {
            _http = http;
        }

        public async Task ReportDeviceAsync(string langCode)
        {
            try
            {
                var payload = new
                {
                    DeviceId = GetOrCreateDeviceId(),
                    DeviceModel = DeviceInfo.Current.Model,
                    CurrentLanguage = langCode,
                    Latitude = 0.0,
                    Longitude = 0.0
                };

                var response = await _http.PostAsJsonAsync(ApiUrl, payload);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("✅ Gửi thiết bị thành công");
                else
                    Console.WriteLine($"❌ Lỗi API: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Không kết nối được API: {ex.Message}");
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
    }
}