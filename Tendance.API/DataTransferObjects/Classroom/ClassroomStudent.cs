namespace Tendance.API.DataTransferObjects.Classroom
{
    public class ClassroomStudent
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
    }
}
