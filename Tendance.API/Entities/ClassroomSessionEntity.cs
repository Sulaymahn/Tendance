using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class ClassroomSessionEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassroomId { get; set; }
        public string? Topic { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime CheckInFrom { get; set; }
        public DateTime CheckInTo { get; set; }
        public DateTime CheckOutFrom { get; set; }
        public DateTime CheckOutTo { get; set; }
        public string? Note { get; set; }
        public DateTime Created { get; set; }

        public ClassroomEntity? Classroom { get; set; }
        public List<AttendanceEntity> Attendances { get; set; } = [];
    }
}
