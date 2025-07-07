namespace Tendance.API.DataTransferObjects.Teacher
{
    public class TeacherForCreation
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Middlename { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}