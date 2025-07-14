namespace Tendance.API.Entities
{
    public class TeacherFaceEntity : FaceEntity
    {
        public int TeacherId { get; set; }
        public TeacherEntity? Teacher { get; set; }
    }
}
