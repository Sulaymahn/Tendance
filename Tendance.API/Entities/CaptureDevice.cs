using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tendance.API.Entities
{
    public class CaptureDevice
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public int? ClassroomId { get; set; }
        public bool IsActive { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string ClientKey { get; set; } = string.Empty;
        public CaptureDeviceType Type { get; set; }
        public DateTime Created { get; set; }

        public Classroom? Classroom { get; set; }
    }

    public class TrackerDeviceEntityConfiguration : IEntityTypeConfiguration<CaptureDevice>
    {
        public void Configure(EntityTypeBuilder<CaptureDevice> builder)
        {
            builder.HasIndex(td => td.ClientKey);
        }
    }
}
