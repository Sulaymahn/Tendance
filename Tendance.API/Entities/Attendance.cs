using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class Attendance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public DateTime Timestamp { get; set; }
        public AttendanceRole Role { get; set; }
        public AttendanceType Type { get; set; }
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
