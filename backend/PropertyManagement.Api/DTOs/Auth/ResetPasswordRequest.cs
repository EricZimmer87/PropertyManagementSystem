using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        public required string Token { get; init; }

        [Required]
        public required string NewPassword { get; init; }
    }
}
