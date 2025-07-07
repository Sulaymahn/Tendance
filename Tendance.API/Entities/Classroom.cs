using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class Classroom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public int CourseId { get; set; }
        public int RoomId { get; set; }
        public int TeacherId { get; set; }
        public DateTime Created { get; set; }

        public School? School { get; set; }
        public Course? Course { get; set; }
        public Room? Room { get; set; }
        public Teacher? Teacher { get; set; }
        public List<Student> Students { get; set; } = [];
    }

    public class ClassroomEntityConfiguration : IEntityTypeConfiguration<Classroom>
    {
        public void Configure(EntityTypeBuilder<Classroom> builder)
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
                .UsingEntity<ClassroomStudent>();
        }
    }
}
