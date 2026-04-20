using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VinhKhanhApp.Models;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using System.Linq;

namespace VinhKhanhApp.ViewModels
{
    public class MapViewModel : BindableObject
    {
        public ObservableCollection<POI> Pois { get; } = new ObservableCollection<POI>();

        public ObservableCollection<POI> NearbyPois { get; } = new ObservableCollection<POI>();

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; } public ICommand DeleteCommand { get; }
        public MapViewModel()
        {
            LoadCommand = new Command(async () => await LoadAsync());
            // Optionally, you can auto-load nearby POIs on construction
            // _ = LoadNearbyPoisAsync();
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                using var http = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, er) => true });
                var json = await http.GetStringAsync("https://localhost:7139/api/POIs");
                var list = JsonConvert.DeserializeObject<List<POI>>(json);
                Pois.Clear();
                foreach (var p in list) Pois.Add(p);
                // Optionally, update nearby POIs after loading
                await LoadNearbyPoisAsync();
            }
            catch { }
            finally { IsBusy = false; }
        }   

        public async Task LoadNearbyPoisAsync(double radiusMeters = 1000)
        {
            try
            {
                var location = await Geolocation.Default.GetLocationAsync();
                if (location == null) return;
                var nearby = Pois.Where(p => HaversineDistanceMeters(location.Latitude, location.Longitude, p.Latitude, p.Longitude) <= radiusMeters).ToList();
                NearbyPois.Clear();
                foreach (var p in nearby) NearbyPois.Add(p);
            }
            catch { }
        }

        // Haversine formula to calculate distance between two lat/lon points in meters
        private double HaversineDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
