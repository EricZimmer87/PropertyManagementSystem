using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Api.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Booking> CreatedBookings { get; set; } = [];
        public ICollection<Booking> ModifiedBookings { get; set; } = [];
        public ICollection<Booking> CanceledBookings { get; set; } = [];
    }
}
