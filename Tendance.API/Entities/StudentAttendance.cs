namespace Tendance.API.Entities
{
    public class StudentAttendance : Attendance
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
