using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Users
{
    public class UpdateEmailRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
