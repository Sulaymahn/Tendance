using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public string SchoolAssignedId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime Created { get; set; }

        public School? School { get; set; }
        public List<Course> Courses { get; set; } = [];
        public List<ClassroomSession> Sessions { get; set; } = [];
        public List<Classroom> Classrooms { get; set; } = [];
        public List<Attendance> Attendances { get; set; } = [];
    }
}
