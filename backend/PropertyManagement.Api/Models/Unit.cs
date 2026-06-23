namespace PropertyManagement.Api.Models
{
    public class Unit
    {
        public long UnitId { get; set; }
        public required string UnitNumber { get; set; }
        public required string UnitType { get; set; }
        public string? Notes { get; set; }
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
