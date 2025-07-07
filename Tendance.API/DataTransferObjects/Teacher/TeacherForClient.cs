namespace Tendance.API.DataTransferObjects.Teacher
{
    public class TeacherForClient
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public float AttendanceRate { get; set; }
        public DateTime Created { get; set; }
    }
}
