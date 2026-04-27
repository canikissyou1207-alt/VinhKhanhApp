using Microsoft.AspNetCore.Hosting;

namespace VinhKhanhApi.Services
{
    public class MediaStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public MediaStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> SaveAsync(IFormFile? file, string subFolder)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var root = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(root))
            {
                root = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var safeFolder = subFolder.Replace('\\', '/').Trim('/');
            var folder = Path.Combine(root, safeFolder);
            Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream);

            return $"/{safeFolder}/{fileName}";
        }

        public void DeleteIfManaged(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var root = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(root))
            {
                root = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var trimmed = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(root, trimmed);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
