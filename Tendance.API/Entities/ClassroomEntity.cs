using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class ClassroomEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public int CourseId { get; set; }
        public int RoomId { get; set; }
        public int TeacherId { get; set; }
        public DateTime Created { get; set; }

        public SchoolEntity? School { get; set; }
        public CourseEntity? Course { get; set; }
        public RoomEntity? Room { get; set; }
        public TeacherEntity? Teacher { get; set; }
        public List<StudentEntity> Students { get; set; } = [];
        public List<ClassroomSessionEntity> ClassroomSessions { get; set; } = [];
        public List<CaptureDeviceEntity> CaptureDevices { get; set; } = [];
    }

    public class ClassroomEntityConfiguration : IEntityTypeConfiguration<ClassroomEntity>
    {
        public void Configure(EntityTypeBuilder<ClassroomEntity> builder)
        {
            builder.HasOne(c => c.School)
                .WithMany(s => s.Classrooms)
                .HasForeignKey(c => c.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Course)
                .WithMany(c => c.Classrooms)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(c => c.Room)
                .WithMany(r => r.Classrooms)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(c => c.Teacher)
                .WithMany(t => t.Classrooms)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany(c => c.Students)
                .WithMany(c => c.Classrooms)
                .UsingEntity<ClassroomStudentEntity>();
        }
    }
}
