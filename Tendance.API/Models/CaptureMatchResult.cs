using Tendance.API.Entities;

namespace Tendance.API.Models
{
    public struct CaptureMatchResult
    {
        public int? MatchId { get; set; }
        public AttendanceRole? Role { get; set; }
        public bool Success { get; set; }
        public CaptureError Error { get; set; }
        public string Message { get; set; }
    }
}
