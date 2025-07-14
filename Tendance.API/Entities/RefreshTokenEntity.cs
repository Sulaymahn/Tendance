using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class RefreshTokenEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime Created { get; set; }
        public bool IsConsumed { get; set; }

        public UserEntity? User { get; set; }
    }
}
