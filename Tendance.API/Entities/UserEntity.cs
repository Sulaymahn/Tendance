namespace Tendance.API.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public SchoolEntity? School { get; set; }
        public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
    }
}
