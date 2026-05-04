using VinhKhanhApp.Services; // ← thêm dòng này
namespace VinhKhanhApp;

public partial class App : Application
{
    private readonly DeviceService _deviceService;

    public App(DeviceService deviceService)
    {
        InitializeComponent();
        _deviceService = deviceService;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        // Gửi thông tin thiết bị lên admin ngay khi app mở
        // Thay "VN" bằng ngôn ngữ người dùng đang chọn thực tế
        await _deviceService.ReportDeviceAsync("VN");
    }
}