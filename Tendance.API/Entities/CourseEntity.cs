using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class CourseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }

        public SchoolEntity? School { get; set; }
        public List<ClassroomEntity> Classrooms { get; set; } = [];
        public List<TeacherEntity> Teachers { get; set; } = [];
    }
}
