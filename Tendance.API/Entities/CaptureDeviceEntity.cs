using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tendance.API.Entities
{
    public class CaptureDeviceEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SchoolId { get; set; }
        public int? ClassroomId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string ClientKey { get; set; } = string.Empty;
        public CaptureDeviceType Type { get; set; }
        public CaptureDeviceMode Mode { get; set; }
        public DateTime Created { get; set; }

        public SchoolEntity? School { get; set; }
        public ClassroomEntity? Classroom { get; set; }
    }

    public class TrackerDeviceEntityConfiguration : IEntityTypeConfiguration<CaptureDeviceEntity>
    {
        public void Configure(EntityTypeBuilder<CaptureDeviceEntity> builder)
        {
            builder.HasIndex(td => td.ClientKey);

            builder.HasOne(cd => cd.School)
                .WithMany(s => s.CaptureDevice)
                .HasForeignKey(cd => cd.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cd => cd.Classroom)
                .WithMany(s => s.CaptureDevices)
                .HasForeignKey(cd => cd.ClassroomId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
