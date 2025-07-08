namespace Tendance.API.Entities
{
    public class TeacherAttendance : Attendance
    {
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
