using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public abstract class Attendance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public AttendanceType Type { get; set; }
    }
}
