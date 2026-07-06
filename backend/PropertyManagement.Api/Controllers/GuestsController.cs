using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Guests;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.Services.Guests;
using System.Text.RegularExpressions;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/guests")]
    [ApiController]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

        // Helper to normalize phone number
        public static string NormalizePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            return Regex.Replace(phoneNumber, "[^0-9]", "");
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<GuestResponse>>> GetGuests(
            QueryFilter filter,
            IGuestService service,
            CancellationToken cancellationToken)
        {
            var result = await service.GetAllGuestsAsync(filter, cancellationToken);
            return Ok(result);
        }

        // GET /api/guests - gets all guests
        //[HttpGet]
        //public async Task<ActionResult<PagedResponse<GuestResponse>>> GetGuests(
        //    string? sort,
        //    string? dir,
        //    int pageNumber = 1,
        //    int pageSize = 10,
        //    string? search = null)
        //{
        //    // pageNumber and pageSize must be greater than 0
        //    if (pageNumber <= 0)
        //        return BadRequest($"{nameof(pageNumber)} must be greater than 0");
        //    if (pageSize <= 0)
        //        return BadRequest($"{nameof(pageSize)} must be greater than 0");

        //    pageSize = Math.Clamp(pageSize, 1, 100); // Limit pageSize to a maximum of 100

        //    bool descending = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);
        //    sort = sort?.Trim().ToLowerInvariant();

        //    // Start query
        //    var query = _context.Guests.AsNoTracking();

        //    // Search
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        search = search.Trim();
        //        var like = $"%{search}%";
        //        var normalizedSearch = NormalizePhoneNumber(search);
        //        var normalizedLike = $"%{normalizedSearch}%";

        //        query = query.Where(g =>
        //            EF.Functions.Like(g.FirstName, like) ||
        //            EF.Functions.Like(g.LastName, like) ||
        //            EF.Functions.Like(g.Address, like) ||
        //            EF.Functions.Like(g.City, like) ||
        //            EF.Functions.Like(g.State, like) ||
        //            EF.Functions.Like(g.ZipCode, like) ||
        //            EF.Functions.Like(g.Email, like) ||
        //            EF.Functions.Like(g.Notes, like) || // remove for performance??
        //            EF.Functions.Like(g.Company, like) ||
        //            (!string.IsNullOrEmpty(normalizedSearch) &&
        //            EF.Functions.Like(g.NormalizedPhoneNumber ?? "", normalizedLike))
        //        );
        //    }

        //    // Sort
        //    query = sort switch
        //    {
        //        "id" => descending
        //        ? query.OrderByDescending(g => g.GuestId)
        //        : query.OrderBy(g => g.GuestId),

        //        "firstname" => descending
        //        ? query.OrderByDescending(g => g.FirstName)
        //        : query.OrderBy(g => g.FirstName),

        //        "lastname" => descending
        //        ? query.OrderByDescending(g => g.LastName)
        //        : query.OrderBy(g => g.LastName),

        //        "phonenumber" => descending
        //        ? query.OrderByDescending(g => g.NormalizedPhoneNumber)
        //        : query.OrderBy(g => g.NormalizedPhoneNumber),

        //        "address" => descending
        //        ? query.OrderByDescending(g => g.Address)
        //        : query.OrderBy(g => g.Address),

        //        "city" => descending
        //        ? query.OrderByDescending(g => g.City)
        //        : query.OrderBy(g => g.City),

        //        "state" => descending
        //        ? query.OrderByDescending(g => g.State)
        //        : query.OrderBy(g => g.State),

        //        "zipcode" => descending
        //        ? query.OrderByDescending(g => g.ZipCode)
        //        : query.OrderBy(g => g.ZipCode),

        //        "email" => descending
        //        ? query.OrderByDescending(g => g.Email)
        //        : query.OrderBy(g => g.Email),

        //        "company" => descending
        //        ? query.OrderByDescending(g => g.Company)
        //        : query.OrderBy(g => g.Company),

        //        "notes" => descending
        //        ? query.OrderByDescending(g => g.Notes)
        //        : query.OrderBy(g => g.Notes),

        //        _ => descending
        //        ? query.OrderByDescending(g => g.GuestId)
        //        : query.OrderBy(g => g.GuestId)
        //    };

        //    // Offset pagination
        //    var totalGuests = await query.CountAsync();
        //    int skip = (pageNumber - 1) * pageSize;

        //    var guestResponses = await query
        //        .Skip(skip)
        //        .Take(pageSize)
        //        .Select(g => new GuestResponse
        //        {
        //            GuestId = g.GuestId,
        //            FirstName = g.FirstName,
        //            LastName = g.LastName,
        //            PhoneNumber = g.PhoneNumber,
        //            NormalizedPhoneNumber = g.NormalizedPhoneNumber,
        //            Address = g.Address,
        //            City = g.City,
        //            State = g.State,
        //            ZipCode = g.ZipCode,
        //            Email = g.Email,
        //            Company = g.Company,
        //            Notes = g.Notes
        //        })
        //        .ToListAsync();

        //    var totalPages = (int)Math.Ceiling(totalGuests / (double)pageSize);

        //    var response = new PagedResponse<GuestResponse>
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalCount = totalGuests,
        //        TotalPages = totalPages,
        //        HasNextPage = pageNumber < totalPages,
        //        HasPreviousPage = pageNumber > 1,
        //        Items = guestResponses
        //    };

        //    return Ok(response);
        //}
    }
}
