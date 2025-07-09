using Tendance.API.Entities;

namespace Tendance.API.DataTransferObjects.CaptureDevice
{
    public class CaptureDeviceForCreation
    {
        public string Nickname { get; set; } = string.Empty;
        public int? ClassroomId { get; set; }
        public CaptureDeviceType Type { get; set; }
    }
}
