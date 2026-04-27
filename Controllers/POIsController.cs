using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhApi.DTOs;
using VinhKhanhApi.Models;

namespace VinhKhanhApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POIsController : ControllerBase
    {
        private readonly VinhKhanhContext _context;

        public POIsController(VinhKhanhContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PoiMobileDto>>> GetPOIs()
        {
            var items = await _context.POIs
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return items.Select(MapDto).ToList();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PoiMobileDto>> GetPOI(int id)
        {
            var poi = await _context.POIs.FindAsync(id);
            if (poi == null)
            {
                return NotFound();
            }

            return MapDto(poi);
        }

        [HttpPost]
        public async Task<ActionResult<POI>> PostPOI(POI poi)
        {
            _context.POIs.Add(poi);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPOI), new { id = poi.POIID }, poi);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPOI(int id, POI poi)
        {
            if (id != poi.POIID)
            {
                return BadRequest();
            }

            _context.Entry(poi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.POIs.AnyAsync(e => e.POIID == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePOI(int id)
        {
            var poi = await _context.POIs.FindAsync(id);
            if (poi == null)
            {
                return NotFound();
            }

            _context.POIs.Remove(poi);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private PoiMobileDto MapDto(POI poi)
        {
            return new PoiMobileDto
            {
                POIID = poi.POIID,
                CategoryID = poi.CategoryID,
                Latitude = poi.Latitude,
                Longitude = poi.Longitude,
                Radius = poi.Radius,
                Priority = poi.Priority,
                ImagePath = ToAbsoluteUrl(poi.ImagePath),
                AudioUrl = ToAbsoluteUrl(poi.AudioUrl),
                Name = poi.Name,
                Description_VN = poi.Description_VN,
                Description_EN = poi.Description_EN
            };
        }

        private string? ToAbsoluteUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if (Uri.TryCreate(path, UriKind.Absolute, out _))
            {
                return path;
            }

            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}
