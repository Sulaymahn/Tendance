namespace Tendance.API.Entities
{
    public class StudentFaceEntity : FaceEntity
    {
        public int StudentId { get; set; }
        public StudentEntity? Student { get; set; }
    }
}
