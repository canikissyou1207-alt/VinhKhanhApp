using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.Models;

namespace VinhKhanhApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POITranslationsController : ControllerBase
    {
        private readonly VinhKhanhContext _context;

        public POITranslationsController(VinhKhanhContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<POITranslation>>> GetTranslations()
            => await _context.POI_Translations.OrderBy(x => x.POIID).ThenBy(x => x.LangCode).ToListAsync();

        [HttpGet("poi/{poiId:int}")]
        public async Task<ActionResult<IEnumerable<POITranslation>>> GetByPoi(int poiId)
            => await _context.POI_Translations.Where(x => x.POIID == poiId).OrderBy(x => x.LangCode).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<POITranslation>> Post(POITranslation translation)
        {
            _context.POI_Translations.Add(translation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByPoi), new { poiId = translation.POIID }, translation);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, POITranslation translation)
        {
            if (id != translation.TransID)
            {
                return BadRequest();
            }

            _context.Entry(translation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var translation = await _context.POI_Translations.FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            _context.POI_Translations.Remove(translation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
