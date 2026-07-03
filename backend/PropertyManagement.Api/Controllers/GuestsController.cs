using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/guests")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/guests - gets all guests
        [HttpGet]
        public async Task<ActionResult<List<Guest>>> GetGuests()
        {
            var guests = await _context.Guests.ToListAsync();

            return Ok(guests);
        }
    }
}
