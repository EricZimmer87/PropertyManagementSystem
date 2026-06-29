using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.AppUsers
{
    public class IsActiveRequest
    {
        [Required]
        public required bool IsActive { get; init; }
    }
}
