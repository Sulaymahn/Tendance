using System.ComponentModel.DataAnnotations;

namespace Tendance.API.DataTransferObjects.ClassroomSession
{
    public class ClassroomSessionForCreation
    {
        public int ClassroomId { get; set; }
        public string? Topic { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime CheckInFrom { get; set; }
        public DateTime CheckInTo { get; set; }
        public DateTime CheckOutFrom { get; set; }
        public DateTime CheckOutTo { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
