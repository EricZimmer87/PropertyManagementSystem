using PropertyManagement.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        [Required]
        public required long GuestId { get; init; }

        [Required]
        public required long UnitId { get; init; }

        [Required]
        public required DateOnly StartDate { get; init; }

        [Required]
        public required DateOnly EndDate { get; init; }

        public int? Occupants { get; set; }

        public BookingStatus Status { get; init; }

        public string? Notes { get; init; }
        public int? CardLastFour { get; init; }
    }
}
