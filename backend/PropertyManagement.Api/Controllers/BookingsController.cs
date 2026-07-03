using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/bookings - gets all bookings
        [HttpGet]
        public async Task<ActionResult<List<Booking>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .AsNoTracking()
                .ToListAsync();

            return Ok(bookings);
        }
    }
}
