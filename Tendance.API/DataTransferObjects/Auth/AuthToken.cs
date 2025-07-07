namespace Tendance.API.DataTransferObjects.Auth
{
    public class AuthToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
