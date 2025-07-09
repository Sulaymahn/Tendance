namespace Tendance.API.DataTransferObjects.Auth
{
    public class SignupCredential
    {
        public string SchoolEmail { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
