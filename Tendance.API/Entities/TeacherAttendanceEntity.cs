using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class TeacherAttendanceEntity : AttendanceEntity
    {
        public int TeacherId { get; set; }
        public TeacherEntity? Teacher { get; set; }
    }

    public class TeacherAttendanceEntityConfiguration : IEntityTypeConfiguration<TeacherAttendanceEntity>
    {
        public void Configure(EntityTypeBuilder<TeacherAttendanceEntity> builder)
        {
            builder
                .HasOne(att => att.Teacher)
                .WithMany()
                .HasForeignKey(att => att.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
