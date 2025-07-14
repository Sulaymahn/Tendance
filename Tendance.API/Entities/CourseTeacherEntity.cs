using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class CourseTeacherEntity
    {
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public TeacherEntity? Teacher { get; set; }
        public CourseEntity? Course { get; set; }
    }

    public class CourseTeacherEntityConfiguration : IEntityTypeConfiguration<CourseTeacherEntity>
    {
        public void Configure(EntityTypeBuilder<CourseTeacherEntity> builder)
        {
            builder
                .HasOne(ct => ct.Course)
                .WithMany()
                .HasForeignKey(ct => ct.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(ct => ct.Teacher)
                .WithMany()
                .HasForeignKey(ct => ct.TeacherId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
