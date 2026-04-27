using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Models;
using VinhKhanhApi.Services;
using VinhKhanhApi.ViewModels;

namespace VinhKhanhApi.Controllers
{
    public class AdminPoisController : Controller
    {
        private readonly VinhKhanhContext _context;
        private readonly MediaStorageService _mediaStorageService;

        public AdminPoisController(VinhKhanhContext context, MediaStorageService mediaStorageService)
        {
            _context = context;
            _mediaStorageService = mediaStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.POIs
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new PoiAdminEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoiAdminEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var poi = new POI
            {
                CategoryID = vm.CategoryID,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                Radius = vm.Radius,
                Priority = vm.Priority,
                Name = vm.Name,
                Description_VN = vm.Description_VN,
                Description_EN = vm.Description_EN,
                ImagePath = await _mediaStorageService.SaveAsync(vm.ImageFile, "uploads/images"),
                AudioUrl = await _mediaStorageService.SaveAsync(vm.AudioFile, "uploads/audio")
            };

            _context.POIs.Add(poi);
            await _context.SaveChangesAsync();
            await UpsertTranslationsAsync(poi);
            TempData["Success"] = "Đã tạo POI mới.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var poi = await _context.POIs.FindAsync(id);
            if (poi == null)
            {
                return NotFound();
            }

            return View(new PoiAdminEditViewModel
            {
                POIID = poi.POIID,
                CategoryID = poi.CategoryID,
                Latitude = poi.Latitude,
                Longitude = poi.Longitude,
                Radius = poi.Radius,
                Priority = poi.Priority,
                Name = poi.Name,
                Description_VN = poi.Description_VN,
                Description_EN = poi.Description_EN,
                ExistingImagePath = poi.ImagePath,
                ExistingAudioUrl = poi.AudioUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PoiAdminEditViewModel vm)
        {
            if (id != vm.POIID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var poi = await _context.POIs.FindAsync(id);
            if (poi == null)
            {
                return NotFound();
            }

            poi.CategoryID = vm.CategoryID;
            poi.Latitude = vm.Latitude;
            poi.Longitude = vm.Longitude;
            poi.Radius = vm.Radius;
            poi.Priority = vm.Priority;
            poi.Name = vm.Name;
            poi.Description_VN = vm.Description_VN;
            poi.Description_EN = vm.Description_EN;

            if (vm.ImageFile != null)
            {
                _mediaStorageService.DeleteIfManaged(poi.ImagePath);
                poi.ImagePath = await _mediaStorageService.SaveAsync(vm.ImageFile, "uploads/images");
            }

            if (vm.AudioFile != null)
            {
                _mediaStorageService.DeleteIfManaged(poi.AudioUrl);
                poi.AudioUrl = await _mediaStorageService.SaveAsync(vm.AudioFile, "uploads/audio");
            }

            await _context.SaveChangesAsync();
            await UpsertTranslationsAsync(poi);
            TempData["Success"] = "Đã cập nhật POI.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var poi = await _context.POIs.FindAsync(id);
            if (poi == null)
            {
                return NotFound();
            }

            return View(poi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poi = await _context.POIs
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.POIID == id);

            if (poi == null)
            {
                return NotFound();
            }

            _mediaStorageService.DeleteIfManaged(poi.ImagePath);
            _mediaStorageService.DeleteIfManaged(poi.AudioUrl);
            _context.POIs.Remove(poi);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa POI.";
            return RedirectToAction(nameof(Index));
        }

        private async Task UpsertTranslationsAsync(POI poi)
        {
            await UpsertTranslationAsync(poi.POIID, "vi", poi.Name, poi.Description_VN, poi.AudioUrl);
            await UpsertTranslationAsync(poi.POIID, "en", poi.Name, poi.Description_EN, poi.AudioUrl);
            await _context.SaveChangesAsync();
        }

        private async Task UpsertTranslationAsync(int poiId, string langCode, string? title, string? description, string? audioPath)
        {
            var item = await _context.POI_Translations.FirstOrDefaultAsync(x => x.POIID == poiId && x.LangCode == langCode);
            if (item == null)
            {
                _context.POI_Translations.Add(new POITranslation
                {
                    POIID = poiId,
                    LangCode = langCode,
                    Title = title,
                    Description = description,
                    AudioPath = audioPath
                });
                return;
            }

            item.Title = title;
            item.Description = description;
            item.AudioPath = audioPath;
        }
    }
}
