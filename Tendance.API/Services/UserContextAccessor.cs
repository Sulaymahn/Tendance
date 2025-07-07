using System.Security.Claims;

namespace Tendance.API.Services
{
    public class UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        public Guid UserId
        {
            get
            {
                var text = httpContextAccessor.HttpContext!.User.FindFirstValue("UserId") ?? throw new ArgumentException();
                return Guid.Parse(text);
            }
        }

        public Guid SchoolId
        {
            get
            {
                var text = httpContextAccessor.HttpContext!.User.FindFirstValue("SchoolId") ?? throw new ArgumentException();
                return Guid.Parse(text);
            }
        }
    }
}