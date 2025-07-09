namespace Tendance.API.Entities
{
    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Joined { get; set; }

        public List<Room> Rooms { get; set; } = [];
        public List<User> Users { get; set; } = [];
        public List<Course> Courses { get; set; } = [];
        public List<Student> Students { get; set; } = [];
        public List<Teacher> Teachers { get; set; } = [];
        public List<Classroom> Classrooms { get; set; } = [];
        public List<ClassroomSession> ClassroomSessions { get; set; } = [];
        public List<CaptureDevice> CaptureDevice { get; set; } = [];
    }
}
