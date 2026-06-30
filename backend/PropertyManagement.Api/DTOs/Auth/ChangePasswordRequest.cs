using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required]
        public required string OldPassword { get; init; }

        [Required]
        public required string NewPassword { get; init; }
    }
}
