using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.Services.Bookings;
using QueryFilter = PropertyManagement.Api.Common.QueryFilter;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;

        public BookingsController(AppDbContext context,
            IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        // GET /api/bookings - gets all bookings
        [HttpGet]
        public async Task<ActionResult<PagedResponse<BookingResponse>>> GetBookings(
            [FromQuery] QueryFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await _bookingService.GetAllBookingsAsync(filter, cancellationToken);

            return Ok(result);
        }

        // GET /api/bookings/{id} - gets a booking by id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponse>> GetBookingById(long id)
        {
            var booking = _context.Bookings
                .Where(b => b.BookingId == id);

            var response = await booking
                .Select(BookingProjections.ToBookingResponse)
                .SingleOrDefaultAsync();

            if (response == null) return NotFound();

            return Ok(response);
        }
    }
}
