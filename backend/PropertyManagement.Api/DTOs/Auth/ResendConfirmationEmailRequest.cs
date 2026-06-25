using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class ResendConfirmationEmailRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
