using System.Security.Claims;
using Tendance.API.Entities;
using Tendance.API.Models;

namespace Tendance.API.Services
{
    public class UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        public Guid UserId
        {
            get
            {
                var text = httpContextAccessor.HttpContext!.User.FindFirstValue(TendanceClaim.UserId) ?? throw new ArgumentException();
                return Guid.Parse(text);
            }
        }

        public Guid SchoolId
        {
            get
            {
                var text = httpContextAccessor.HttpContext!.User.FindFirstValue(TendanceClaim.SchoolId) ?? throw new ArgumentException();
                return Guid.Parse(text);
            }
        }
        public Guid DeviceId
        {
            get
            {
                return Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(TendanceClaim.DeviceId) ?? throw new ArgumentException());
            }
        }

        public CaptureDeviceType DeviceType
        {
            get
            {
                return Enum.Parse<CaptureDeviceType>(httpContextAccessor.HttpContext!.User.FindFirstValue(TendanceClaim.DeviceType) ?? throw new ArgumentException());
            }
        }
    }
}