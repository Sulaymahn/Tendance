using Tendance.API.DataTransferObjects.Course;
using Tendance.API.DataTransferObjects.Room;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.DataTransferObjects.Teacher;

namespace Tendance.API.DataTransferObjects.Classroom
{
    public class ClassroomForClientMinimal
    {
        public int Id { get; set; }
        public required RoomForClientMinimal Room { get; set; }
        public required CourseForClientMinimal Course { get; set; }
        public required TeacherForClientMinimal Teacher { get; set; }
        public IEnumerable<StudentForClientMinimal> Students { get; set; } = [];
    }
}
