namespace Tendance.API.DataTransferObjects.Course
{
    public class CourseForClient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
    }
}
