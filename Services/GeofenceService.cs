using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VinhKhanhApp.Models;
using Microsoft.Maui.Devices.Sensors;

namespace VinhKhanhApp.Services
{
    public class GeofenceEventArgs : EventArgs
    {
        public POI Poi { get; set; }
        public double DistanceMeters { get; set; }
    }

    public class GeofenceService
    {
        // Khoảng thời gian giữa các lần kiểm tra vị trí (ms)
        public int PollingIntervalMs { get; set; } = 5000;

        // Sự kiện khi người dùng vào vùng POI
        public event EventHandler<GeofenceEventArgs> EnteredRegion;

        private bool _isRunning;

        public void Start(IEnumerable<POI> pois)
        {
            if (_isRunning) return;
            _isRunning = true;
            _ = MonitorLoop(pois);
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private async Task MonitorLoop(IEnumerable<POI> pois)
        {
            while (_isRunning)
            {
                try
                {
                    var location = await Geolocation.Default.GetLocationAsync();
                    if (location == null) { await Task.Delay(PollingIntervalMs); continue; }

                    foreach (var poi in pois)
                    {
                        double d = HaversineDistanceMeters(location.Latitude, location.Longitude, poi.Latitude, poi.Longitude);
                        if (d <= poi.Radius)
                        {
                            EnteredRegion?.Invoke(this, new GeofenceEventArgs { Poi = poi, DistanceMeters = d });
                        }
                    }
                }
                catch { }

                await Task.Delay(PollingIntervalMs);
            }
        }

        // Haversine (trả về mét)
        public static double HaversineDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // bán kính trái đất (m)
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
