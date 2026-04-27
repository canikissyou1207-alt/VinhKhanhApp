using Microsoft.EntityFrameworkCore;

namespace VinhKhanhApi.Models
{
    public class VinhKhanhContext : DbContext
    {
        public VinhKhanhContext(DbContextOptions<VinhKhanhContext> options) : base(options)
        {
        }

        public DbSet<POI> POIs => Set<POI>();
        public DbSet<POITranslation> POI_Translations => Set<POITranslation>();

        // --- CODE MỚI: Thêm bảng lưu vị trí User ---
        public DbSet<UserPosition> UserPositions => Set<UserPosition>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng POIs và Translations
            modelBuilder.Entity<POI>().ToTable("POIs");
            modelBuilder.Entity<POITranslation>().ToTable("POI_Translations");

            // --- CODE MỚI: Cấu hình bảng UserPositions ---
            modelBuilder.Entity<UserPosition>().ToTable("UserPositions");
            // Thiết lập DeviceId là khóa chính (nếu trong Model chưa có [Key])
            modelBuilder.Entity<UserPosition>().HasKey(x => x.DeviceId);

            modelBuilder.Entity<POITranslation>()
                .HasIndex(x => new { x.POIID, x.LangCode })
                .IsUnique();

            modelBuilder.Entity<POI>()
                .HasMany(x => x.Translations)
                .WithOne(x => x.POI)
                .HasForeignKey(x => x.POIID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}