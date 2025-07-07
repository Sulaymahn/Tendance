using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class ClassroomStudent
    {
        public int ClassroomId { get; set; }
        public int StudentId { get; set; }
        public Classroom? Classroom { get; set; }
        public Student? Student { get; set; }
    }

    public class ClassroomStudentEntityConfiguration : IEntityTypeConfiguration<ClassroomStudent>
    {
        public void Configure(EntityTypeBuilder<ClassroomStudent> builder)
        {
            builder
                .HasOne(ct => ct.Student)
                .WithMany()
                .HasForeignKey(ct => ct.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(ct => ct.Classroom)
                .WithMany()
                .HasForeignKey(ct => ct.ClassroomId)
                .OnDelete(DeleteBehavior.ClientCascade);

        }
    }
}
