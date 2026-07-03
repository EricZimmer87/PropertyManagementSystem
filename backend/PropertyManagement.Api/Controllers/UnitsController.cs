using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Units;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/units")]
    [ApiController]
    [Authorize]
    public class UnitsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UnitsController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/units - gets all units
        [HttpGet]
        public async Task<ActionResult<List<UnitResponse>>> GetUnits(string? sort, string? dir)
        {
            bool descending = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);

            sort = sort?.ToLowerInvariant();

            var query = _context.Units
                .AsNoTracking();

            query = sort switch
            {
                "id" => descending
                ? query.OrderByDescending(u => u.UnitId)
                : query.OrderBy(u => u.UnitId),

                "number" => descending
                ? query.OrderByDescending(u => u.UnitNumber)
                : query.OrderBy(u => u.UnitNumber),

                "type" => descending
                ? query.OrderByDescending(u => u.UnitType)
                : query.OrderBy(u => u.UnitType),

                "notes" => descending
                ? query.OrderByDescending(u => u.Notes)
                : query.OrderBy(u => u.Notes),

                _ => query.OrderBy(u => u.UnitId)
            };

            var units = await query
                .Select(u => new UnitResponse
                {
                    UnitId = u.UnitId,
                    UnitNumber = u.UnitNumber,
                    UnitType = u.UnitType,
                    Notes = u.Notes ?? "-"
                })
                .ToListAsync();

            return Ok(units);
        }

        // GET api/units/{id} - gets a unit by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitResponse>> GetUnit(long id)
        {
            var unit = await _context.Units
                .AsNoTracking()
                .Where(u => u.UnitId == id)
                .Select(u => new UnitResponse
                {
                    UnitId = u.UnitId,
                    UnitNumber = u.UnitNumber,
                    UnitType = u.UnitType,
                    Notes = u.Notes ?? "-"
                })
                .FirstOrDefaultAsync();

            if (unit == null)
                return NotFound();

            return Ok(unit);
        }

        // POST api/units - creates a new unit
        [HttpPost]
        public async Task<ActionResult<UnitResponse>> CreateUnit(CreateUnitRequest unitRequest)
        {
            var unit = new Unit
            {
                UnitNumber = unitRequest.UnitNumber.Trim(),
                UnitType = unitRequest.UnitType.Trim(),
                Notes = unitRequest.Notes?.Trim()
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            var response = new UnitResponse
            {
                UnitId = unit.UnitId,
                UnitNumber = unit.UnitNumber,
                UnitType = unit.UnitType,
                Notes = unit.Notes ?? "-"
            };

            return CreatedAtAction(
                nameof(GetUnit),
                new { id = unit.UnitId },
                response);
        }

        // PUT api/units/{id} - updates an existing unit
        [HttpPut("{id}")]
        public async Task<ActionResult<UnitResponse>> UpdateUnit(long id, CreateUnitRequest unitRequest)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
                return NotFound();

            unit.UnitNumber = unitRequest.UnitNumber.Trim();
            unit.UnitType = unitRequest.UnitType.Trim();
            unit.Notes = unitRequest.Notes?.Trim();

            await _context.SaveChangesAsync();

            var response = new UnitResponse
            {
                UnitId = unit.UnitId,
                UnitNumber = unit.UnitNumber,
                UnitType = unit.UnitType,
                Notes = unit.Notes ?? "-"
            };

            return Ok(response);
        }

        // DELETE api/units/{id} - deletes a unit
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUnit(long id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
                return NotFound();

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}