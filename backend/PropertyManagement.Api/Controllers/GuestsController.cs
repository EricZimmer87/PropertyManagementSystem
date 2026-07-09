using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Guests;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;
using PropertyManagement.Api.Services.Guests;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/guests")]
    [ApiController]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGuestService _guestService;

        public GuestsController(AppDbContext context, IGuestService guestService)
        {
            _context = context;
            _guestService = guestService;
        }

        // GET api/guests - gets all guests with pagination, sorting, and filtering
        [HttpGet]
        public async Task<ActionResult<PagedResponse<GuestResponse>>> GetGuests(
            [FromQuery] QueryFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await _guestService.GetAllGuestsAsync(filter, cancellationToken);
            return Ok(result);
        }

        // GET /api/guests/{id} - gets a guest by id
        [HttpGet("{id}")]
        public async Task<ActionResult<GuestResponse>> GetGuestById(long id)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest == null)
                return NotFound();

            var response = new GuestResponse
            {
                GuestId = guest.GuestId,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PhoneNumber = guest.PhoneNumber,
                Address = guest.Address,
                City = guest.City,
                State = guest.State,
                ZipCode = guest.ZipCode,
                Email = guest.Email,
                Company = guest.Company,
                Notes = guest.Notes
            };

            return Ok(response);
        }

        // POST /api/guests - creates a new guest
        [HttpPost]
        public async Task<ActionResult<GuestResponse>> CreateGuest([FromBody] CreateGuestRequest request)
        {
            // Can't create two guests with the same phone number
            if (request.PhoneNumber != null)
            {
                var normalizedPhoneNumber = PhoneNumberHelper.Normalize(request.PhoneNumber.Trim());

                bool cannotCreate = await _context.Guests
                    .AsNoTracking()
                    .AnyAsync(g => g.NormalizedPhoneNumber == normalizedPhoneNumber);

                if (cannotCreate)
                    return Conflict("A guest with that phone number already exists.");
            }

            var guest = new Guest
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                Address = request.Address?.Trim(),
                City = request.City?.Trim(),
                State = request.State?.Trim(),
                ZipCode = request.ZipCode?.Trim(),
                Email = request.Email?.Trim(),
                Company = request.Company?.Trim(),
                Notes = request.Notes?.Trim()
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            var response = await _context.Guests
                .AsNoTracking()
                .Where(g => g.GuestId == guest.GuestId)
                .Select(g => new GuestResponse
                {
                    GuestId = g.GuestId,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    PhoneNumber = g.PhoneNumber,
                    Address = g.Address,
                    City = g.City,
                    State = g.State,
                    ZipCode = g.ZipCode,
                    Email = g.Email,
                    Company = g.Company,
                    Notes = g.Notes
                })
                .SingleAsync();

            return CreatedAtAction(
                nameof(GetGuestById),
                new { id = guest.GuestId },
                response);
        }

        // PUT /api/guests/{id} - updates an existing guest
        [HttpPut("{id}")]
        public async Task<ActionResult<GuestResponse>> UpdateGuest(long id, [FromBody] UpdateGuestRequest request)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest == null)
                return NotFound();

            // Can't create two guests with the same phone number
            if (request.PhoneNumber != null)
            {
                var normalizedPhoneNumber = PhoneNumberHelper.Normalize(request.PhoneNumber.Trim());

                var exists = await _context.Guests
                    .AsNoTracking()
                    .AnyAsync(g => g.GuestId != guest.GuestId &&
                        g.NormalizedPhoneNumber == normalizedPhoneNumber);

                if (exists)
                    return Conflict("A guest with that phone number already exists.");
            }

            guest.FirstName = request.FirstName.Trim();
            guest.LastName = request.LastName.Trim();
            guest.PhoneNumber = request.PhoneNumber?.Trim();
            guest.Address = request.Address?.Trim();
            guest.City = request.City?.Trim();
            guest.State = request.State?.Trim();
            guest.ZipCode = request.ZipCode?.Trim();
            guest.Email = request.Email?.Trim();
            guest.Company = request.Company?.Trim();
            guest.Notes = request.Notes?.Trim();

            await _context.SaveChangesAsync();

            var response = new GuestResponse
            {
                GuestId = guest.GuestId,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PhoneNumber = guest.PhoneNumber,
                Address = guest.Address,
                City = guest.City,
                State = guest.State,
                ZipCode = guest.ZipCode,
                Email = guest.Email,
                Company = guest.Company,
                Notes = guest.Notes
            };

            return Ok(response);
        }

        // DELETE /api/guests/{id} - deletes a guest by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGuest(long id)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest == null) return NotFound();

            bool cannotDelete = await _context.Bookings
                .AsNoTracking()
                .AnyAsync(b => b.GuestId == id);

            if (cannotDelete)
                return Conflict("Cannot delete guest with existing bookings.");

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
