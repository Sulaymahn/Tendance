using System.Security.Claims;
using Tendance.API.Entities;

namespace Tendance.API.Services
{
    public class DeviceContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        public Guid DeviceId
        {
            get
            {
                return Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue("DeviceId") ?? throw new ArgumentException());
            }
        }

        public Guid SchoolId
        {
            get
            {
                return Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue("SchoolId") ?? throw new ArgumentException());
            }
        }

        public CaptureDeviceType DeviceType
        {
            get
            {
                return Enum.Parse<CaptureDeviceType>(httpContextAccessor.HttpContext!.User.FindFirstValue("DeviceType") ?? throw new ArgumentException());
            }
        }
    }
}
