using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public abstract class AttendanceEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public int ClassroomSessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public AttendanceType Type { get; set; }
        public SchoolEntity? School { get; set; }
        public ClassroomSessionEntity? ClassroomSession { get; set; }
    }

    public class AttendanceEntityConfiguration : IEntityTypeConfiguration<AttendanceEntity>
    {
        public void Configure(EntityTypeBuilder<AttendanceEntity> builder)
        {
            builder
                .HasDiscriminator<string>("Role")
                .HasValue<StudentAttendanceEntity>("Student")
                .HasValue<TeacherAttendanceEntity>("Teacher");

            builder
                .HasOne(att => att.School)
                .WithMany(ss => ss.Attendances)
                .HasForeignKey(att => att.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(att => att.ClassroomSession)
                .WithMany(session => session.Attendances)
                .HasForeignKey(att => att.ClassroomSessionId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
