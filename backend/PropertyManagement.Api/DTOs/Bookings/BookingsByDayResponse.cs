using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.DTOs.Bookings
{
    public class BookingsByDayResponse
    {
        public DateTime CreatedOn { get; set; }
        public string? CreatedByUserName { get; set; } = string.Empty;

        public string UnitNumber { get; set; } = string.Empty;

        public string GuestName { get; set; } = string.Empty;
        public string? Address {get; set; }
        public string? City {get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public int? Occupants { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Booked;
        public string? Notes { get; set; }
        public int? CardLastFour { get; set; }
    }
}