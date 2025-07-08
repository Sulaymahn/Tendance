using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class Teacher
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Created { get; set; }

        public School? School { get; set; }
        public List<Course> Courses { get; set; } = [];
        public List<Classroom> Classrooms { get; set; } = [];
        public List<ClassroomSession> Sessions { get; set; } = [];
        public List<Attendance> Attendances { get; set; } = [];
    }
}
