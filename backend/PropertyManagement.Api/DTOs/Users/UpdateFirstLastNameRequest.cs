using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Users
{
    public class UpdateFirstLastNameRequest
    {
        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }
    }
}
