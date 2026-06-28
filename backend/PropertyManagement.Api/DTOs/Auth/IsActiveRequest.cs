using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class IsActiveRequest
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required bool IsActive { get; init; }
    }
}
