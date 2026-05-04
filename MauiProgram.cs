using Microsoft.Extensions.Logging;
using VinhKhanhApp.Models;
using VinhKhanhApp.Services;

namespace VinhKhanhApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Đăng ký services
        builder.Services.AddSingleton<GeofenceService>();
        builder.Services.AddSingleton<NarrationService>();

#if DEBUG
        // Bypass SSL cho localhost khi debug
        builder.Services.AddHttpClient<DeviceService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => true
                };
            });
#else
        // Production: dùng SSL thật
        builder.Services.AddHttpClient<DeviceService>();
#endif

        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}