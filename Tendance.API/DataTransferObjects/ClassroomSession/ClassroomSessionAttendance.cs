using Tendance.API.Entities;

namespace Tendance.API.DataTransferObjects.ClassroomSession
{
    public class ClassroomSessionAttendance
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public AttendanceRole Role { get; set; }
        public bool CheckedIn { get; set; }
        public DateTime? CheckInTimeStamp { get; set; }
        public bool CheckedOut { get; set; }
        public DateTime? CheckOutTimeStamp { get; set; }
    }
}
