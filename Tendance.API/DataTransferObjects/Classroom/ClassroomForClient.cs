using Tendance.API.DataTransferObjects.Room;

namespace Tendance.API.DataTransferObjects.Classroom
{
    public class ClassroomForClient
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }

        public required RoomForClient Room { get; set; }
        public required ClassroomCourse Course { get; set; }
        public required ClassroomTeacher Teacher { get; set; }
        public IEnumerable<ClassroomStudent> Students { get; set; } = [];
    }
}
