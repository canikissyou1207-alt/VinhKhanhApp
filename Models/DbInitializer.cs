using Microsoft.EntityFrameworkCore;

namespace VinhKhanhApi.Models
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(VinhKhanhContext context)
        {
            await context.Database.EnsureCreatedAsync();

            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.POIs', 'AudioUrl') IS NULL
    ALTER TABLE dbo.POIs ADD AudioUrl NVARCHAR(500) NULL;
");

            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID(N'dbo.POI_Translations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.POI_Translations
    (
        TransID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        POIID INT NOT NULL,
        LangCode NVARCHAR(10) NULL,
        Title NVARCHAR(200) NULL,
        Description NVARCHAR(MAX) NULL,
        AudioPath NVARCHAR(500) NULL,
        CONSTRAINT FK_POI_Translations_POIs FOREIGN KEY (POIID) REFERENCES dbo.POIs(POIID) ON DELETE CASCADE
    );
END
");

            if (await context.POIs.AnyAsync())
            {
                return;
            }

            var seedPois = new List<POI>
            {
                new()
                {
                    CategoryID = 1,
                    Latitude = 10.758370,
                    Longitude = 106.705120,
                    Radius = 30,
                    Priority = 1,
                    ImagePath = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?auto=format&fit=crop&w=1200&q=80",
                    Name = "Quán Nướng Vĩnh Khánh",
                    Description_VN = "Đây là quán nướng nổi bật trên phố ẩm thực Vĩnh Khánh, phù hợp để trải nghiệm không khí nhộn nhịp về đêm và các món nướng hải sản đặc trưng.",
                    Description_EN = "This grilled seafood stop is a popular place on Vinh Khanh food street, ideal for experiencing the lively nightlife and signature grilled dishes."
                },
                new()
                {
                    CategoryID = 1,
                    Latitude = 10.758520,
                    Longitude = 106.705410,
                    Radius = 30,
                    Priority = 2,
                    ImagePath = "https://images.unsplash.com/photo-1515003197210-e0cd71810b5f?auto=format&fit=crop&w=1200&q=80",
                    Name = "Ốc Đêm Vĩnh Khánh",
                    Description_VN = "Quán chuyên các món ốc và hải sản chế biến đậm vị. Đây là kiểu quán rất đặc trưng của khu phố ẩm thực Quận 4.",
                    Description_EN = "This restaurant specializes in snails and flavorful seafood dishes, representing the iconic late-night style of District 4."
                },
                new()
                {
                    CategoryID = 2,
                    Latitude = 10.758090,
                    Longitude = 106.704860,
                    Radius = 20,
                    Priority = 3,
                    ImagePath = "https://images.unsplash.com/photo-1559847844-5315695dadae?auto=format&fit=crop&w=1200&q=80",
                    Name = "Bánh Tráng Nướng Góc Phố",
                    Description_VN = "Điểm dừng chân cho món ăn vặt quen thuộc với lớp bánh giòn, topping đa dạng và hương vị gần gũi với giới trẻ.",
                    Description_EN = "A casual street-food stop for grilled rice paper with crispy texture, assorted toppings, and flavors popular with young locals."
                }
            };

            context.POIs.AddRange(seedPois);
            await context.SaveChangesAsync();

            var translations = seedPois.SelectMany(p => new[]
            {
                new POITranslation
                {
                    POIID = p.POIID,
                    LangCode = "vi",
                    Title = p.Name,
                    Description = p.Description_VN,
                    AudioPath = p.AudioUrl
                },
                new POITranslation
                {
                    POIID = p.POIID,
                    LangCode = "en",
                    Title = p.Name,
                    Description = p.Description_EN,
                    AudioPath = p.AudioUrl
                }
            }).ToList();

            context.POI_Translations.AddRange(translations);
            await context.SaveChangesAsync();
        }
    }
}
