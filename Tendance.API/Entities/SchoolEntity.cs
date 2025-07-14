namespace Tendance.API.Entities
{
    public class SchoolEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Joined { get; set; }

        public List<RoomEntity> Rooms { get; set; } = [];
        public List<UserEntity> Users { get; set; } = [];
        public List<CourseEntity> Courses { get; set; } = [];
        public List<StudentEntity> Students { get; set; } = [];
        public List<TeacherEntity> Teachers { get; set; } = [];
        public List<ClassroomEntity> Classrooms { get; set; } = [];
        public List<AttendanceEntity> Attendances { get; set; } = [];
        public List<CaptureDeviceEntity> CaptureDevice { get; set; } = [];
        public List<ClassroomSessionEntity> ClassroomSessions { get; set; } = [];
    }
}
