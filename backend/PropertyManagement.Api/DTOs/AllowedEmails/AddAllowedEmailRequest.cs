using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.AllowedEmails
{
    public class AddAllowedEmailRequest
    {
        [Required]
        public required string Email { get; init; }
    }
}
