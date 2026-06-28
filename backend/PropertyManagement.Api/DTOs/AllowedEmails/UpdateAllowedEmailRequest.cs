using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.AllowedEmails
{
    public class UpdateAllowedEmailRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
