using Microsoft.Extensions.Logging;
using VinhKhanhApp.Models;

namespace VinhKhanhApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();
        //.UseMauiMaps()

        // Register services
        builder.Services.AddSingleton<Services.GeofenceService>();
        builder.Services.AddSingleton<Services.NarrationService>();

        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
        builder.Logging.AddDebug();

        // ĐOẠN CODE THẦN CHÚ: Cho phép gọi API localhost không cần chứng chỉ xịn
        builder.Services.AddSingleton(sp =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return new HttpClient(handler);
        });
#endif

        return builder.Build();
    }
}