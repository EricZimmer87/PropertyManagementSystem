namespace PropertyManagement.Api.DTOs.Auth
{
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
