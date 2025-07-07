namespace Tendance.API.DataTransferObjects.Room
{
    public class RoomForCreation
    {
        public required string Name { get; set; }
        public string? Building { get; set; }
    }
}