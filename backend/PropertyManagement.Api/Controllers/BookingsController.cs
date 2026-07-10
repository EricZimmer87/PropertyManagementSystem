using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services.Bookings;
using QueryFilter = PropertyManagement.Api.Common.QueryFilter;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;

        public BookingsController(
            AppDbContext context,
            IBookingService bookingService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _bookingService = bookingService;
            _userManager = userManager;
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

        // POST /api/bookings - creates a new booking
        [HttpPost]
        public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            // StartDate must be less than EndDate
            // The dates can be equal in case the business wants to charge hourly rates
            if (request.StartDate > request.EndDate)
                return Conflict("End date must be greater than start date.");

            var booking = new Booking
            {
                GuestId = request.GuestId,
                UnitId = request.UnitId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Occupants = request.Occupants,
                Status = request.Status,
                Notes = request.Notes,
                CardLastFour = request.CardLastFour,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = user.Id
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var response = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.BookingId == booking.BookingId)
                .Select(BookingProjections.ToBookingResponse)
                .SingleAsync();

            return Ok(response);
        }
    }
}
