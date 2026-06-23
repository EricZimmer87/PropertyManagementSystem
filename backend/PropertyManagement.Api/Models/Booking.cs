namespace PropertyManagement.Api.Models
{
    public class Booking
    {
        public long BookingId { get; set; }

        public long GuestId { get; set; }
        public Guest Guest { get; set; } = default!;

        public long UnitId { get; set; }
        public Unit Unit { get; set; } = default!;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? Occupants { get; set; }
        public string Status { get; set; } = "Booked";
        public string? Notes { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? CanceledOn { get; set; }

        public string? CreatedByUserId { get; set; }
        public AppUser? CreatedByUser { get; set; }

        public string? ModifiedByUserId { get; set; }
        public AppUser? ModifiedByUser { get; set; }

        public string? CanceledByUserId { get; set; }
        public AppUser? CanceledByUser { get; set; }
    }
}
