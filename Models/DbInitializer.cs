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
        }
    }
}