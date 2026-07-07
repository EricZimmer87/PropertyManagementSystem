using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Guests
{
    public class CreateGuestRequest
    {
        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }

        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? State { get; init; }
        public string? ZipCode { get; init; }
        public string? Email { get; init; }
        public string? Company { get; init; }
        public string? Notes { get; init; }
    }
}
