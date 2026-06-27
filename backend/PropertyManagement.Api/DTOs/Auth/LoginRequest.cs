using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
