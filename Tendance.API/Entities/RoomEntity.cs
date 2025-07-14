namespace Tendance.API.Entities
{
    public class RoomEntity
    {
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public required string Name { get; set; }
        public string? Building { get; set; }

        public SchoolEntity? School { get; set; }
        public List<ClassroomEntity> Classrooms { get; set; } = [];
    }
}
