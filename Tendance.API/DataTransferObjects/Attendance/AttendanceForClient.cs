using Tendance.API.Entities;

namespace Tendance.API.DataTransferObjects.Attendance
{
    public class AttendanceForClient
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public AttendanceType Type { get; set; }
        public AttendanceRole Role { get; set; }
        public required AttendanceClassroom Classroom { get; set; }
    }
}
