using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.Entities;

namespace Tendance.API.DataTransferObjects.Device
{
    public class CaptureDeviceForClientMinimal
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public CaptureDeviceType Type { get; set; }
        public DateTime Created { get; set; }
        public ClassroomForClientMinimal? Classroom { get; set; }
    }
}
