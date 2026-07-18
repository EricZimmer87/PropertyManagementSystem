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
                .AsNoTracking()
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


        // PUT /api/bookings/{id} - updates a booking
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponse>> UpdateBooking(long id, UpdateBookingRequest request)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var now = DateTime.UtcNow;

            booking.GuestId = request.GuestId;
            booking.UnitId = request.UnitId;
            booking.StartDate = request.StartDate;
            booking.EndDate = request.EndDate;
            booking.Occupants = request.Occupants;
            booking.Status = request.Status;
            booking.Notes = request.Notes;
            booking.CardLastFour = request.CardLastFour;

            booking.ModifiedByUserId = user.Id;
            booking.ModifiedOn = now;

            if (booking.Status == BookingStatus.Canceled)
            {
                booking.CanceledOn = now;
                booking.CanceledByUserId = user.Id;
            }

            await _context.SaveChangesAsync();

            var response = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.BookingId == booking.BookingId)
                .Select(BookingProjections.ToBookingResponse)
                .SingleAsync();

            return Ok(response);
        }


        // DELETE /api/bookings/{id} - deletes a booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET /api/bookings/by-day
        [HttpGet("by-day")]
        public async Task<ActionResult> GetBookingsByDay(DateOnly? selectedDay)
        {
            var day = selectedDay ?? DateOnly.FromDateTime(DateTime.Today);

            /* Convert this to LINQ Query
             SELECT u.UnitNumber,
	            b.StartDate,
	            b.EndDate,
	            g.FirstName
            FROM Units AS u
            LEFT JOIN Bookings AS b ON b.UnitId = u.UnitId
	            AND b.StartDate <= CAST(GETDATE() AS DATE)
	            AND b.EndDate > CAST(GETDATE() AS DATE)
            LEFT JOIN Guests AS g ON g.GuestId = b.GuestId
            ORDER BY u.UnitNumber;
             */

            var query = await (from u in _context.Units
                               join b in _context.Bookings
                                   on u.UnitId equals b.UnitId
                                   into bookings
                               from b in bookings
                               .Where(b =>
                                   b.StartDate <= day &&
                                   b.EndDate > day)
                                   .DefaultIfEmpty()
                               join g in _context.Guests
                                   on b.GuestId equals g.GuestId
                                   into guests
                               from g in guests.DefaultIfEmpty()
                               orderby u.UnitNumber
                               select new BookingsByDayResponse
                               {
                                    CreatedOn = b == null ? default : b.CreatedOn,
                                    CreatedByUserName =
                                        b == null || b.CreatedByUser == null
                                            ? string.Empty
                                            : (string.IsNullOrEmpty(b.CreatedByUser.FirstName) ||
                                            string.IsNullOrEmpty(b.CreatedByUser.LastName))
                                                ? b.CreatedByUser.Email
                                                : b.CreatedByUser.FirstName + " " + b.CreatedByUser.LastName,

                                    UnitNumber = u.UnitNumber,

                                    GuestName = g == null ? "" : g.FirstName + " " + g.LastName,
                                    Address = g == null ? "" : (g.Address ?? ""),
                                    City = g == null ? "" : (g.City ?? ""),
                                    State = g == null ? "" : (g.State ?? ""),
                                    ZipCode = g == null ? "" : (g.ZipCode ?? ""),
                                    PhoneNumber = g == null ? "" : (g.PhoneNumber ?? ""),

                                    StartDate = b == null ? default : b.StartDate,
                                    EndDate = b == null ? default : b.EndDate,

                                    Occupants = b == null ? default : b.Occupants,
                                    Status = b == null ? default : b.Status,
                                    Notes = b == null ? default : b.Notes,
                                    CardLastFour = b == null ? default : b.CardLastFour
                               })
                               .ToListAsync();

            return Ok(query);
        }
    }
}
