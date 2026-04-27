using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Models;
using VinhKhanhApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DỊCH VỤ (SERVICES) ---
builder.Services.AddDbContext<VinhKhanhContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MediaStorageService>();

// Cấu hình CORS để trang Admin (React) có thể truy cập API
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. XÂY DỰNG ỨNG DỤNG (BUILD) ---
var app = builder.Build();

// --- 3. KHỞI TẠO DỮ LIỆU (DATABASE INITIALIZER) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VinhKhanhContext>();
    await DbInitializer.InitializeAsync(context);
}

// --- 4. CẤU HÌNH LUỒNG XỬ LÝ (MIDDLEWARE) ---

// Swagger nên bật ở môi trường Development
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

// QUAN TRỌNG: Thứ tự này phải chính xác để không bị lỗi "Network Error"
app.UseRouting();      // Bước 1: Xác định đường dẫn

app.UseCors();         // Bước 2: Cho phép quyền truy cập (Phải nằm sau Routing)

app.UseAuthorization(); // Bước 3: Kiểm tra quyền (Phải nằm sau Cors)

// Định tuyến cho MVC Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Định tuyến cho API Controller
app.MapControllers();

app.Run();