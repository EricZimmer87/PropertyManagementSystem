using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.DTOs.Units;
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

            /* Covnert this to LINQ Query
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

            var bookings = from u in _context.Units
                           join b in _context.Bookings
                            on b.UnitId equals u.UnitId

            //// Get all units
            //var units = await _context.Units
            //    .AsNoTracking()
            //    .OrderBy(u => u.UnitNumber)
            //    .Select(u => new UnitResponse
            //    {
            //        UnitId = u.UnitId,
            //        UnitNumber = u.UnitNumber,
            //        UnitType = u.UnitType,
            //        Notes = u.Notes ?? ""
            //    })
            //    .ToListAsync();

                           //// Get all bookings
                           //var bookings = await _context.Bookings
                           //    .AsNoTracking()
                           //    .Where(b => b.StartDate <= day && b.EndDate > day)
                           //    .Select(BookingProjections.ToBookingResponse)
                           //    .ToListAsync();

                           //// Group bookings by unit into a dictionary - unitNumber:booking
                           //var byUnit = bookings
                           //    .GroupBy(b => b.UnitNumber)
                           //    .ToDictionary(b => b.Key, b => b.ToList());

            return Ok(byUnit);
        }
    }
}
