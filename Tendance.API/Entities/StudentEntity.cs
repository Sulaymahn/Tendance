using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class StudentEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime Created { get; set; }

        public SchoolEntity? School { get; set; }
        public List<CourseEntity> Courses { get; set; } = [];
        public List<ClassroomSessionEntity> Sessions { get; set; } = [];
        public List<ClassroomEntity> Classrooms { get; set; } = [];
        public List<AttendanceEntity> Attendances { get; set; } = [];
    }
}
