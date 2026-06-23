using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Api.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;

        public ICollection<Booking> CreatedBookings { get; set; } = [];
        public ICollection<Booking> ModifiedBookings { get; set; } = [];
        public ICollection<Booking> CanceledBookings { get; set; } = [];
    }
}
