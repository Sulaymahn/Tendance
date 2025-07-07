using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class ClassroomSession
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassroomId { get; set; }
        public string? Topic { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Classroom? Classroom { get; set; }
        public List<Attendance> Attendances { get; set; } = [];
    }
}
