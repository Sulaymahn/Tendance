using Tendance.API.Entities;

namespace Tendance.API.Models
{
    public struct CaptureResult
    {
        public AttendanceRole? Role { get; set; }
        public object? Person { get; set; }
        public bool Success { get; set; }
    }
}
