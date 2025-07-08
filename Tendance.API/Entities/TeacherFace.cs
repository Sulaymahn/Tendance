namespace Tendance.API.Entities
{
    public class TeacherFace : Face
    {
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
