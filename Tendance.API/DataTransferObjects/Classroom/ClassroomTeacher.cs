namespace Tendance.API.DataTransferObjects.Classroom
{
    public class ClassroomTeacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
    }
}
