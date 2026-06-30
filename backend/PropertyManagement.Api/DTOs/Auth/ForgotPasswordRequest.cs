using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
