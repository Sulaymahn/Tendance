namespace Tendance.API.DataTransferObjects.Classroom
{
    public class ClassroomRoom
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Building { get; set; }
    }
}
