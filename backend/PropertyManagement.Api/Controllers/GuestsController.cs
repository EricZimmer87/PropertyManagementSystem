using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Guests;
using PropertyManagement.Api.DTOs.Shared;
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

        [HttpGet]
        public async Task<ActionResult<PagedResponse<GuestResponse>>> GetGuests(
            [FromQuery] QueryFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await _guestService.GetAllGuestsAsync(filter, cancellationToken);
            return Ok(result);
        }
    }
}
