namespace PropertyManagement.Api.DTOs.AllowedEmails
{
    public class AllowedEmailResponse
    {
        public long AllowedEmailId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
