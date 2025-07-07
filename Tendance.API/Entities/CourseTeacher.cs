using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class CourseTeacher
    {
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public Teacher? Teacher { get; set; }
        public Course? Course { get; set; }
    }

    public class CourseTeacherEntityConfiguration : IEntityTypeConfiguration<CourseTeacher>
    {
        public void Configure(EntityTypeBuilder<CourseTeacher> builder)
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
