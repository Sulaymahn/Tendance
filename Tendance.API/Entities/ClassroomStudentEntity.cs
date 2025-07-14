using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class ClassroomStudentEntity
    {
        public int ClassroomId { get; set; }
        public int StudentId { get; set; }
        public ClassroomEntity? Classroom { get; set; }
        public StudentEntity? Student { get; set; }
    }

    public class ClassroomStudentEntityConfiguration : IEntityTypeConfiguration<ClassroomStudentEntity>
    {
        public void Configure(EntityTypeBuilder<ClassroomStudentEntity> builder)
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
