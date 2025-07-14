using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class StudentAttendanceEntity : AttendanceEntity
    {
        public int StudentId { get; set; }
        public StudentEntity? Student { get; set; }
    }

    public class StudentAttendanceEntityConfiguration : IEntityTypeConfiguration<StudentAttendanceEntity>
    {
        public void Configure(EntityTypeBuilder<StudentAttendanceEntity> builder)
        {
            builder
                .HasOne(att => att.Student)
                .WithMany()
                .HasForeignKey(att => att.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
