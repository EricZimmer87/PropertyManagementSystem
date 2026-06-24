namespace PropertyManagement.Api.Models
{
    public class AllowedEmail
    {
        public long AllowedEmailId { get; set; }
        public required string Email { get; set; }
        public string NormalizedEmail { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; }
    }
}
