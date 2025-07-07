using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }

        public School? School { get; set; }
        public List<Classroom> Classrooms { get; set; } = [];
        public List<Teacher> Teachers { get; set; } = [];
    }
}
