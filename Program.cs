using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Data;
using VinhKhanhApi.Models;
using VinhKhanhApi.Services;

var cultureInfo = new System.Globalization.CultureInfo("en-US");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VinhKhanhContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MediaStorageService>();

// Thêm Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VinhKhanhContext>();
    await DbInitializer.InitializeAsync(context);

    var adminDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await adminDb.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserPositions' AND xtype='U')
        CREATE TABLE [dbo].[UserPositions] (
            [DeviceId]        NVARCHAR (450) NOT NULL,
            [Latitude]        FLOAT          NOT NULL,
            [Longitude]       FLOAT          NOT NULL,
            [DeviceModel]     NVARCHAR (MAX) NULL,
            [CurrentLanguage] NVARCHAR (MAX) NULL,
            [LastUpdate]      DATETIME2 (7)  NOT NULL,
            CONSTRAINT [PK_UserPositions] PRIMARY KEY ([DeviceId])
        )
    ");

    await adminDb.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AdminUsers' AND xtype='U')
        CREATE TABLE [dbo].[AdminUsers] (
            [Id]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [Username]     NVARCHAR(100)     NOT NULL,
            [PasswordHash] NVARCHAR(256)     NOT NULL
        )
    ");

    // Tạo tài khoản admin mặc định nếu chưa có (password: admin123)
    var hasAdmin = await adminDb.AdminUsers.AnyAsync();
    if (!hasAdmin)
    {
        adminDb.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
        });
        await adminDb.SaveChangesAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();