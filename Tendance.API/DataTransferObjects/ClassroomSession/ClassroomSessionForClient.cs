using Tendance.API.DataTransferObjects.Classroom;

namespace Tendance.API.DataTransferObjects.ClassroomSession
{
    public class ClassroomSessionForClient
    {
        public int Id { get; set; }
        public string? Topic { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime CheckInFrom { get; set; }
        public DateTime CheckInTo { get; set; }
        public DateTime CheckOutFrom { get; set; }
        public DateTime CheckOutTo { get; set; }
        public string? Note { get; set; }
        public DateTime Created { get; set; }
        public required ClassroomForClient Classroom { get; set; }
        public List<ClassroomSessionAttendance> Attendances { get; set; } = [];
    }
}
