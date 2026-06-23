namespace PropertyManagement.Api.Models
{
    public class Guest
    {
        public long GuestId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }
        public string? Notes { get; set; }
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
