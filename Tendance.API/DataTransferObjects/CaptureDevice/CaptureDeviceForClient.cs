using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.Entities;

namespace Tendance.API.DataTransferObjects.CaptureDevice
{
    public class CaptureDeviceForClient
    {
        public int Id { get; set; }
        public int? ClassroomId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string ClientKey { get; set; } = string.Empty;
        public CaptureDeviceType Type { get; set; }
        public CaptureDeviceMode Mode { get; set; }
        public DateTime Created { get; set; }
    }
}