using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Models;

namespace VinhKhanhApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserPosition> UserPositions { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
    }
}