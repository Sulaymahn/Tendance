namespace Tendance.API.DataTransferObjects.Room
{
    public class RoomForClient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Building { get; set; }
    }
}