using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.DTOs.Bookings
{
    public class BookingResponse
    {
        public long BookingId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? Occupants { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Booked;
        public string? Notes { get; set; }
        public int? CardLastFour { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? CanceledOn { get; set; }

        public string? CreatedByUserName { get; set; } = string.Empty;
        public string? ModifiedByUserName { get; set; }
        public string? CanceledByUserName { get; set; }
    }
}
