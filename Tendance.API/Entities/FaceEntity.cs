using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Tendance.API.Entities
{
    public abstract class FaceEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CaptureDeviceId { get; set; }
        public DateTime Created { get; set; }

        [NotMapped]
        public float[] Embedding { get; set; } = [];

        public string EmbeddingJson
        {
            get => JsonSerializer.Serialize(Embedding);
            set => Embedding = JsonSerializer.Deserialize<float[]>(value) ?? [];
        }

        public CaptureDeviceEntity? CaptureDevice { get; set; }
    }
}
