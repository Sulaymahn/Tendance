namespace Tendance.API.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public required string Name { get; set; }
        public string? Building { get; set; }

        public School? School { get; set; }
        public List<Classroom> Classrooms { get; set; } = [];
    }
}
