using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Users
{
    public class UpdateUserRoleRequest
    {
        [Required]
        public required string Role { get; init; }
    }
}
